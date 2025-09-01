using UnityEngine;

namespace TowerDefense.Towers
{
    /// <summary>
    /// Tower that deals high damage with a chance for critical hits.
    /// </summary>
    public class SniperTower : Tower
    {
        #region Variables
        [Header("Sniper Properties")]
        [SerializeField] private float criticalHitChance = 0.25f;
        [SerializeField] private float criticalHitMultiplier = 2f;
        #endregion

        #region Protected Methods
        protected override void Attack()
        {
            if (currentTarget == null)
                return;

            int damage = towerData.Damage;

            if (Random.value < criticalHitChance)
            {
                damage = Mathf.RoundToInt(damage * criticalHitMultiplier);
            }

            if (projectilePrefab != null && firingPoint != null)
            {
                GameObject projectileObj = Instantiate(projectilePrefab, firingPoint.position, firingPoint.rotation);
                Projectile projectile = projectileObj.GetComponent<Projectile>();

                if (projectile != null)
                {
                    projectile.Initialize(currentTarget, damage);
                }
            }

            OnTowerFired?.Invoke(this, currentTarget);
        }
        #endregion
    }
}