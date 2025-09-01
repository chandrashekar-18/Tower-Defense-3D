using UnityEngine;

namespace TowerDefense.Towers
{
    /// <summary>
    /// Area-of-effect cannon tower.
    /// </summary>
    public class AOECannonTower : Tower
    {
        #region Variables
        [Header("AOE Cannon Properties")]
        [SerializeField] private float aoeRadius = 2f;
        #endregion

        #region Protected Methods
        protected override void Attack()
        {
            if (currentTarget == null)
                return;

            if (projectilePrefab != null && firingPoint != null)
            {
                GameObject projectileObj = Instantiate(projectilePrefab, firingPoint.position, firingPoint.rotation);
                AOEProjectile projectile = projectileObj.GetComponent<AOEProjectile>();

                if (projectile != null)
                {
                    projectile.Initialize(currentTarget, towerData.Damage);
                    projectile.SetAOERadius(aoeRadius);
                }
                else
                {
                    Projectile baseProjectile = projectileObj.GetComponent<Projectile>();
                    if (baseProjectile != null)
                    {
                        baseProjectile.Initialize(currentTarget, towerData.Damage);
                    }
                }
            }

            // Notify listeners
            OnTowerFired?.Invoke(this, currentTarget);
        }
        #endregion
    }
}