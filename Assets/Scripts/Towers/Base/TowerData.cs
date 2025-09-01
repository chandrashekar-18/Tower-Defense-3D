using UnityEngine;

namespace TowerDefense.Towers
{
    /// <summary>
    /// ScriptableObject for tower data.
    /// </summary>
    [CreateAssetMenu(fileName = "New Tower", menuName = "Tower Defense/Tower Data")]
    public class TowerData : ScriptableObject
    {
        #region Variables
        [Header("Identity")]
        [SerializeField] private string towerID;
        [SerializeField] private string towerName;
        [SerializeField] private string description;
        [SerializeField] private Sprite icon;
        [SerializeField] private GameObject towerPrefab;
        [Header("Health Settings")]
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private bool canBeRepaired = true;
        [SerializeField] private int repairCost = 25;
        [Header("Combat Settings")]
        [SerializeField] private int cost;
        [SerializeField] private float range;
        [SerializeField] private float fireRate;
        [SerializeField] private int damage;
        #endregion

        #region Properties
        public string TowerID => towerID;
        public string TowerName => towerName;
        public string Description => description;
        public int MaxHealth => maxHealth;
        public bool CanBeRepaired => canBeRepaired;
        public int RepairCost => repairCost;
        public int Cost => cost;
        public float Range => range;
        public float FireRate => fireRate;
        public int Damage => damage;
        public Sprite Icon => icon;
        public GameObject TowerPrefab => towerPrefab;
        #endregion
    }
}