using UnityEngine;

namespace TowerDefense.Towers
{
    /// <summary>
    /// Tower that applies a slow effect to enemies.
    /// </summary>
    public class SlowTower : Tower
    {
        #region Variables
        [Header("Slow Properties")]
        [SerializeField] private float slowEffect = 0.5f;
        [SerializeField] private float slowDuration = 3f;
        #endregion

        #region Protected Methods
        protected override void Attack()
        {
            if (currentTarget == null)
                return;

            if (projectilePrefab != null && firingPoint != null)
            {
                GameObject projectileObj = Instantiate(projectilePrefab, firingPoint.position, firingPoint.rotation);
                SlowProjectile projectile = projectileObj.GetComponent<SlowProjectile>();

                if (projectile != null)
                {
                    projectile.Initialize(currentTarget, towerData.Damage);
                    projectile.SetSlowEffect(slowEffect, slowDuration);
                }
                else
                {
                    Projectile baseProjectile = projectileObj.GetComponent<Projectile>();
                    if (baseProjectile != null)
                    {
                        baseProjectile.Initialize(currentTarget, towerData.Damage);

                        currentTarget.Slow(slowEffect, slowDuration);
                    }
                }
            }

            OnTowerFired?.Invoke(this, currentTarget);
        }
        #endregion
    }
}