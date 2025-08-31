using UnityEngine;
using System.Collections.Generic;

namespace TowerDefense.Enemies
{
    /// <summary>
    /// Factory for creating different enemy types.
    /// </summary>
    public class EnemyFactory : MonoBehaviour
    {
        #region Variables
        [SerializeField] private GameObject basicEnemyPrefab;
        [SerializeField] private GameObject fastEnemyPrefab;
        [SerializeField] private GameObject tankEnemyPrefab;
        [SerializeField] private GameObject towerAttackerPrefab;

        private Dictionary<EnemyType, GameObject> enemyPrefabs = new Dictionary<EnemyType, GameObject>();
        private Dictionary<EnemyType, EnemyData> enemyDataByType = new Dictionary<EnemyType, EnemyData>();

        private Transform enemyContainer;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeEnemyPrefabs();
            LoadEnemyData();

            // Create container for enemies
            GameObject container = new GameObject("EnemyContainer");
            enemyContainer = container.transform;
        }
        #endregion

        #region Public Methods
        public Enemy CreateEnemy(EnemyType enemyType, Vector3 position, List<Vector3> path)
        {
            if (!enemyPrefabs.TryGetValue(enemyType, out GameObject prefab))
            {
                Debug.LogError($"Enemy prefab not found for type: {enemyType}");
                return null;
            }

            // Create enemy instance
            GameObject enemyObject = Instantiate(prefab, position, Quaternion.identity, enemyContainer);

            // Initialize enemy with data and path
            Enemy enemy = enemyObject.GetComponent<Enemy>();
            if (enemy != null && enemyDataByType.TryGetValue(enemyType, out EnemyData enemyData))
            {
                enemy.Initialize(enemyData, path);
            }

            return enemy;
        }

        public EnemyData GetEnemyData(EnemyType enemyType)
        {
            if (enemyDataByType.TryGetValue(enemyType, out EnemyData enemyData))
            {
                return enemyData;
            }

            return null;
        }
        #endregion

        #region Private Methods
        private void InitializeEnemyPrefabs()
        {
            // Map enemy types to prefabs
            enemyPrefabs[EnemyType.Basic] = basicEnemyPrefab;
            enemyPrefabs[EnemyType.Fast] = fastEnemyPrefab;
            enemyPrefabs[EnemyType.Tank] = tankEnemyPrefab;
            enemyPrefabs[EnemyType.TowerAttacker] = towerAttackerPrefab;
        }

        private void LoadEnemyData()
        {
            // Load enemy data from resources
            EnemyData[] enemyDatas = Resources.LoadAll<EnemyData>("Enemies");
            if (enemyDatas != null && enemyDatas.Length > 0)
            {
                // Map types to data
                foreach (EnemyData data in enemyDatas)
                {
                    enemyDataByType[data.EnemyType] = data;
                }
            }
            else
            {
                // Create default enemy data
                CreateDefaultEnemyData();
            }
        }

        private void CreateDefaultEnemyData()
        {
            // Basic Enemy
            EnemyData basicEnemy = ScriptableObject.CreateInstance<EnemyData>();
            basicEnemy.Initialize(EnemyType.Basic, "Basic Enemy", 100, 3f, 1, 20);
            enemyDataByType[EnemyType.Basic] = basicEnemy;

            // Fast Enemy
            EnemyData fastEnemy = ScriptableObject.CreateInstance<EnemyData>();
            fastEnemy.Initialize(EnemyType.Fast, "Fast Enemy", 50, 6f, 1, 30);
            enemyDataByType[EnemyType.Fast] = fastEnemy;

            // Tank Enemy
            EnemyData tankEnemy = ScriptableObject.CreateInstance<EnemyData>();
            tankEnemy.Initialize(EnemyType.Tank, "Tank Enemy", 300, 1.5f, 2, 50);
            enemyDataByType[EnemyType.Tank] = tankEnemy;

            // Tower Attacker
            EnemyData towerAttacker = ScriptableObject.CreateInstance<EnemyData>();
            towerAttacker.Initialize(EnemyType.TowerAttacker, "Tower Attacker", 150, 2.5f, 1, 40, true);
            enemyDataByType[EnemyType.TowerAttacker] = towerAttacker;
        }
        #endregion
    }
}