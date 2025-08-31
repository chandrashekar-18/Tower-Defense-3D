using UnityEngine;
using System.Collections;

namespace TowerDefense.Enemies
{
    /// <summary>
    /// Fast enemy that moves quickly but has less health
    /// </summary>
    public class FastEnemy : Enemy
    {
        [SerializeField] private float speedBoostInterval = 5f;
        [SerializeField] private float speedBoostDuration = 2f;
        [SerializeField] private float speedBoostMultiplier = 1.5f;
        
        private float speedBoostTimer;
        private bool boosting = false;
        
        protected override void Start()
        {
            base.Start();
            speedBoostTimer = speedBoostInterval;
        }
        
        protected override void ExecuteBehavior()
        {
            if (!boosting)
            {
                speedBoostTimer -= Time.deltaTime;
                if (speedBoostTimer <= 0f)
                {
                    StartCoroutine(ApplySpeedBoost());
                    speedBoostTimer = speedBoostInterval;
                }
            }
        }
        
        private IEnumerator ApplySpeedBoost()
        {
            boosting = true;
            
            float originalSpeed = moveSpeed;
            moveSpeed *= speedBoostMultiplier;
            
            if (animator != null)
            {
                animator.SetBool("SpeedBoost", true);
            }
            
            yield return new WaitForSeconds(speedBoostDuration);
            
            moveSpeed = originalSpeed;
            if (animator != null)
            {
                animator.SetBool("SpeedBoost", false);
            }
            
            boosting = false;
        }
    }
}