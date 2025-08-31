using UnityEngine;
using System.Collections.Generic;

namespace TowerDefense.Towers
{
    /// <summary>
    /// Factory for creating different tower types.
    /// </summary>
    public class TowerFactory : MonoBehaviour
    {
        #region Variables
        [SerializeField] private List<TowerData> availableTowers = new List<TowerData>();
        [SerializeField] private GameObject basicTowerPrefab;
        [SerializeField] private GameObject aoeCannonPrefab;
        [SerializeField] private GameObject sniperTowerPrefab;
        [SerializeField] private GameObject slowTowerPrefab;

        private Dictionary<TowerType, GameObject> towerPrefabs = new Dictionary<TowerType, GameObject>();
        private Dictionary<TowerType, TowerData> towerDataByType = new Dictionary<TowerType, TowerData>();
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeTowerPrefabs();
            LoadTowerData();
        }
        #endregion

        #region Public Methods
        public GameObject CreateTower(TowerType towerType, Vector3 position)
        {
            if (!towerPrefabs.TryGetValue(towerType, out GameObject prefab))
            {
                Debug.LogError($"Tower prefab not found for type: {towerType}");
                return null;
            }

            // Create tower instance
            GameObject towerObject = Instantiate(prefab, position, Quaternion.identity);

            // Initialize tower with data
            Tower tower = towerObject.GetComponent<Tower>();
            if (tower != null && towerDataByType.TryGetValue(towerType, out TowerData towerData))
            {
                tower.Initialize(towerData);
            }

            return towerObject;
        }

        public List<TowerData> GetAvailableTowers()
        {
            return availableTowers;
        }

        public TowerData GetTowerData(TowerType towerType)
        {
            if (towerDataByType.TryGetValue(towerType, out TowerData towerData))
            {
                return towerData;
            }

            return null;
        }
        #endregion

        #region Private Methods
        private void InitializeTowerPrefabs()
        {
            // Map tower types to prefabs
            towerPrefabs[TowerType.Basic] = basicTowerPrefab;
            towerPrefabs[TowerType.AOECannon] = aoeCannonPrefab;
            towerPrefabs[TowerType.Sniper] = sniperTowerPrefab;
            towerPrefabs[TowerType.Slow] = slowTowerPrefab;
        }

        private void LoadTowerData()
        {
            // Load tower data from resources
            TowerData[] towerDatas = Resources.LoadAll<TowerData>("Towers");
            if (towerDatas != null && towerDatas.Length > 0)
            {
                availableTowers.AddRange(towerDatas);

                // Map types to data
                foreach (TowerData data in towerDatas)
                {
                    towerDataByType[data.TowerType] = data;
                }
            }
            else
            {
                // Create default tower data
                CreateDefaultTowerData();
            }
        }

        private void CreateDefaultTowerData()
        {
            // Basic Tower
            TowerData basicTower = ScriptableObject.CreateInstance<TowerData>();
            basicTower.Initialize(TowerType.Basic, "Basic Tower", "Basic defensive tower", 100, 3f, 1f, 10);
            availableTowers.Add(basicTower);
            towerDataByType[TowerType.Basic] = basicTower;

            // AOE Cannon
            TowerData aoeCannon = ScriptableObject.CreateInstance<TowerData>();
            aoeCannon.Initialize(TowerType.AOECannon, "AOE Cannon", "Deals splash damage", 200, 2.5f, 0.5f, 15);
            availableTowers.Add(aoeCannon);
            towerDataByType[TowerType.AOECannon] = aoeCannon;

            // Sniper Tower
            TowerData sniperTower = ScriptableObject.CreateInstance<TowerData>();
            sniperTower.Initialize(TowerType.Sniper, "Sniper Tower", "Long range, high damage", 250, 6f, 0.25f, 40);
            availableTowers.Add(sniperTower);
            towerDataByType[TowerType.Sniper] = sniperTower;

            // Slow Tower
            TowerData slowTower = ScriptableObject.CreateInstance<TowerData>();
            slowTower.Initialize(TowerType.Slow, "Slow Tower", "Slows enemies", 150, 3.5f, 0.75f, 5);
            availableTowers.Add(slowTower);
            towerDataByType[TowerType.Slow] = slowTower;
        }
        #endregion
    }
}