using UnityEngine;

namespace TowerDefense.Enemies
{
    /// <summary>
    /// Tank enemy with high health and damage resistance.
    /// Example of how to modify damage processing.
    /// </summary>
    public class TankEnemy : Enemy
    {
        #region Tank Enemy Variables
        [Header("Tank Enemy Properties")]
        [SerializeField] private float damageResistance = 0.25f;
        [SerializeField] private bool explodeOnDeath = true;
        [SerializeField] private float explosionRadius = 2f;
        [SerializeField] private int explosionDamage = 50;
        #endregion
        
        #region Overridden Methods
        protected override int ProcessIncomingDamage(int damage)
        {
            int reducedDamage = Mathf.RoundToInt(damage * (1f - damageResistance));
            return Mathf.Max(1, reducedDamage);
        }
        
        protected override void OnEnemyDeath()
        {
            base.OnEnemyDeath();
            
            if (explodeOnDeath)
            {
                CreateExplosionEffect();
                
                DamageNearbyEnemies();
            }
        }
        #endregion
        
        #region Private Methods
        private void CreateExplosionEffect()
        {
            // You would instantiate explosion particle effect here
            Debug.Log($"Tank enemy exploded at {transform.position}!");
        }
        
        private void DamageNearbyEnemies()
        {
            Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, explosionRadius);
            
            foreach (Collider collider in nearbyColliders)
            {
                Enemy nearbyEnemy = collider.GetComponent<Enemy>();
                if (nearbyEnemy != null && nearbyEnemy != this && nearbyEnemy.IsAlive)
                {
                    nearbyEnemy.TakeDamage(explosionDamage);
                }
            }
        }
        #endregion
    }
}