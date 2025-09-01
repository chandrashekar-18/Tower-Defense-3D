using UnityEngine;
using TowerDefense.Utils;
using System.Collections.Generic;

namespace TowerDefense.Towers
{
    /// <summary>
    /// Object pool for projectiles with automatic type detection.
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
        [SerializeField] private int initialPoolSize = 20;
        
        private Dictionary<string, ObjectPool<Projectile>> projectilePools = new Dictionary<string, ObjectPool<Projectile>>();
        private Transform poolContainer;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            poolContainer = new GameObject("ProjectilePoolContainer").transform;
            poolContainer.SetParent(transform);
        }
        #endregion

        #region Public Methods
        public Projectile GetProjectile(GameObject projectilePrefab)
        {
            if (projectilePrefab == null)
                return null;

            string prefabName = projectilePrefab.name;
            
            // Create pool if it doesn't exist
            if (!projectilePools.ContainsKey(prefabName))
            {
                Projectile projectileComponent = projectilePrefab.GetComponent<Projectile>();
                if (projectileComponent != null)
                {
                    projectilePools[prefabName] = new ObjectPool<Projectile>(projectileComponent, initialPoolSize, poolContainer);
                }
                else
                {
                    Debug.LogError($"Projectile prefab {prefabName} doesn't have a Projectile component");
                    return null;
                }
            }

            return projectilePools[prefabName].Get();
        }

        public void ReturnProjectile(Projectile projectile)
        {
            if (projectile == null)
                return;

            string prefabName = projectile.name.Replace("(Clone)", "");
            
            if (projectilePools.ContainsKey(prefabName))
            {
                projectilePools[prefabName].Return(projectile);
            }
            else
            {
                // If pool doesn't exist, just destroy the projectile
                Destroy(projectile.gameObject);
            }
        }
        #endregion
    }
}