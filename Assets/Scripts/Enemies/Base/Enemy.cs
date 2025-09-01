using UnityEngine;
using TowerDefense.Core;
using System.Collections;
using System.Collections.Generic;
using TowerDefense.UI.Components;

namespace TowerDefense.Enemies
{
    /// <summary>
    /// Base class for all enemies.
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        #region Variables
        [Header("Base Enemy Properties")]
        [SerializeField] protected Animator animator;
        [SerializeField] protected EnemyHealthUI healthUI;

        protected EnemyData enemyData;
        protected int currentHealth;
        protected List<Vector3> path = new List<Vector3>();
        protected int currentPathIndex = 0;
        protected float moveSpeed;
        protected bool isAlive = true;
        protected bool reachedEnd = false;
        #endregion

        #region Properties
        public bool IsAlive => isAlive;
        public bool ReachedEnd => reachedEnd;
        public int CurrentHealth => currentHealth;
        public int MaxHealth => enemyData.Health;
        public string EnemyId => enemyData.EnemyId;
        public EnemyData EnemyData => enemyData;
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
            Initialize();
            OnEnemyInitialized();
        }

        protected virtual void Update()
        {
            if (!isAlive || reachedEnd || GameManager.Instance.IsPaused)
                return;

            if (path.Count > 0 && currentPathIndex < path.Count)
            {
                MoveAlongPath();
            }

            // Call custom behavior for derived classes
            ExecuteBehavior();
        }
        #endregion

        #region Public Methods
        public virtual void Initialize(EnemyData enemyData, List<Vector3> path)
        {
            this.enemyData = enemyData;
            this.path = new List<Vector3>(path); // Create a copy
            currentPathIndex = 0;

            Initialize();
        }

        public virtual void TakeDamage(int damage)
        {
            if (!isAlive)
                return;

            // Allow derived classes to modify damage
            int finalDamage = ProcessIncomingDamage(damage);

            currentHealth -= finalDamage;

            OnHealthChanged?.Invoke(currentHealth, enemyData.Health);

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

        #region Protected Virtual Methods - Override these in derived classes
        /// <summary>
        /// Override this method in derived classes to implement specific enemy behavior
        /// </summary>
        protected virtual void ExecuteBehavior()
        {
            // Base implementation does nothing
        }

        /// <summary>
        /// Called after basic initialization is complete
        /// </summary>
        protected virtual void OnEnemyInitialized()
        {
            // Base implementation does nothing
        }

        /// <summary>
        /// Override to modify incoming damage (for armor, resistances, etc.)
        /// </summary>
        protected virtual int ProcessIncomingDamage(int damage)
        {
            return damage; // No modification by default
        }

        /// <summary>
        /// Override to add custom death behavior
        /// </summary>
        protected virtual void OnEnemyDeath()
        {
            // Base implementation does nothing
        }
        #endregion

        #region Protected Methods
        protected virtual void Initialize()
        {
            currentHealth = enemyData.Health;
            moveSpeed = enemyData.MoveSpeed;
            healthUI.Initialize(this);
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

            GameManager.Instance.ReducePlayerLives(enemyData.Damage);

            OnReachedEnd?.Invoke(this);

            Destroy(gameObject);
        }

        protected virtual void Die()
        {
            isAlive = false;

            ResourceManager.Instance.AddCurrency(enemyData.CurrencyReward);

            OnEnemyDeath();

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

        protected virtual IEnumerator SlowRoutine(float slowFactor, float duration)
        {
            float originalSpeed = enemyData.MoveSpeed;
            moveSpeed *= (1f - slowFactor);

            yield return new WaitForSeconds(duration);

            moveSpeed = originalSpeed;
        }
        #endregion
    }
}