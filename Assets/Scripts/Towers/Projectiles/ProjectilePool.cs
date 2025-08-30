using UnityEngine;
using TowerDefense.Utils;

namespace TowerDefense.Towers
{
    /// <summary>
    /// Object pool for projectiles
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

        #region Properties
        [SerializeField] private Projectile _basicProjectilePrefab;
        [SerializeField] private Projectile _aoeProjectilePrefab;
        [SerializeField] private Projectile _sniperProjectilePrefab;
        [SerializeField] private Projectile _slowProjectilePrefab;

        [SerializeField] private int _initialPoolSize = 20;

        private ObjectPool<Projectile> _basicProjectilePool;
        private ObjectPool<Projectile> _aoeProjectilePool;
        private ObjectPool<Projectile> _sniperProjectilePool;
        private ObjectPool<Projectile> _slowProjectilePool;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            // Initialize pools
            Transform container = new GameObject("ProjectilePoolContainer").transform;
            container.SetParent(transform);

            _basicProjectilePool = new ObjectPool<Projectile>(_basicProjectilePrefab, _initialPoolSize, container);
            _aoeProjectilePool = new ObjectPool<Projectile>(_aoeProjectilePrefab, _initialPoolSize, container);
            _sniperProjectilePool = new ObjectPool<Projectile>(_sniperProjectilePrefab, _initialPoolSize, container);
            _slowProjectilePool = new ObjectPool<Projectile>(_slowProjectilePrefab, _initialPoolSize, container);
        }
        #endregion

        #region Public Methods
        public Projectile GetProjectile(TowerType towerType)
        {
            switch (towerType)
            {
                case TowerType.AOECannon:
                    return _aoeProjectilePool.Get();
                case TowerType.Sniper:
                    return _sniperProjectilePool.Get();
                case TowerType.Slow:
                    return _slowProjectilePool.Get();
                case TowerType.Basic:
                default:
                    return _basicProjectilePool.Get();
            }
        }

        public void ReturnProjectile(Projectile projectile, TowerType towerType)
        {
            switch (towerType)
            {
                case TowerType.AOECannon:
                    _aoeProjectilePool.Return(projectile);
                    break;
                case TowerType.Sniper:
                    _sniperProjectilePool.Return(projectile);
                    break;
                case TowerType.Slow:
                    _slowProjectilePool.Return(projectile);
                    break;
                case TowerType.Basic:
                default:
                    _basicProjectilePool.Return(projectile);
                    break;
            }
        }
        #endregion
    }
}