using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TowerDefense.Towers
{
    /// <summary>
    /// Factory for creating towers from TowerData ScriptableObjects.
    /// </summary>
    public class TowerFactory : MonoBehaviour
    {
        #region Variables
        [SerializeField] private List<TowerData> availableTowers = new List<TowerData>();
        
        private Dictionary<string, TowerData> towerDataByID = new Dictionary<string, TowerData>();
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            LoadTowerData();
        }
        #endregion

        #region Public Methods
        public GameObject CreateTower(string towerID, Vector3 position)
        {
            if (!towerDataByID.TryGetValue(towerID, out TowerData towerData))
            {
                Debug.LogError($"Tower data not found for ID: {towerID}");
                return null;
            }

            if (towerData.TowerPrefab == null)
            {
                Debug.LogError($"Tower prefab is null for ID: {towerID}");
                return null;
            }

            // Create tower instance
            GameObject towerObject = Instantiate(towerData.TowerPrefab, position, Quaternion.identity);

            // Initialize tower with data
            Tower tower = towerObject.GetComponent<Tower>();
            if (tower != null)
            {
                tower.Initialize(towerData);
            }

            return towerObject;
        }

        public GameObject CreateTower(TowerData towerData, Vector3 position)
        {
            if (towerData == null || towerData.TowerPrefab == null)
            {
                Debug.LogError("Invalid tower data or prefab");
                return null;
            }

            // Create tower instance
            GameObject towerObject = Instantiate(towerData.TowerPrefab, position, Quaternion.identity);

            // Initialize tower with data
            Tower tower = towerObject.GetComponent<Tower>();
            if (tower != null)
            {
                tower.Initialize(towerData);
            }

            return towerObject;
        }

        public List<TowerData> GetAvailableTowers()
        {
            return availableTowers.ToList(); // Return copy to prevent external modification
        }

        public TowerData GetTowerData(string towerID)
        {
            towerDataByID.TryGetValue(towerID, out TowerData towerData);
            return towerData;
        }

        public void RegisterTowerData(TowerData towerData)
        {
            if (towerData != null && !string.IsNullOrEmpty(towerData.TowerID))
            {
                if (!towerDataByID.ContainsKey(towerData.TowerID))
                {
                    availableTowers.Add(towerData);
                    towerDataByID[towerData.TowerID] = towerData;
                }
            }
        }
        #endregion

        #region Private Methods
        private void LoadTowerData()
        {
            // Clear existing data
            availableTowers.Clear();
            towerDataByID.Clear();

            // Load tower data from resources
            TowerData[] towerDatas = Resources.LoadAll<TowerData>("Towers");
            
            foreach (TowerData data in towerDatas)
            {
                if (data != null && !string.IsNullOrEmpty(data.TowerID))
                {
                    availableTowers.Add(data);
                    towerDataByID[data.TowerID] = data;
                }
            }

            Debug.Log($"Loaded {availableTowers.Count} tower types");
        }
        #endregion
    }
}
