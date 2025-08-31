using UnityEngine;
using TowerDefense.Enemies;

namespace TowerDefense.Towers
{
    /// <summary>
    /// Base class for all towers.
    /// </summary>
    public abstract class Tower : MonoBehaviour
    {
        #region Variables
        [Header("Base Tower Properties")]
        [SerializeField] protected TowerData towerData;
        [SerializeField] protected float attackRange = 3f;
        [SerializeField] protected float attackRate = 1f;
        [SerializeField] protected int damage = 10;
        [SerializeField] protected float rotationSpeed = 5f;
        [SerializeField] protected Transform turretHead;
        [SerializeField] protected Transform firingPoint;
        [SerializeField] protected GameObject projectilePrefab;
        [SerializeField] protected LayerMask enemyLayerMask;

        protected float attackTimer = 0f;
        protected Enemy currentTarget;
        protected bool isStunned = false;
        protected float stunTimer = 0f;
        #endregion

        #region Properties
        public TowerData TowerData => towerData;
        public float AttackRange => attackRange;
        public int Damage => damage;
        public bool IsStunned => isStunned;
        #endregion

        #region Events
        public delegate void TowerFiredDelegate(Tower tower, Enemy target);
        public TowerFiredDelegate OnTowerFired;

        public delegate void TowerStunnedDelegate(Tower tower, float duration);
        public event TowerStunnedDelegate OnTowerStunned;

        public delegate void TowerUnstunnedDelegate(Tower tower);
        public event TowerUnstunnedDelegate OnTowerUnstunned;
        #endregion

        #region Unity Lifecycle
        protected virtual void Start()
        {
            // Initialize tower from data
            if (towerData != null)
            {
                attackRange = towerData.Range;
                attackRate = towerData.FireRate;
                damage = towerData.Damage;
            }

            // Reset attack timer
            attackTimer = 0f;
        }

        protected virtual void Update()
        {
            // Handle stun effect
            if (isStunned)
            {
                stunTimer -= Time.deltaTime;
                if (stunTimer <= 0f)
                {
                    isStunned = false;
                    OnTowerUnstunned?.Invoke(this);
                }
                return;
            }

            // Find target if none
            if (currentTarget == null)
            {
                FindTarget();
            }
            else
            {
                // Check if target is still valid
                if (!IsValidTarget(currentTarget))
                {
                    currentTarget = null;
                    FindTarget();
                }
                else
                {
                    // Rotate towards target
                    RotateTowardsTarget();

                    // Attack if ready
                    attackTimer -= Time.deltaTime;
                    if (attackTimer <= 0f)
                    {
                        Attack();
                        attackTimer = 1f / attackRate;
                    }
                }
            }
        }

        protected virtual void OnDrawGizmosSelected()
        {
            // Draw attack range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
        #endregion

        #region Public Methods
        public virtual void Initialize(TowerData towerData)
        {
            this.towerData = towerData;
            attackRange = towerData.Range;
            attackRate = towerData.FireRate;
            damage = towerData.Damage;
        }

        public virtual void Upgrade()
        {
            // Implement in derived classes
        }

        public virtual void Stun(float duration)
        {
            isStunned = true;
            stunTimer = duration;
            OnTowerStunned?.Invoke(this, duration);
        }
        #endregion

        #region Protected Methods
        protected virtual void FindTarget()
        {
            // Find nearest enemy within range
            Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange, enemyLayerMask);

            float nearestDistance = float.MaxValue;
            Enemy nearestEnemy = null;

            foreach (Collider collider in colliders)
            {
                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy != null && enemy.IsAlive)
                {
                    float distance = Vector3.Distance(transform.position, enemy.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestEnemy = enemy;
                    }
                }
            }

            currentTarget = nearestEnemy;
        }

        protected virtual bool IsValidTarget(Enemy target)
        {
            if (target == null || !target.IsAlive)
                return false;

            float distance = Vector3.Distance(transform.position, target.transform.position);
            return distance <= attackRange;
        }

        protected virtual void RotateTowardsTarget()
        {
            if (currentTarget == null || turretHead == null)
                return;

            Vector3 targetDirection = currentTarget.transform.position - turretHead.position;
            targetDirection.y = 0f; // Keep rotation on horizontal plane

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            turretHead.rotation = Quaternion.Slerp(turretHead.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        protected virtual void Attack()
        {
            if (currentTarget == null)
                return;

            // Fire projectile
            if (projectilePrefab != null && firingPoint != null)
            {
                GameObject projectileObj = Instantiate(projectilePrefab, firingPoint.position, firingPoint.rotation);
                Projectile projectile = projectileObj.GetComponent<Projectile>();

                if (projectile != null)
                {
                    projectile.Initialize(currentTarget, damage);
                }
            }
            else
            {
                // Direct damage (no projectile)
                currentTarget.TakeDamage(damage);
            }

            // Notify listeners
            OnTowerFired?.Invoke(this, currentTarget);
        }
        #endregion
    }
}