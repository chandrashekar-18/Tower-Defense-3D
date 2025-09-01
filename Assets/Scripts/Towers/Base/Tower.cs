using UnityEngine;
using TowerDefense.Core;
using TowerDefense.Enemies;
using TowerDefense.UI.Components;

namespace TowerDefense.Towers
{
    /// <summary>
    /// Base class for all towers.
    /// </summary>
    public abstract class Tower : MonoBehaviour
    {
        #region Variables
        [Header("Base Tower Properties")]
        [SerializeField] protected TowerHealthUI healthUI;
        [SerializeField] protected float rotationSpeed = 5f;
        [SerializeField] protected Transform turretHead;
        [SerializeField] protected Transform firingPoint;
        [SerializeField] protected GameObject projectilePrefab;
        [SerializeField] protected LayerMask enemyLayerMask;

        protected TowerData towerData;
        protected float attackTimer = 0f;
        protected Enemy currentTarget;
        protected bool isStunned = false;
        protected float stunTimer = 0f;
        protected int currentHealth;
        protected bool isDestroyed = false;
        #endregion

        #region Properties
        public TowerData TowerData => towerData;
        public float AttackRange => towerData.Range;
        public int Damage => towerData.Damage;
        public bool IsStunned => isStunned;
        public int CurrentHealth => currentHealth;
        public int MaxHealth => towerData.MaxHealth;
        public bool IsDestroyed => isDestroyed;
        public bool CanBeRepaired => towerData.CanBeRepaired;
        public int RepairCost => towerData.RepairCost;
        #endregion

        #region Events
        public delegate void TowerHealthChangedDelegate(Tower tower, int currentHealth, int maxHealth);
        public event TowerHealthChangedDelegate OnHealthChanged;

        public delegate void TowerDestroyedDelegate(Tower tower);
        public event TowerDestroyedDelegate OnDestroyed;

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
            attackTimer = 0f;
        }

        protected virtual void Update()
        {
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

            if (currentTarget == null)
            {
                FindTarget();
            }
            else
            {
                if (!IsValidTarget(currentTarget))
                {
                    currentTarget = null;
                    FindTarget();
                }
                else
                {
                    RotateTowardsTarget();

                    attackTimer -= Time.deltaTime;
                    if (attackTimer <= 0f)
                    {
                        Attack();
                        attackTimer = 1f / towerData.FireRate;
                    }
                }
            }
        }
        #endregion

        #region Public Methods
        public virtual void Initialize(TowerData towerData)
        {
            this.towerData = towerData;
            currentHealth = towerData.MaxHealth;
            isDestroyed = false;
            healthUI.Initialize(this);
        }

        public virtual void TakeDamage(int damage)
        {
            if (isDestroyed)
                return;

            currentHealth -= damage;
            OnHealthChanged?.Invoke(this, currentHealth, towerData.MaxHealth);

            if (currentHealth <= 0)
            {
                DestroyTower();
            }
        }

        public virtual bool Repair()
        {
            if (!isDestroyed || !towerData.CanBeRepaired)
                return false;

            if (ResourceManager.Instance.SpendCurrency(towerData.RepairCost))
            {
                currentHealth = towerData.MaxHealth;
                isDestroyed = false;
                OnHealthChanged?.Invoke(this, currentHealth, towerData.MaxHealth);
                return true;
            }

            return false;
        }

        protected virtual void DestroyTower()
        {
            isDestroyed = true;
            OnDestroyed?.Invoke(this);

            currentTarget = null;
            isStunned = true;
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
            Collider[] colliders = Physics.OverlapSphere(transform.position, towerData.Range, enemyLayerMask);

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
            return distance <= towerData.Range;
        }

        protected virtual void RotateTowardsTarget()
        {
            if (currentTarget == null || turretHead == null)
                return;

            Vector3 targetDirection = currentTarget.transform.position - turretHead.position;
            targetDirection.y = 0f;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            turretHead.rotation = Quaternion.Slerp(turretHead.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        protected virtual void Attack()
        {
            if (currentTarget == null)
                return;

            if (projectilePrefab != null && firingPoint != null)
            {
                GameObject projectileObj = Instantiate(projectilePrefab, firingPoint.position, firingPoint.rotation);
                Projectile projectile = projectileObj.GetComponent<Projectile>();

                if (projectile != null)
                {
                    projectile.Initialize(currentTarget, towerData.Damage);
                }
            }
            else
            {
                currentTarget.TakeDamage(towerData.Damage);
            }

            OnTowerFired?.Invoke(this, currentTarget);
        }
        #endregion
    }
}