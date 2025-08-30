using UnityEngine;

namespace TowerDefense.Towers
{
    /// <summary>
    /// ScriptableObject for tower data
    /// </summary>
    [CreateAssetMenu(fileName = "New Tower", menuName = "Tower Defense/Tower Data")]
    public class TowerData : ScriptableObject
    {
        #region Properties
        [SerializeField] private TowerType _towerType;
        [SerializeField] private string _towerName;
        [SerializeField] private string _description;
        [SerializeField] private int _cost;
        [SerializeField] private float _range;
        [SerializeField] private float _fireRate;
        [SerializeField] private int _damage;
        [SerializeField] private Sprite _icon;
        
        public TowerType TowerType => _towerType;
        public string TowerName => _towerName;
        public string Description => _description;
        public int Cost => _cost;
        public float Range => _range;
        public float FireRate => _fireRate;
        public int Damage => _damage;
        public Sprite Icon => _icon;
        #endregion

        #region Public Methods
        public void Initialize(TowerType type, string name, string description, int cost, float range, float fireRate, int damage)
        {
            _towerType = type;
            _towerName = name;
            _description = description;
            _cost = cost;
            _range = range;
            _fireRate = fireRate;
            _damage = damage;
        }
        #endregion
    }

    /// <summary>
    /// Enum for different tower types
    /// </summary>
    public enum TowerType
    {
        Basic,
        AOECannon,
        Sniper,
        Slow
    }
}