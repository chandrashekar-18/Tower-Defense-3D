using UnityEngine;
using TowerDefense.Enums;
using TowerDefense.Enemies;

namespace TowerDefense.Core
{
    /// <summary>
    /// Manages player score
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        #region Singleton
        public static ScoreManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        #endregion

        #region Properties
        [SerializeField] private int _currentScore = 0;

        public int CurrentScore => _currentScore;
        #endregion

        #region Events
        public delegate void ScoreChangedDelegate(int newScore);
        public static event ScoreChangedDelegate OnScoreChanged;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            // Register for events
            WaveManager.OnEnemyDefeated += HandleEnemyDefeated;
            WaveManager.OnWaveCompleted += HandleWaveCompleted;
            GameManager.OnGameStateChanged += HandleGameStateChanged;
        }

        private void OnDestroy()
        {
            // Unregister from events
            WaveManager.OnEnemyDefeated -= HandleEnemyDefeated;
            WaveManager.OnWaveCompleted -= HandleWaveCompleted;
            GameManager.OnGameStateChanged -= HandleGameStateChanged;
        }
        #endregion

        #region Public Methods
        public void AddScore(int amount)
        {
            _currentScore += amount;
            OnScoreChanged?.Invoke(_currentScore);
        }

        public void ResetScore()
        {
            _currentScore = 0;
            OnScoreChanged?.Invoke(_currentScore);
        }
        #endregion

        #region Private Methods
        private void HandleEnemyDefeated(EnemyController enemy)
        {
            // Add score based on enemy type
            int scoreValue = 0;

            switch (enemy.EnemyType)
            {
                case EnemyType.Basic:
                    scoreValue = 10;
                    break;
                case EnemyType.Fast:
                    scoreValue = 15;
                    break;
                case EnemyType.Tank:
                    scoreValue = 25;
                    break;
                case EnemyType.TowerAttacker:
                    scoreValue = 30;
                    break;
            }

            AddScore(scoreValue);
        }

        private void HandleWaveCompleted(int waveIndex)
        {
            // Bonus score for completing a wave
            AddScore(50 + (waveIndex * 25));
        }

        private void HandleGameStateChanged(GameState newState)
        {
            if (newState == GameState.Playing)
            {
                // Reset score when starting a new game
                if (GameManager.Instance.CurrentLevel == 1)
                {
                    ResetScore();
                }
            }
        }
        #endregion
    }
}