using UnityEngine;
using TowerDefense.Core;
using System.Collections;
using TowerDefense.Towers;
using System.Collections.Generic;

namespace TowerDefense.Enemies
{
    /// <summary>
    /// Base class for all enemies
    /// </summary>
    public class EnemyController : MonoBehaviour
    {
        #region Properties
        [Header("Base Enemy Properties")]
        [SerializeField] protected EnemyData _enemyData;
        [SerializeField] protected int _maxHealth = 100;
        [SerializeField] protected int _currentHealth;
        [SerializeField] protected float _moveSpeed = 3f;
        [SerializeField] protected int _damage = 1;
        [SerializeField] protected int _currencyReward = 20;
        [SerializeField] protected bool _canAttackTowers = false;
        [SerializeField] protected float _attackRange = 2f;
        [SerializeField] protected float _attackRate = 1f;
        [SerializeField] protected float _towerStunDuration = 3f;

        [SerializeField] protected List<Vector3> _path = new List<Vector3>();
        [SerializeField] protected int _currentPathIndex = 0;

        [SerializeField] protected Animator _animator;
        [SerializeField] protected GameObject _healthBarPrefab;
        [SerializeField] protected Transform _healthBarPoint;

        protected bool _isAlive = true;
        protected bool _reachedEnd = false;
        protected float _attackTimer = 0f;
        protected Tower _currentTowerTarget;
        protected GameObject _healthBar;

        public bool IsAlive => _isAlive;
        public bool ReachedEnd => _reachedEnd;
        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;
        public EnemyType EnemyType => _enemyData != null ? _enemyData.EnemyType : EnemyType.Basic;
        #endregion

        #region Events
        public delegate void EnemyDefeatedDelegate(EnemyController enemy);
        public event EnemyDefeatedDelegate OnDefeated;

        public delegate void EnemyReachedEndDelegate(EnemyController enemy);
        public event EnemyReachedEndDelegate OnReachedEnd;

        public delegate void EnemyHealthChangedDelegate(int currentHealth, int maxHealth);
        public event EnemyHealthChangedDelegate OnHealthChanged;

        public delegate void EnemyAttackedTowerDelegate(Tower tower);
        public event EnemyAttackedTowerDelegate OnAttackedTower;
        #endregion

        #region Unity Lifecycle
        protected virtual void Start()
        {
            InitializeFromData();

            // Create health bar
            if (_healthBarPrefab != null && _healthBarPoint != null)
            {
                _healthBar = Instantiate(_healthBarPrefab, _healthBarPoint.position, Quaternion.identity, _healthBarPoint);
            }

            // Set initial health
            _currentHealth = _maxHealth;
            UpdateHealthBar();
        }

        protected virtual void Update()
        {
            if (!_isAlive || _reachedEnd || GameManager.Instance.IsPaused)
                return;

            // Move along path
            if (_path.Count > 0 && _currentPathIndex < _path.Count)
            {
                MoveAlongPath();
            }

            // Attack tower if applicable
            if (_canAttackTowers)
            {
                // Check for towers in range
                if (_currentTowerTarget == null)
                {
                    FindTowerTarget();
                }
                else
                {
                    // Check if target is still valid
                    if (!IsValidTowerTarget(_currentTowerTarget))
                    {
                        _currentTowerTarget = null;
                    }
                    else
                    {
                        // Attack if ready
                        _attackTimer -= Time.deltaTime;
                        if (_attackTimer <= 0f)
                        {
                            AttackTower();
                            _attackTimer = 1f / _attackRate;
                        }
                    }
                }
            }
        }
        #endregion

        #region Public Methods
        public virtual void Initialize(EnemyData enemyData, List<Vector3> path)
        {
            _enemyData = enemyData;
            _path = new List<Vector3>(path); // Create a copy
            _currentPathIndex = 0;

            InitializeFromData();
        }

