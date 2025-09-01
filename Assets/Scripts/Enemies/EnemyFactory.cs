using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TowerDefense.Enemies
{
    /// <summary>
    /// Factory for creating enemies based on EnemyData.
    /// </summary>
    public class EnemyFactory : MonoBehaviour
    {
        #region Variables
        [SerializeField] private List<EnemyData> availableEnemies = new List<EnemyData>();
        
        private Dictionary<string, EnemyData> enemyDatabase = new Dictionary<string, EnemyData>();
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            BuildEnemyDatabase();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Create an enemy by ID
        /// </summary>
        public Enemy CreateEnemy(string enemyId, Vector3 spawnPosition, List<Vector3> path)
        {
            if (!enemyDatabase.ContainsKey(enemyId))
            {
                Debug.LogError($"Enemy with ID '{enemyId}' not found in database!");
                return null;
            }

            EnemyData enemyData = enemyDatabase[enemyId];
            
            if (enemyData.EnemyPrefab == null)
            {
                Debug.LogError($"No prefab assigned to enemy '{enemyId}'!");
                return null;
            }

            // Instantiate the prefab
            GameObject enemyObject = Instantiate(enemyData.EnemyPrefab, spawnPosition, Quaternion.identity);
            
            // Get the Enemy component
            Enemy enemy = enemyObject.GetComponent<Enemy>();
            if (enemy == null)
            {
                Debug.LogError($"Enemy prefab '{enemyId}' doesn't have an Enemy component!");
                Destroy(enemyObject);
                return null;
            }

            // Initialize the enemy
            enemy.Initialize(enemyData, path);

            return enemy;
        }

        /// <summary>
        /// Create an enemy using EnemyData directly
        /// </summary>
        public Enemy CreateEnemy(EnemyData enemyData, Vector3 spawnPosition, List<Vector3> path)
        {
            if (enemyData == null || enemyData.EnemyPrefab == null)
            {
                Debug.LogError("Invalid enemy data or missing prefab!");
                return null;
            }

            GameObject enemyObject = Instantiate(enemyData.EnemyPrefab, spawnPosition, Quaternion.identity);
            Enemy enemy = enemyObject.GetComponent<Enemy>();
            
            if (enemy == null)
            {
                Debug.LogError($"Enemy prefab '{enemyData.EnemyName}' doesn't have an Enemy component!");
                Destroy(enemyObject);
                return null;
            }

            enemy.Initialize(enemyData, path);
            return enemy;
        }

        /// <summary>
        /// Get all available enemy IDs
        /// </summary>
        public string[] GetAvailableEnemyIds()
        {
            return enemyDatabase.Keys.ToArray();
        }

        /// <summary>
        /// Get enemy data by ID
        /// </summary>
        public EnemyData GetEnemyData(string enemyId)
        {
            return enemyDatabase.ContainsKey(enemyId) ? enemyDatabase[enemyId] : null;
        }

        /// <summary>
        /// Add new enemy data at runtime (useful for modding or DLC)
        /// </summary>
        public void RegisterEnemyData(EnemyData enemyData)
        {
            if (enemyData != null && !string.IsNullOrEmpty(enemyData.EnemyId))
            {
                enemyDatabase[enemyData.EnemyId] = enemyData;
                if (!availableEnemies.Contains(enemyData))
                {
                    availableEnemies.Add(enemyData);
                }
            }
        }
        #endregion

        #region Private Methods
        private void BuildEnemyDatabase()
        {
            enemyDatabase.Clear();
            
            foreach (EnemyData enemyData in availableEnemies)
            {
                if (enemyData != null && !string.IsNullOrEmpty(enemyData.EnemyId))
                {
                    if (enemyDatabase.ContainsKey(enemyData.EnemyId))
                    {
                        Debug.LogWarning($"Duplicate enemy ID '{enemyData.EnemyId}' found! Skipping...");
                        continue;
                    }
                    
                    enemyDatabase[enemyData.EnemyId] = enemyData;
                }
            }

            Debug.Log($"Enemy database built with {enemyDatabase.Count} enemy types.");
        }
        #endregion

        #region Editor Methods
#if UNITY_EDITOR
        [ContextMenu("Refresh Enemy Database")]
        private void RefreshDatabase()
        {
            BuildEnemyDatabase();
        }
#endif
        #endregion
    }
}