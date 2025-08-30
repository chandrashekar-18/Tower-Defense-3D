using UnityEngine;
using TowerDefense.Enemies;

namespace TowerDefense.Towers
{
    public class SlowTower : Tower
    {
        [SerializeField] private float _slowEffect = 0.5f;
        [SerializeField] private float _slowDuration = 3f;
        
        protected override void Attack()
        {
            if (_currentTarget == null)
                return;
                
            // Fire slow projectile
            if (_projectilePrefab != null && _firingPoint != null)
            {
                GameObject projectileObj = Instantiate(_projectilePrefab, _firingPoint.position, _firingPoint.rotation);
                SlowProjectile projectile = projectileObj.GetComponent<SlowProjectile>();
                
                if (projectile != null)
                {
                    projectile.Initialize(_currentTarget, _damage);
                    projectile.SetSlowEffect(_slowEffect, _slowDuration);
                }
                else
                {
                    // Fallback to regular projectile
                    Projectile baseProjectile = projectileObj.GetComponent<Projectile>();
                    if (baseProjectile != null)
                    {
                        baseProjectile.Initialize(_currentTarget, _damage);
                        
                        // Apply slow effect directly to target
                        _currentTarget.Slow(_slowEffect, _slowDuration);
                    }
                }
            }
            
            // Notify listeners
            OnTowerFired?.Invoke(this, _currentTarget);
        }
    }
}