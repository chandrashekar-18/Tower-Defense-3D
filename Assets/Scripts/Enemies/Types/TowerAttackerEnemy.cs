using UnityEngine;
using TowerDefense.Towers;

namespace TowerDefense.Enemies
{
    /// <summary>
    /// Enemy that can attack and temporarily disable towers.
    /// Example of how to add complex behaviors.
    /// </summary>
    public class TowerAttackerEnemy : Enemy
    {
        #region Tower Attacker Variables
        [Header("Tower Attacker Properties")]
        [SerializeField] private bool canAttackTowers = true;
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float attackRate = 1f;
        [SerializeField] private float towerStunDuration = 3f;

        private float attackTimer = 0f;
        private Tower currentTowerTarget;
        #endregion

        #region Events
        public delegate void EnemyAttackedTowerDelegate(Tower tower);
        public event EnemyAttackedTowerDelegate OnAttackedTower;
        #endregion

        #region Overridden Methods
        protected override void OnEnemyInitialized()
        {
            base.OnEnemyInitialized();

            if (enemyData != null)
            {
                canAttackTowers = enemyData.CanAttackTowers;
                attackRange = enemyData.AttackRange;
                attackRate = enemyData.AttackRate;
                towerStunDuration = enemyData.TowerStunDuration;
            }
        }

        protected override void ExecuteBehavior()
        {
            base.ExecuteBehavior();
            
            if (canAttackTowers)
            {
                HandleTowerAttacking();
            }
        }
        #endregion

        #region Private Methods
        private void HandleTowerAttacking()
        {
            // Check for towers in range
            if (currentTowerTarget == null)
            {
                FindTowerTarget();
            }
            else
            {
                // Check if target is still valid
                if (!IsValidTowerTarget(currentTowerTarget))
                {
                    currentTowerTarget = null;
                }
                else
                {
                    // Attack if ready
                    attackTimer -= Time.deltaTime;
                    if (attackTimer <= 0f)
                    {
                        AttackTower();
                        attackTimer = 1f / attackRate;
                    }
                }
            }
        }

        private void FindTowerTarget()
        {
            // Find nearest tower within range
            Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange);

            Tower closestTower = null;
            float closestDistance = float.MaxValue;

            foreach (Collider collider in colliders)
            {
                Tower tower = collider.GetComponent<Tower>();
                if (tower != null)
                {
                    float distance = Vector3.Distance(transform.position, tower.transform.position);
                    if (distance <= attackRange && distance < closestDistance)
                    {
                        closestTower = tower;
                        closestDistance = distance;
                    }
                }
            }

            currentTowerTarget = closestTower;
        }

        private bool IsValidTowerTarget(Tower tower)
        {
            if (tower == null)
                return false;

            float distance = Vector3.Distance(transform.position, tower.transform.position);
            return distance <= attackRange;
        }

        private void AttackTower()
        {
            if (currentTowerTarget == null)
                return;

            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            // Stun the tower
            currentTowerTarget.Stun(towerStunDuration);

            // Trigger event
            OnAttackedTower?.Invoke(currentTowerTarget);
            
            Debug.Log($"Tower Attacker stunned tower for {towerStunDuration} seconds!");
        }
        #endregion
    }
}