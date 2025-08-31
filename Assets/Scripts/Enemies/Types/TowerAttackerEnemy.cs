using UnityEngine;
using TowerDefense.Towers;

namespace TowerDefense.Enemies
{
    /// <summary>
    /// Enemy that can attack and temporarily disable towers
    /// </summary>
    public class TowerAttackerEnemy : Enemy
    {
        #region Variables
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

        #region Base Methods
        protected override void InitializeFromData()
        {
            base.InitializeFromData();

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
            if (canAttackTowers)
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
        }
        #endregion

        #region Private Methods
        private void FindTowerTarget()
        {
            // Find nearest tower within range
            Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange);

            foreach (Collider collider in colliders)
            {
                Tower tower = collider.GetComponent<Tower>();
                if (tower != null)
                {
                    float distance = Vector3.Distance(transform.position, tower.transform.position);
                    if (distance <= attackRange)
                    {
                        currentTowerTarget = tower;
                        break;
                    }
                }
            }
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

            currentTowerTarget.Stun(towerStunDuration);

            OnAttackedTower?.Invoke(currentTowerTarget);
        }
        #endregion
    }
}