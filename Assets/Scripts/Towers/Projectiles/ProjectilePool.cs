using UnityEngine;
using TowerDefense.Utils;

namespace TowerDefense.Towers
{
    /// <summary>
    /// Object pool for projectiles.
    /// </summary>
    public class ProjectilePool : MonoBehaviour
    {
        #region Singleton
        public static ProjectilePool Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
        #endregion

        #region Variables
        [SerializeField] private Projectile basicProjectilePrefab;
        [SerializeField] private Projectile aoeProjectilePrefab;
        [SerializeField] private Projectile sniperProjectilePrefab;
        [SerializeField] private Projectile slowProjectilePrefab;
        [SerializeField] private int initialPoolSize = 20;

        private ObjectPool<Projectile> basicProjectilePool;
        private ObjectPool<Projectile> aoeProjectilePool;
        private ObjectPool<Projectile> sniperProjectilePool;
        private ObjectPool<Projectile> slowProjectilePool;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            Transform container = new GameObject("ProjectilePoolContainer").transform;
            container.SetParent(transform);

            basicProjectilePool = new ObjectPool<Projectile>(basicProjectilePrefab, initialPoolSize, container);
            aoeProjectilePool = new ObjectPool<Projectile>(aoeProjectilePrefab, initialPoolSize, container);
            sniperProjectilePool = new ObjectPool<Projectile>(sniperProjectilePrefab, initialPoolSize, container);
            slowProjectilePool = new ObjectPool<Projectile>(slowProjectilePrefab, initialPoolSize, container);
        }
        #endregion

        #region Public Methods
        public Projectile GetProjectile(TowerType towerType)
        {
            switch (towerType)
            {
                case TowerType.AOECannon:
                    return aoeProjectilePool.Get();
                case TowerType.Sniper:
                    return sniperProjectilePool.Get();
                case TowerType.Slow:
                    return slowProjectilePool.Get();
                case TowerType.Basic:
                default:
                    return basicProjectilePool.Get();
            }
        }

        public void ReturnProjectile(Projectile projectile, TowerType towerType)
        {
            switch (towerType)
            {
                case TowerType.AOECannon:
                    aoeProjectilePool.Return(projectile);
                    break;
                case TowerType.Sniper:
                    sniperProjectilePool.Return(projectile);
                    break;
                case TowerType.Slow:
                    slowProjectilePool.Return(projectile);
                    break;
                case TowerType.Basic:
                default:
                    basicProjectilePool.Return(projectile);
                    break;
            }
        }
        #endregion
    }
}