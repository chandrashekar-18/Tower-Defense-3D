using UnityEngine;
using TowerDefense.Enemies;

namespace TowerDefense.Towers
{
    /// <summary>
    /// Base class for tower projectiles
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        #region Properties
        [SerializeField] protected float _speed = 15f;
        [SerializeField] protected int _damage = 10;
        [SerializeField] protected float _lifetime = 5f;
        [SerializeField] protected GameObject _impactEffect;
        
        protected EnemyController _target;
        protected float _lifetimeTimer;
        
        public int Damage => _damage;
        #endregion

        #region Unity Lifecycle
        protected virtual void Start()
        {
            _lifetimeTimer = _lifetime;
        }
        
        protected virtual void Update()
        {
            if (_target == null || !_target.IsAlive)
            {
                Destroy(gameObject);
                return;
            }
            
            // Move towards target
            Vector3 direction = _target.transform.position - transform.position;
            float distanceThisFrame = _speed * Time.deltaTime;
            
            if (direction.magnitude <= distanceThisFrame)
            {
                // Hit target
                OnHit();
            }
            else
            {
                // Move towards target
                transform.Translate(direction.normalized * distanceThisFrame, Space.World);
                transform.LookAt(_target.transform);
            }
            
            // Lifetime check
            _lifetimeTimer -= Time.deltaTime;
            if (_lifetimeTimer <= 0f)
            {
                Destroy(gameObject);
            }
        }
        #endregion

        #region Public Methods
        public virtual void Initialize(EnemyController target, int damage)
        {
            _target = target;
            _damage = damage;
        }
        #endregion

        #region Protected Methods
        protected virtual void OnHit()
        {
            // Deal damage to target
            _target.TakeDamage(_damage);
            
            // Spawn impact effect
            if (_impactEffect != null)
            {
                Instantiate(_impactEffect, transform.position, transform.rotation);
            }
            
            // Destroy projectile
            Destroy(gameObject);
        }
        #endregion
    }
}