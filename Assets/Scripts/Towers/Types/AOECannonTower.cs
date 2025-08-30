using UnityEngine;

namespace TowerDefense.Towers
{
    public class AOECannonTower : Tower
    {
        [SerializeField] private float _aoeRadius = 2f;

        protected override void Attack()
        {
            if (_currentTarget == null)
                return;

            // Fire AOE projectile
            if (_projectilePrefab != null && _firingPoint != null)
            {
                GameObject projectileObj = Instantiate(_projectilePrefab, _firingPoint.position, _firingPoint.rotation);
                AOEProjectile projectile = projectileObj.GetComponent<AOEProjectile>();

                if (projectile != null)
                {
                    projectile.Initialize(_currentTarget, _damage);
                    projectile.SetAOERadius(_aoeRadius);
                }
                else
                {
                    // Fallback if using base projectile
                    Projectile baseProjectile = projectileObj.GetComponent<Projectile>();
                    if (baseProjectile != null)
                    {
                        baseProjectile.Initialize(_currentTarget, _damage);
                    }
                }
            }

            // Notify listeners
            OnTowerFired?.Invoke(this, _currentTarget);
        }
    }
}