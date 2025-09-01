using UnityEngine;
using System.Collections;

namespace TowerDefense.Enemies
{
    /// <summary>
    /// Fast enemy that moves quickly and has speed boost ability.
    /// Example of how to add special behaviors to enemies.
    /// </summary>
    public class FastEnemy : Enemy
    {
        #region Fast Enemy Variables
        [Header("Fast Enemy Properties")]
        [SerializeField] private float speedBoostInterval = 5f;
        [SerializeField] private float speedBoostDuration = 2f;
        [SerializeField] private float speedBoostMultiplier = 1.5f;

        private float speedBoostTimer;
        private bool boosting = false;
        #endregion

        #region Overridden Methods
        protected override void OnEnemyInitialized()
        {
            base.OnEnemyInitialized();
            speedBoostTimer = speedBoostInterval;
        }

        protected override void ExecuteBehavior()
        {
            base.ExecuteBehavior();

            // Handle speed boost behavior
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
        #endregion

        #region Private Methods
        private IEnumerator ApplySpeedBoost()
        {
            boosting = true;

            float originalSpeed = enemyData.MoveSpeed;
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
        #endregion
    }
}