        public virtual void TakeDamage(int damage)
        {
            if (!_isAlive)
                return;

            _currentHealth -= damage;
            UpdateHealthBar();

            // Notify listeners
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            // Play hit animation
            if (_animator != null)
            {
                _animator.SetTrigger("Hit");
            }

            if (_currentHealth <= 0)
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
        protected virtual void InitializeFromData()
        {
            if (_enemyData != null)
            {
                _maxHealth = _enemyData.Health;
                _currentHealth = _maxHealth;
                _moveSpeed = _enemyData.MoveSpeed;
                _damage = _enemyData.Damage;
                _currencyReward = _enemyData.CurrencyReward;
                _canAttackTowers = _enemyData.CanAttackTowers;
                _attackRange = _enemyData.AttackRange;
                _attackRate = _enemyData.AttackRate;
                _towerStunDuration = _enemyData.TowerStunDuration;
            }
        }

        protected virtual void MoveAlongPath()
        {
            Vector3 targetPosition = _path[_currentPathIndex];
            Vector3 moveDirection = (targetPosition - transform.position).normalized;

            // Rotate towards movement direction
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
            }

            // Move towards target
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                _moveSpeed * Time.deltaTime
            );

            // Check if reached waypoint
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
            if (distanceToTarget < 0.1f)
            {
                // Move to next waypoint
                _currentPathIndex++;

                // Check if reached end
                if (_currentPathIndex >= _path.Count)
                {
                    ReachEnd();
                }
                else
                {
                    // Update animation
                    if (_animator != null)
                    {
                        _animator.SetTrigger("NextWaypoint");
                    }
                }
            }
        }

        protected virtual void ReachEnd()
        {
            _reachedEnd = true;

            // Damage player
            GameManager.Instance.ReducePlayerLives(_damage);

            // Notify listeners
            OnReachedEnd?.Invoke(this);

            // Destroy this enemy
            Destroy(gameObject);
        }

        protected virtual void Die()
        {
            _isAlive = false;

            // Award currency
            ResourceManager.Instance.AddCurrency(_currencyReward);

            // Play death animation
            if (_animator != null)
            {
                _animator.SetTrigger("Die");

                // Wait for animation to finish before destroying
                StartCoroutine(DestroyAfterAnimation());
            }
            else
            {
                // Destroy immediately
                Destroy(gameObject, 0.1f);
            }

            // Notify listeners
            OnDefeated?.Invoke(this);
        }

        protected virtual IEnumerator DestroyAfterAnimation()
        {
            // Wait for death animation
            yield return new WaitForSeconds(1.5f);

            // Destroy object
            Destroy(gameObject);
        }

        protected virtual void UpdateHealthBar()
        {
            if (_healthBar != null)
            {
                // Update health bar scale
                Transform fill = _healthBar.transform.Find("Fill");
                if (fill != null)
                {
                    float healthPercent = (float)_currentHealth / _maxHealth;
                    fill.localScale = new Vector3(healthPercent, 1, 1);
                }
            }
        }

        protected virtual void FindTowerTarget()
        {
            // Find nearest tower within range
            Collider[] colliders = Physics.OverlapSphere(transform.position, _attackRange);

            foreach (Collider collider in colliders)
            {
                Tower tower = collider.GetComponent<Tower>();
                if (tower != null)
                {
                    float distance = Vector3.Distance(transform.position, tower.transform.position);
                    if (distance <= _attackRange)
                    {
                        _currentTowerTarget = tower;
                        break;
                    }
                }
            }
        }

        protected virtual bool IsValidTowerTarget(Tower tower)
        {
            if (tower == null)
                return false;

            float distance = Vector3.Distance(transform.position, tower.transform.position);
            return distance <= _attackRange;
        }

        protected virtual void AttackTower()
        {
            if (_currentTowerTarget == null)
                return;

            // Play attack animation
            if (_animator != null)
            {
                _animator.SetTrigger("Attack");
            }

            // Stun tower
            _currentTowerTarget.Stun(_towerStunDuration);

            // Notify listeners
            OnAttackedTower?.Invoke(_currentTowerTarget);
        }

        protected virtual IEnumerator SlowRoutine(float slowFactor, float duration)
        {
            float originalSpeed = _moveSpeed;
            _moveSpeed *= (1f - slowFactor);

            yield return new WaitForSeconds(duration);

            _moveSpeed = originalSpeed;
        }
        #endregion
    }
}