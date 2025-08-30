using UnityEngine;
using TowerDefense.Enemies;

namespace TowerDefense.Towers
{
    public class SlowProjectile : Projectile
    {
        [SerializeField] private float _slowEffect = 0.5f;
        [SerializeField] private float _slowDuration = 3f;
        
        public void SetSlowEffect(float effect, float duration)
        {
            _slowEffect = Mathf.Clamp01(effect);
            _slowDuration = duration;
        }
        
        protected override void OnHit()
        {
            // Deal damage
            _target.TakeDamage(_damage);
            
            // Apply slow effect
            _target.Slow(_slowEffect, _slowDuration);
            
            // Spawn impact effect
            if (_impactEffect != null)
            {
                Instantiate(_impactEffect, transform.position, transform.rotation);
            }
            
            // Destroy projectile
            Destroy(gameObject);
        }
    }
}