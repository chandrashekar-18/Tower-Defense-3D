using UnityEngine;
using TowerDefense.Enemies;

namespace TowerDefense.Towers
{
    public class AOEProjectile : Projectile
    {
        #region Variables
        [SerializeField] private float aoeRadius = 2f;
        #endregion

        #region Public Methods
        public void SetAOERadius(float radius)
        {
            aoeRadius = radius;
        }
        #endregion

        #region Protected Methods
        protected override void OnHit()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, aoeRadius);
            foreach (Collider collider in colliders)
            {
                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy != null && enemy.IsAlive)
                {
                    enemy.TakeDamage(damage);
                }
            }

            if (impactEffect != null)
            {
                Instantiate(impactEffect, transform.position, transform.rotation);
            }

            Destroy(gameObject);
        }
        #endregion

        #region Private Methods
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, aoeRadius);
        }
        #endregion
    }
}