using UnityEngine;

namespace TowerDefense.Towers
{
    public class SlowProjectile : Projectile
    {
        #region Variables
        [SerializeField] private float slowEffect = 0.5f;
        [SerializeField] private float slowDuration = 3f;
        #endregion

        #region Public Methods
        public void SetSlowEffect(float effect, float duration)
        {
            slowEffect = Mathf.Clamp01(effect);
            slowDuration = duration;
        }
        #endregion

        #region Protected Methods
        protected override void OnHit()
        {
            target.TakeDamage(damage);

            target.Slow(slowEffect, slowDuration);

            if (impactEffect != null)
            {
                Instantiate(impactEffect, transform.position, transform.rotation);
            }

            Destroy(gameObject);
        }
        #endregion
    }
}