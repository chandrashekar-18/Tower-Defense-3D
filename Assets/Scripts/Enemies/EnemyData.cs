using UnityEngine;

namespace TowerDefense.Enemies
{
    /// <summary>
    /// ScriptableObject for enemy data.
    /// </summary>
    [CreateAssetMenu(fileName = "New Enemy", menuName = "Tower Defense/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        #region Variables
        [Header("Identity")]
        [SerializeField] private string enemyId; // Used instead of enum
        [SerializeField] private string enemyName;
        [SerializeField] private GameObject enemyPrefab; // Direct reference to prefab
        
        [Header("Stats")]
        [SerializeField] private int health = 100;
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private int damage = 1;
        [SerializeField] private int scoreValue = 10;
        [SerializeField] private int currencyReward = 20;
        
        [Header("Combat")]
        [SerializeField] private bool canAttackTowers = false;
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float attackRate = 1f;
        [SerializeField] private float towerStunDuration = 3f;
        
        [Header("Visual")]
        [SerializeField] private Sprite icon;
        #endregion

        #region Properties
        public string EnemyId => enemyId;
        public string EnemyName => enemyName;
        public GameObject EnemyPrefab => enemyPrefab;
        public int Health => health;
        public float MoveSpeed => moveSpeed;
        public int Damage => damage;
        public int ScoreValue => scoreValue;
        public int CurrencyReward => currencyReward;
        public bool CanAttackTowers => canAttackTowers;
        public float AttackRange => attackRange;
        public float AttackRate => attackRate;
        public float TowerStunDuration => towerStunDuration;
        public Sprite Icon => icon;
        #endregion

        #region Editor Validation
        private void OnValidate()
        {
            // Auto-generate ID from name if empty
            if (string.IsNullOrEmpty(enemyId) && !string.IsNullOrEmpty(enemyName))
            {
                enemyId = enemyName.ToLower().Replace(" ", "_");
            }
        }
        #endregion
    }
}