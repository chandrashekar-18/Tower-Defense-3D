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
        [SerializeField] private TowerType towerType;
        [SerializeField] private string towerName;
        [SerializeField] private string description;
        [SerializeField] private int cost;
        [SerializeField] private float range;
        [SerializeField] private float fireRate;
        [SerializeField] private int damage;
        [SerializeField] private Sprite icon;
        #endregion

        #region Properties
        public TowerType TowerType => towerType;
        public string TowerName => towerName;
        public string Description => description;
        public int Cost => cost;
        public float Range => range;
        public float FireRate => fireRate;
        public int Damage => damage;
        public Sprite Icon => icon;
        #endregion

        #region Public Methods
        public void Initialize(TowerType type, string name, string description, int cost, float range, float fireRate, int damage)
        {
            towerType = type;
            towerName = name;
            this.description = description;
            this.cost = cost;
            this.range = range;
            this.fireRate = fireRate;
            this.damage = damage;
        }
        #endregion
    }

    /// <summary>
    /// Enum for different tower types.
    /// </summary>
    public enum TowerType
    {
        Basic,
        AOECannon,
        Sniper,
        Slow
    }
}