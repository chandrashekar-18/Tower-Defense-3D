using UnityEngine;
using TowerDefense.Enemies;

namespace TowerDefense.Towers
{
    /// <summary>
    /// Base class for tower projectiles.
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        #region Variables
        [SerializeField] protected float speed = 15f;
        [SerializeField] protected int damage = 10;
        [SerializeField] protected float lifetime = 5f;
        [SerializeField] protected GameObject impactEffect;

        protected Enemy target;
        protected float lifetimeTimer;
        #endregion

        #region Properties
        public int Damage => damage;
        #endregion

        #region Unity Lifecycle
        protected virtual void Start()
        {
            lifetimeTimer = lifetime;
        }

        protected virtual void Update()
        {
            if (target == null || !target.IsAlive)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 direction = target.transform.position - transform.position;
            float distanceThisFrame = speed * Time.deltaTime;

            if (direction.magnitude <= distanceThisFrame)
            {
                OnHit();
            }
            else
            {
                transform.Translate(direction.normalized * distanceThisFrame, Space.World);
                transform.LookAt(target.transform);
            }

            lifetimeTimer -= Time.deltaTime;
            if (lifetimeTimer <= 0f)
            {
                Destroy(gameObject);
            }
        }
        #endregion

        #region Public Methods
        public virtual void Initialize(Enemy target, int damage)
        {
            this.target = target;
            this.damage = damage;
        }
        #endregion

        #region Protected Methods
        protected virtual void OnHit()
        {
            target.TakeDamage(damage);

            if (impactEffect != null)
            {
                Instantiate(impactEffect, transform.position, transform.rotation);
            }

            Destroy(gameObject);
        }
        #endregion
    }
}