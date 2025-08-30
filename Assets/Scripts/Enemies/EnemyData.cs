using UnityEngine;

namespace TowerDefense.Enemies
{
    /// <summary>
    /// ScriptableObject for enemy data
    /// </summary>
    [CreateAssetMenu(fileName = "New Enemy", menuName = "Tower Defense/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        #region Properties
        [SerializeField] private EnemyType _enemyType;
        [SerializeField] private string _enemyName;
        [SerializeField] private int _health = 100;
        [SerializeField] private float _moveSpeed = 3f;
        [SerializeField] private int _damage = 1;
        [SerializeField] private int _currencyReward = 20;
        [SerializeField] private bool _canAttackTowers = false;
        [SerializeField] private float _attackRange = 2f;
        [SerializeField] private float _attackRate = 1f;
        [SerializeField] private float _towerStunDuration = 3f;
        [SerializeField] private Sprite _icon;

        public EnemyType EnemyType => _enemyType;
        public string EnemyName => _enemyName;
        public int Health => _health;
        public float MoveSpeed => _moveSpeed;
        public int Damage => _damage;
        public int CurrencyReward => _currencyReward;
        public bool CanAttackTowers => _canAttackTowers;
        public float AttackRange => _attackRange;
        public float AttackRate => _attackRate;
        public float TowerStunDuration => _towerStunDuration;
        public Sprite Icon => _icon;
        #endregion

        #region Public Methods
        public void Initialize(EnemyType type, string name, int health, float moveSpeed, int damage, int reward, bool canAttackTowers = false)
        {
            _enemyType = type;
            _enemyName = name;
            _health = health;
            _moveSpeed = moveSpeed;
            _damage = damage;
            _currencyReward = reward;
            _canAttackTowers = canAttackTowers;
        }
        #endregion
    }

    /// <summary>
    /// Enum for different enemy types
    /// </summary>
    public enum EnemyType
    {
        Basic,
        Fast,
        Tank,
        TowerAttacker
    }
}