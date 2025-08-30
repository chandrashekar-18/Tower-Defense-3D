using UnityEngine;
using TowerDefense.Enemies;

namespace TowerDefense.Towers
{
    public class AOEProjectile : Projectile
    {
        [SerializeField] private float _aoeRadius = 2f;
        
        public void SetAOERadius(float radius)
        {
            _aoeRadius = radius;
        }
        
        protected override void OnHit()
        {
            // Deal AOE damage
            Collider[] colliders = Physics.OverlapSphere(transform.position, _aoeRadius);
            foreach (Collider collider in colliders)
            {
                EnemyController enemy = collider.GetComponent<EnemyController>();
                if (enemy != null && enemy.IsAlive)
                {
                    enemy.TakeDamage(_damage);
                }
            }
            
            // Spawn impact effect
            if (_impactEffect != null)
            {
                Instantiate(_impactEffect, transform.position, transform.rotation);
            }
            
            // Destroy projectile
            Destroy(gameObject);
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _aoeRadius);
        }
    }
}