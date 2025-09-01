using UnityEngine;
using TowerDefense.Enums;
using TowerDefense.Enemies;

namespace TowerDefense.Core
{
    /// <summary>
    /// Manages player score.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        #region Singleton
        public static ScoreManager Instance { get; private set; }
        #endregion

        #region Variables
        private int currentScore = 0;
        public int CurrentScore => currentScore;
        #endregion

        #region Events
        public delegate void ScoreChangedDelegate(int newScore);
        public static event ScoreChangedDelegate OnScoreChanged;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
        private void Start()
        {
            WaveManager.OnEnemyDefeated += HandleEnemyDefeated;
            WaveManager.OnWaveCompleted += HandleWaveCompleted;
            GameManager.OnGameStateChanged += HandleGameStateChanged;
            ResetScore();
        }

        private void OnDestroy()
        {
            WaveManager.OnEnemyDefeated -= HandleEnemyDefeated;
            WaveManager.OnWaveCompleted -= HandleWaveCompleted;
            GameManager.OnGameStateChanged -= HandleGameStateChanged;
        }
        #endregion

        #region Public Methods
        public void AddScore(int amount)
        {
            currentScore += amount;
            OnScoreChanged?.Invoke(currentScore);
        }

        public void ResetScore()
        {
            currentScore = 0;
            OnScoreChanged?.Invoke(currentScore);
        }
        #endregion

        #region Private Methods
        private void HandleEnemyDefeated(Enemy enemy)
        {
            AddScore(enemy.EnemyData.ScoreValue);
        }

        private void HandleWaveCompleted(int waveIndex)
        {
            AddScore(50 + (waveIndex * 25));
        }

        private void HandleGameStateChanged(GameState newState)
        {
            if (newState == GameState.Playing)
            {
                if (GameManager.Instance.CurrentLevel == 1)
                {
                    ResetScore();
                }
            }
        }
        #endregion
    }
}