using UnityEngine;
using System.Collections.Generic;

namespace TowerDefense.Towers
{
    /// <summary>
    /// Factory for creating different tower types
    /// </summary>
    public class TowerFactory : MonoBehaviour
    {
        #region Properties
        [SerializeField] private List<TowerData> _availableTowers = new List<TowerData>();
        [SerializeField] private GameObject _basicTowerPrefab;
        [SerializeField] private GameObject _aoeCannonPrefab;
        [SerializeField] private GameObject _sniperTowerPrefab;
        [SerializeField] private GameObject _slowTowerPrefab;
        
        private Dictionary<TowerType, GameObject> _towerPrefabs = new Dictionary<TowerType, GameObject>();
        private Dictionary<TowerType, TowerData> _towerDataByType = new Dictionary<TowerType, TowerData>();
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
            if (!_towerPrefabs.TryGetValue(towerType, out GameObject prefab))
            {
                Debug.LogError($"Tower prefab not found for type: {towerType}");
                return null;
            }
            
            // Create tower instance
            GameObject towerObject = Instantiate(prefab, position, Quaternion.identity);
            
            // Initialize tower with data
            Tower tower = towerObject.GetComponent<Tower>();
            if (tower != null && _towerDataByType.TryGetValue(towerType, out TowerData towerData))
            {
                tower.Initialize(towerData);
            }
            
            return towerObject;
        }
        
        public List<TowerData> GetAvailableTowers()
        {
            return _availableTowers;
        }
        
        public TowerData GetTowerData(TowerType towerType)
        {
            if (_towerDataByType.TryGetValue(towerType, out TowerData towerData))
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
            _towerPrefabs[TowerType.Basic] = _basicTowerPrefab;
            _towerPrefabs[TowerType.AOECannon] = _aoeCannonPrefab;
            _towerPrefabs[TowerType.Sniper] = _sniperTowerPrefab;
            _towerPrefabs[TowerType.Slow] = _slowTowerPrefab;
        }
        
        private void LoadTowerData()
        {
            // Load tower data from resources
            TowerData[] towerDatas = Resources.LoadAll<TowerData>("Towers");
            if (towerDatas != null && towerDatas.Length > 0)
            {
                _availableTowers.AddRange(towerDatas);
                
                // Map types to data
                foreach (TowerData data in towerDatas)
                {
                    _towerDataByType[data.TowerType] = data;
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
            _availableTowers.Add(basicTower);
            _towerDataByType[TowerType.Basic] = basicTower;
            
            // AOE Cannon
            TowerData aoeCannon = ScriptableObject.CreateInstance<TowerData>();
            aoeCannon.Initialize(TowerType.AOECannon, "AOE Cannon", "Deals splash damage", 200, 2.5f, 0.5f, 15);
            _availableTowers.Add(aoeCannon);
            _towerDataByType[TowerType.AOECannon] = aoeCannon;
            
            // Sniper Tower
            TowerData sniperTower = ScriptableObject.CreateInstance<TowerData>();
            sniperTower.Initialize(TowerType.Sniper, "Sniper Tower", "Long range, high damage", 250, 6f, 0.25f, 40);
            _availableTowers.Add(sniperTower);
            _towerDataByType[TowerType.Sniper] = sniperTower;
            
            // Slow Tower
            TowerData slowTower = ScriptableObject.CreateInstance<TowerData>();
            slowTower.Initialize(TowerType.Slow, "Slow Tower", "Slows enemies", 150, 3.5f, 0.75f, 5);
            _availableTowers.Add(slowTower);
            _towerDataByType[TowerType.Slow] = slowTower;
        }
        #endregion
    }
}