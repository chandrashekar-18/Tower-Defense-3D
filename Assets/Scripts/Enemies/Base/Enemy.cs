using UnityEngine;
using TowerDefense.Core;
using System.Collections;
using TowerDefense.Towers;
using System.Collections.Generic;

namespace TowerDefense.Enemies
{
    /// <summary>
    /// Base class for all enemies.
    /// </summary>
    public abstract class Enemy : MonoBehaviour
    {
        #region Variables
        [Header("Base Enemy Properties")]
        [SerializeField] protected EnemyData enemyData;
        [SerializeField] protected int maxHealth = 100;
        [SerializeField] protected int currentHealth;
        [SerializeField] protected float moveSpeed = 3f;
        [SerializeField] protected int damage = 1;
        [SerializeField] protected int currencyReward = 20;
        [SerializeField] protected List<Vector3> path = new List<Vector3>();
        [SerializeField] protected int currentPathIndex = 0;
        [SerializeField] protected Animator animator;
        [SerializeField] protected GameObject healthBarPrefab;
        [SerializeField] protected Transform healthBarPoint;

        protected bool isAlive = true;
        protected bool reachedEnd = false;
        protected GameObject healthBar;
        #endregion

        #region Properties
        public bool IsAlive => isAlive;
        public bool ReachedEnd => reachedEnd;
        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;
        public EnemyType EnemyType => enemyData != null ? enemyData.EnemyType : EnemyType.Basic;
        #endregion

        #region Events
        public delegate void EnemyDefeatedDelegate(Enemy enemy);
        public event EnemyDefeatedDelegate OnDefeated;

        public delegate void EnemyReachedEndDelegate(Enemy enemy);
        public event EnemyReachedEndDelegate OnReachedEnd;

        public delegate void EnemyHealthChangedDelegate(int currentHealth, int maxHealth);
        public event EnemyHealthChangedDelegate OnHealthChanged;
        #endregion

        #region Unity Lifecycle
        protected virtual void Start()
        {
            InitializeFromData();

            if (healthBarPrefab != null && healthBarPoint != null)
            {
                healthBar = Instantiate(healthBarPrefab, healthBarPoint.position, Quaternion.identity, healthBarPoint);
            }

            currentHealth = maxHealth;
            UpdateHealthBar();
        }

        protected virtual void Update()
        {
            if (!isAlive || reachedEnd || GameManager.Instance.IsPaused)
                return;

            if (path.Count > 0 && currentPathIndex < path.Count)
            {
                MoveAlongPath();
            }
            
            ExecuteBehavior();
        }
        #endregion

        #region Public Methods
        public virtual void Initialize(EnemyData enemyData, List<Vector3> path)
        {
            this.enemyData = enemyData;
            this.path = new List<Vector3>(path); // Create a copy
            currentPathIndex = 0;

            InitializeFromData();
        }

        public virtual void TakeDamage(int damage)
        {
            if (!isAlive)
                return;

            currentHealth -= damage;
            UpdateHealthBar();

            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            if (animator != null)
            {
                animator.SetTrigger("Hit");
            }

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public virtual void Slow(float slowFactor, float duration)
        {
            StartCoroutine(SlowRoutine(slowFactor, duration));
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Override this method in derived classes to implement specific enemy behavior
        /// </summary>
        protected virtual void ExecuteBehavior()
        {
            
        }
        
        protected virtual void InitializeFromData()
        {
            if (enemyData != null)
            {
                maxHealth = enemyData.Health;
                currentHealth = maxHealth;
                moveSpeed = enemyData.MoveSpeed;
                damage = enemyData.Damage;
                currencyReward = enemyData.CurrencyReward;
            }
        }

        protected virtual void MoveAlongPath()
        {
            Vector3 targetPosition = path[currentPathIndex];
            Vector3 moveDirection = (targetPosition - transform.position).normalized;

            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
            }

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
            if (distanceToTarget < 0.1f)
            {
                currentPathIndex++;

                if (currentPathIndex >= path.Count)
                {
                    ReachEnd();
                }
                else
                {
                    if (animator != null)
                    {
                        animator.SetTrigger("NextWaypoint");
                    }
                }
            }
        }

        protected virtual void ReachEnd()
        {
            reachedEnd = true;

            GameManager.Instance.ReducePlayerLives(damage);

            OnReachedEnd?.Invoke(this);

            Destroy(gameObject);
        }

        protected virtual void Die()
        {
            isAlive = false;

            ResourceManager.Instance.AddCurrency(currencyReward);

            if (animator != null)
            {
                animator.SetTrigger("Die");

                StartCoroutine(DestroyAfterAnimation());
            }
            else
            {
                Destroy(gameObject, 0.1f);
            }

            OnDefeated?.Invoke(this);
        }

        protected virtual IEnumerator DestroyAfterAnimation()
        {
            yield return new WaitForSeconds(1.5f);
            Destroy(gameObject);
        }

        protected virtual void UpdateHealthBar()
        {
            if (healthBar != null)
            {
                Transform fill = healthBar.transform.Find("Fill");
                if (fill != null)
                {
                    float healthPercent = (float)currentHealth / maxHealth;
                    fill.localScale = new Vector3(healthPercent, 1, 1);
                }
            }
        }

        protected virtual IEnumerator SlowRoutine(float slowFactor, float duration)
        {
            float originalSpeed = moveSpeed;
            moveSpeed *= (1f - slowFactor);

            yield return new WaitForSeconds(duration);

            moveSpeed = originalSpeed;
        }
        #endregion
    }
}