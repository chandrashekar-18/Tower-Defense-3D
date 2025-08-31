using UnityEngine;

namespace TowerDefense.Enemies
{
    /// <summary>
    /// Tank enemy with high health but slow movement
    /// </summary>
    public class TankEnemy : Enemy
    {
        [SerializeField] private float damageResistance = 0.25f; // 25% damage reduction
        
        public override void TakeDamage(int damage)
        {
            int reducedDamage = Mathf.RoundToInt(damage * (1f - damageResistance));
            base.TakeDamage(reducedDamage);
        }
        
        protected override void ExecuteBehavior()
        {
            // Tank has no special active behavior, just damage resistance
        }
        
        protected override void InitializeFromData()
        {
            base.InitializeFromData();
            
            if (enemyData != null)
            {
                // Tank typically has higher health, more damage, but slower speed
                // Could apply additional modifiers here if needed
            }
        }
        
        protected override void Die()
        {
            // Tank explosion with area damage upon death
            Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, 0f); // No self-damage
            
            base.Die();
        }
    }
}