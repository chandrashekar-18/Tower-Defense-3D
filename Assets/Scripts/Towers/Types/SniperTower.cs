using UnityEngine;

namespace TowerDefense.Towers
{
    public class SniperTower : Tower
    {
        [SerializeField] private float _criticalHitChance = 0.25f;
        [SerializeField] private float _criticalHitMultiplier = 2f;

        protected override void Attack()
        {
            if (_currentTarget == null)
                return;

            // Calculate damage
            int damage = _damage;

            // Check for critical hit
            if (Random.value < _criticalHitChance)
            {
                damage = Mathf.RoundToInt(damage * _criticalHitMultiplier);
            }

            // Fire sniper projectile
            if (_projectilePrefab != null && _firingPoint != null)
            {
                GameObject projectileObj = Instantiate(_projectilePrefab, _firingPoint.position, _firingPoint.rotation);
                Projectile projectile = projectileObj.GetComponent<Projectile>();

                if (projectile != null)
                {
                    projectile.Initialize(_currentTarget, damage);
                }
            }

            // Notify listeners
            OnTowerFired?.Invoke(this, _currentTarget);
        }
    }
}