using UnityEngine;
using TowerDefense.Grid;
using System.Collections;
using TowerDefense.Level;
using TowerDefense.Enemies;
using System.Collections.Generic;

namespace TowerDefense.Core
{
    /// <summary>
    /// Manages enemy waves and spawning.
    /// </summary>
    public class WaveManager : MonoBehaviour
    {
        #region Singleton
        public static WaveManager Instance { get; private set; }
        #endregion

        #region Variables
        [SerializeField] private EnemyFactory enemyFactory;
        private List<WaveData> waves = new List<WaveData>();
        private int currentWaveIndex = -1;
        private int remainingEnemiesInWave = 0;
        private bool isSpawning = false;
        private float timeBetweenWaves = 5f;
        #endregion

        #region Properties
        public int CurrentWaveIndex => currentWaveIndex;
        public int TotalWaves => waves.Count;
        public bool IsLastWave => currentWaveIndex >= waves.Count - 1;
        public int RemainingEnemies => remainingEnemiesInWave;
        public bool IsSpawning => isSpawning;
        public WaveData CurrentWave => currentWaveIndex >= 0 && currentWaveIndex < waves.Count ? waves[currentWaveIndex] : null;
        #endregion

        #region Events
        public delegate void WaveStartedDelegate(int waveIndex, WaveData waveData);
        public static event WaveStartedDelegate OnWaveStarted;

        public delegate void WaveCompletedDelegate(int waveIndex);
        public static event WaveCompletedDelegate OnWaveCompleted;

        public delegate void AllWavesCompletedDelegate();
        public static event AllWavesCompletedDelegate OnAllWavesCompleted;

        public delegate void EnemySpawnedDelegate(Enemy enemy);
        public static event EnemySpawnedDelegate OnEnemySpawned;

        public delegate void EnemyDefeatedDelegate(Enemy enemy);
        public static event EnemyDefeatedDelegate OnEnemyDefeated;

        public delegate void TimeBetweenWavesChangedDelegate(float remainingTime);
        public static event TimeBetweenWavesChangedDelegate OnTimeBetweenWavesChanged;
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
            InitializeWaves(LevelManager.Instance.CurrentLevelData);
        }
        #endregion

        #region Public Methods
        public void InitializeWaves(LevelData levelData)
        {
            waves = new List<WaveData>(levelData.Waves);
            currentWaveIndex = -1;
            remainingEnemiesInWave = 0;
            isSpawning = false;

            // Start first wave after a delay
            StartCoroutine(StartWaveAfterDelay(timeBetweenWaves));
        }

        public void StartNextWave()
        {
            if (isSpawning) return;

            currentWaveIndex++;

            if (currentWaveIndex < waves.Count)
            {
                StartCoroutine(SpawnWave(waves[currentWaveIndex]));
            }
            else
            {
                // All waves completed
                OnAllWavesCompleted?.Invoke();

                // Level complete
                GameManager.Instance.GameOver(true);
            }
        }

        public void EnemyDefeated(Enemy enemy)
        {
            remainingEnemiesInWave--;
            OnEnemyDefeated?.Invoke(enemy);

            // Check if wave is complete
            if (remainingEnemiesInWave <= 0 && !isSpawning)
            {
                OnWaveCompleted?.Invoke(currentWaveIndex);

                if (!IsLastWave)
                {
                    // Start next wave after delay
                    StartCoroutine(StartWaveAfterDelay(timeBetweenWaves));
                }
                else
                {
                    // All waves completed
                    OnAllWavesCompleted?.Invoke();

                    // Level complete
                    GameManager.Instance.GameOver(true);
                }
            }
        }

        /// <summary>
        /// Get preview of upcoming wave
        /// </summary>
        public WaveData GetUpcomingWave()
        {
            int nextWaveIndex = currentWaveIndex + 1;
            return nextWaveIndex < waves.Count ? waves[nextWaveIndex] : null;
        }
        #endregion

        #region Private Methods
        private IEnumerator StartWaveAfterDelay(float delay)
        {
            float remainingTime = delay;

            while (remainingTime > 0)
            {
                OnTimeBetweenWavesChanged?.Invoke(remainingTime);
                yield return new WaitForSeconds(0.1f);
                remainingTime -= 0.1f;
            }

            StartNextWave();
        }

        private IEnumerator SpawnWave(WaveData waveData)
        {
            isSpawning = true;
            OnWaveStarted?.Invoke(currentWaveIndex, waveData);

            // Count total enemies in wave
            remainingEnemiesInWave = waveData.GetTotalEnemyCount();

            // Spawn enemy groups
            foreach (var enemyGroup in waveData.EnemyGroups)
            {
                for (int i = 0; i < enemyGroup.Count; i++)
                {
                    // Spawn enemy at random spawn point
                    SpawnEnemy(enemyGroup.EnemyId);

                    // Wait between spawns
                    yield return new WaitForSeconds(enemyGroup.SpawnDelay);
                }

                // Wait between groups
                yield return new WaitForSeconds(waveData.DelayBetweenGroups);
            }

            isSpawning = false;

            // Check if wave is already complete (all enemies defeated during spawning)
            if (remainingEnemiesInWave <= 0)
            {
                OnWaveCompleted?.Invoke(currentWaveIndex);

                if (!IsLastWave)
                {
                    // Start next wave after delay
                    StartCoroutine(StartWaveAfterDelay(timeBetweenWaves));
                }
                else
                {
                    // All waves completed
                    OnAllWavesCompleted?.Invoke();

                    // Level complete
                    GameManager.Instance.GameOver(true);
                }
            }
        }

        private void SpawnEnemy(string enemyId)
        {
            // Choose random spawn point
            Transform[] spawnPoints = GridManager.Instance.GetSpawnPoints();
            if (spawnPoints.Length == 0)
            {
                Debug.LogError("No spawn points found!");
                return;
            }

            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Get path from spawn point
            List<Vector3> path = GridManager.Instance.GetPathFromSpawnPoint(spawnPoint);

            // Create enemy using factory
            Enemy enemy = enemyFactory.CreateEnemy(enemyId, spawnPoint.position, path);
            
            if (enemy == null)
            {
                Debug.LogError($"Failed to create enemy '{enemyId}'!");
                remainingEnemiesInWave--; // Reduce count to prevent infinite waiting
                return;
            }

            // Register for defeat event
            enemy.OnDefeated += EnemyDefeated;

            // Notify subscribers
            OnEnemySpawned?.Invoke(enemy);
        }
        #endregion
    }
}