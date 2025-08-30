using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TowerDefense.Enemies;

namespace TowerDefense.Towers
{
    /// <summary>
    /// Base class for all towers
    /// </summary>
    public abstract class Tower : MonoBehaviour
    {
        #region Properties
        [Header("Base Tower Properties")]
        [SerializeField] protected TowerData _towerData;
        [SerializeField] protected float _attackRange = 3f;
        [SerializeField] protected float _attackRate = 1f;
        [SerializeField] protected int _damage = 10;
        [SerializeField] protected float _rotationSpeed = 5f;
        
        [SerializeField] protected Transform _turretHead;
        [SerializeField] protected Transform _firingPoint;
        
        [SerializeField] protected GameObject _projectilePrefab;
        [SerializeField] protected LayerMask _enemyLayerMask;
        
        protected float _attackTimer = 0f;
        protected EnemyController _currentTarget;
        protected bool _isStunned = false;
        protected float _stunTimer = 0f;
        
        public TowerData TowerData => _towerData;
        public float AttackRange => _attackRange;
        public int Damage => _damage;
        public bool IsStunned => _isStunned;
        #endregion

        #region Events
        public delegate void TowerFiredDelegate(Tower tower, EnemyController target);
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
            if (_towerData != null)
            {
                _attackRange = _towerData.Range;
                _attackRate = _towerData.FireRate;
                _damage = _towerData.Damage;
            }
            
            // Reset attack timer
            _attackTimer = 0f;
        }
        
        protected virtual void Update()
        {
            // Handle stun effect
            if (_isStunned)
            {
                _stunTimer -= Time.deltaTime;
                if (_stunTimer <= 0f)
                {
                    _isStunned = false;
                    OnTowerUnstunned?.Invoke(this);
                }
                return;
            }
            
            // Find target if none
            if (_currentTarget == null)
            {
                FindTarget();
            }
            else
            {
                // Check if target is still valid
                if (!IsValidTarget(_currentTarget))
                {
                    _currentTarget = null;
                    FindTarget();
                }
                else
                {
                    // Rotate towards target
                    RotateTowardsTarget();
                    
                    // Attack if ready
                    _attackTimer -= Time.deltaTime;
                    if (_attackTimer <= 0f)
                    {
                        Attack();
                        _attackTimer = 1f / _attackRate;
                    }
                }
            }
        }
        
        protected virtual void OnDrawGizmosSelected()
        {
            // Draw attack range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _attackRange);
        }
        #endregion

        #region Public Methods
        public virtual void Initialize(TowerData towerData)
        {
            _towerData = towerData;
            _attackRange = towerData.Range;
            _attackRate = towerData.FireRate;
            _damage = towerData.Damage;
        }
        
        public virtual void Upgrade()
        {
            // Implement in derived classes
        }
        
        public virtual void Stun(float duration)
        {
            _isStunned = true;
            _stunTimer = duration;
            OnTowerStunned?.Invoke(this, duration);
        }
        #endregion

        #region Protected Methods
        protected virtual void FindTarget()
        {
            // Find nearest enemy within range
            Collider[] colliders = Physics.OverlapSphere(transform.position, _attackRange, _enemyLayerMask);
            
            float nearestDistance = float.MaxValue;
            EnemyController nearestEnemy = null;
            
            foreach (Collider collider in colliders)
            {
                EnemyController enemy = collider.GetComponent<EnemyController>();
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
            
            _currentTarget = nearestEnemy;
        }
        
        protected virtual bool IsValidTarget(EnemyController target)
        {
            if (target == null || !target.IsAlive)
                return false;
                
            float distance = Vector3.Distance(transform.position, target.transform.position);
            return distance <= _attackRange;
        }
        
        protected virtual void RotateTowardsTarget()
        {
            if (_currentTarget == null || _turretHead == null)
                return;
                
            Vector3 targetDirection = _currentTarget.transform.position - _turretHead.position;
            targetDirection.y = 0f; // Keep rotation on horizontal plane
            
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            _turretHead.rotation = Quaternion.Slerp(_turretHead.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
        
        protected virtual void Attack()
        {
            if (_currentTarget == null)
                return;
                
            // Fire projectile
            if (_projectilePrefab != null && _firingPoint != null)
            {
                GameObject projectileObj = Instantiate(_projectilePrefab, _firingPoint.position, _firingPoint.rotation);
                Projectile projectile = projectileObj.GetComponent<Projectile>();
                
                if (projectile != null)
                {
                    projectile.Initialize(_currentTarget, _damage);
                }
            }
            else
            {
                // Direct damage (no projectile)
                _currentTarget.TakeDamage(_damage);
            }
            
            // Notify listeners
            OnTowerFired?.Invoke(this, _currentTarget);
        }
        #endregion
    }
}