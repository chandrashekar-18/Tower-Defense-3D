using UnityEngine;
using System.Collections.Generic;

namespace TowerDefense.Enemies
{
    /// <summary>
    /// Factory for creating different enemy types
    /// </summary>
    public class EnemyFactory : MonoBehaviour
    {
        #region Properties
        [SerializeField] private GameObject _basicEnemyPrefab;
        [SerializeField] private GameObject _fastEnemyPrefab;
        [SerializeField] private GameObject _tankEnemyPrefab;
        [SerializeField] private GameObject _towerAttackerPrefab;

        private Dictionary<EnemyType, GameObject> _enemyPrefabs = new Dictionary<EnemyType, GameObject>();
        private Dictionary<EnemyType, EnemyData> _enemyDataByType = new Dictionary<EnemyType, EnemyData>();

        private Transform _enemyContainer;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeEnemyPrefabs();
            LoadEnemyData();

            // Create container for enemies
            GameObject container = new GameObject("EnemyContainer");
            _enemyContainer = container.transform;
        }
        #endregion

        #region Public Methods
        public EnemyController CreateEnemy(EnemyType enemyType, Vector3 position, List<Vector3> path)
        {
            if (!_enemyPrefabs.TryGetValue(enemyType, out GameObject prefab))
            {
                Debug.LogError($"Enemy prefab not found for type: {enemyType}");
                return null;
            }

            // Create enemy instance
            GameObject enemyObject = Instantiate(prefab, position, Quaternion.identity, _enemyContainer);

            // Initialize enemy with data and path
            EnemyController enemy = enemyObject.GetComponent<EnemyController>();
            if (enemy != null && _enemyDataByType.TryGetValue(enemyType, out EnemyData enemyData))
            {
                enemy.Initialize(enemyData, path);
            }

            return enemy;
        }

        public EnemyData GetEnemyData(EnemyType enemyType)
        {
            if (_enemyDataByType.TryGetValue(enemyType, out EnemyData enemyData))
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
            _enemyPrefabs[EnemyType.Basic] = _basicEnemyPrefab;
            _enemyPrefabs[EnemyType.Fast] = _fastEnemyPrefab;
            _enemyPrefabs[EnemyType.Tank] = _tankEnemyPrefab;
            _enemyPrefabs[EnemyType.TowerAttacker] = _towerAttackerPrefab;
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
                    _enemyDataByType[data.EnemyType] = data;
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
            _enemyDataByType[EnemyType.Basic] = basicEnemy;

            // Fast Enemy
            EnemyData fastEnemy = ScriptableObject.CreateInstance<EnemyData>();
            fastEnemy.Initialize(EnemyType.Fast, "Fast Enemy", 50, 6f, 1, 30);
            _enemyDataByType[EnemyType.Fast] = fastEnemy;

            // Tank Enemy
            EnemyData tankEnemy = ScriptableObject.CreateInstance<EnemyData>();
            tankEnemy.Initialize(EnemyType.Tank, "Tank Enemy", 300, 1.5f, 2, 50);
            _enemyDataByType[EnemyType.Tank] = tankEnemy;

            // Tower Attacker
            EnemyData towerAttacker = ScriptableObject.CreateInstance<EnemyData>();
            towerAttacker.Initialize(EnemyType.TowerAttacker, "Tower Attacker", 150, 2.5f, 1, 40, true);
            _enemyDataByType[EnemyType.TowerAttacker] = towerAttacker;
        }
        #endregion
    }
}