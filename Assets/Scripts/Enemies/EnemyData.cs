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
        [SerializeField] private EnemyType enemyType;
        [SerializeField] private string enemyName;
        [SerializeField] private int health = 100;
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private int damage = 1;
        [SerializeField] private int scoreValue = 10;
        [SerializeField] private int currencyReward = 20;
        [SerializeField] private bool canAttackTowers = false;
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float attackRate = 1f;
        [SerializeField] private float towerStunDuration = 3f;
        [SerializeField] private Sprite icon;
        #endregion

        #region Properties
        public EnemyType EnemyType => enemyType;
        public string EnemyName => enemyName;
        public int Health => health;
        public float MoveSpeed => moveSpeed;
        public int Damage => damage;
        public int CurrencyReward => currencyReward;
        public bool CanAttackTowers => canAttackTowers;
        public float AttackRange => attackRange;
        public float AttackRate => attackRate;
        public float TowerStunDuration => towerStunDuration;
        public Sprite Icon => icon;
        #endregion

        #region Public Methods
        public void Initialize(
            EnemyType type,
            string name,
            int health,
            float moveSpeed,
            int damage,
            int reward,
            bool canAttackTowers = false)
        {
            enemyType = type;
            enemyName = name;
            this.health = health;
            this.moveSpeed = moveSpeed;
            this.damage = damage;
            currencyReward = reward;
            this.canAttackTowers = canAttackTowers;
        }
        #endregion
    }

    /// <summary>
    /// Enum for different enemy types.
    /// </summary>
    public enum EnemyType
    {
        Basic,
        Fast,
        Tank,
        TowerAttacker
    }
}