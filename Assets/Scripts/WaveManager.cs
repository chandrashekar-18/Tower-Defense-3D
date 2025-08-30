using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TowerDefense.Level;
using TowerDefense.Enemies;
using TowerDefense.Grid;

namespace TowerDefense.Core
{
    /// <summary>
    /// Manages enemy waves and spawning
    /// </summary>
    public class WaveManager : MonoBehaviour
    {
        #region Singleton
        public static WaveManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }

        #endregion

        #region Properties
        [SerializeField] private List<WaveData> _waves = new List<WaveData>();
        [SerializeField] private int _currentWaveIndex = -1;
        [SerializeField] private int _remainingEnemiesInWave = 0;
        [SerializeField] private bool _isSpawning = false;
        [SerializeField] private float _timeBetweenWaves = 5f;
        
        private EnemyFactory _enemyFactory;
        
        public int CurrentWaveIndex => _currentWaveIndex;
        public int TotalWaves => _waves.Count;
        public bool IsLastWave => _currentWaveIndex >= _waves.Count - 1;
        public int RemainingEnemies => _remainingEnemiesInWave;
        public bool IsSpawning => _isSpawning;
        #endregion

        #region Events
        public delegate void WaveStartedDelegate(int waveIndex, WaveData waveData);
        public static event WaveStartedDelegate OnWaveStarted;
        
        public delegate void WaveCompletedDelegate(int waveIndex);
        public static event WaveCompletedDelegate OnWaveCompleted;
        
        public delegate void AllWavesCompletedDelegate();
        public static event AllWavesCompletedDelegate OnAllWavesCompleted;
        
        public delegate void EnemySpawnedDelegate(EnemyController enemy);
        public static event EnemySpawnedDelegate OnEnemySpawned;
        
        public delegate void EnemyDefeatedDelegate(EnemyController enemy);
        public static event EnemyDefeatedDelegate OnEnemyDefeated;
        
        public delegate void TimeBetweenWavesChangedDelegate(float remainingTime);
        public static event TimeBetweenWavesChangedDelegate OnTimeBetweenWavesChanged;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            _enemyFactory = GetComponent<EnemyFactory>();
            if (_enemyFactory == null)
            {
                _enemyFactory = gameObject.AddComponent<EnemyFactory>();
            }

            InitializeWaves(LevelManager.Instance.CurrentLevelData);
        }
        #endregion

        #region Public Methods
        public void InitializeWaves(LevelData levelData)
        {
            _waves = new List<WaveData>(levelData.Waves);
            _currentWaveIndex = -1;
            _remainingEnemiesInWave = 0;
            _isSpawning = false;
            
            // Start first wave after a delay
            StartCoroutine(StartWaveAfterDelay(_timeBetweenWaves));
        }
        
        public void StartNextWave()
        {
            if (_isSpawning) return;
            
            _currentWaveIndex++;
            
            if (_currentWaveIndex < _waves.Count)
            {
                StartCoroutine(SpawnWave(_waves[_currentWaveIndex]));
            }
            else
            {
                // All waves completed
                OnAllWavesCompleted?.Invoke();
                
                // Level complete
                GameManager.Instance.GameOver(true);
            }
        }
        
        public void EnemyDefeated(EnemyController enemy)
        {
            _remainingEnemiesInWave--;
            OnEnemyDefeated?.Invoke(enemy);
            
            // Check if wave is complete
            if (_remainingEnemiesInWave <= 0 && !_isSpawning)
            {
                OnWaveCompleted?.Invoke(_currentWaveIndex);
                
                if (!IsLastWave)
                {
                    // Start next wave after delay
                    StartCoroutine(StartWaveAfterDelay(_timeBetweenWaves));
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
            _isSpawning = true;
            OnWaveStarted?.Invoke(_currentWaveIndex, waveData);
            
            // Count total enemies in wave
            _remainingEnemiesInWave = 0;
            foreach (var enemyGroup in waveData.EnemyGroups)
            {
                _remainingEnemiesInWave += enemyGroup.Count;
            }
            
            // Spawn enemy groups
            foreach (var enemyGroup in waveData.EnemyGroups)
            {
                for (int i = 0; i < enemyGroup.Count; i++)
                {
                    // Spawn enemy at random spawn point
                    SpawnEnemy(enemyGroup.EnemyType);
                    
                    // Wait between spawns
                    yield return new WaitForSeconds(enemyGroup.SpawnDelay);
                }
                
                // Wait between groups
                yield return new WaitForSeconds(waveData.DelayBetweenGroups);
            }
            
            _isSpawning = false;
            
            // Check if wave is already complete (all enemies defeated during spawning)
            if (_remainingEnemiesInWave <= 0)
            {
                OnWaveCompleted?.Invoke(_currentWaveIndex);
                
                if (!IsLastWave)
                {
                    // Start next wave after delay
                    StartCoroutine(StartWaveAfterDelay(_timeBetweenWaves));
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
        
        private void SpawnEnemy(EnemyType enemyType)
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
            
            // Create enemy
            EnemyController enemy = _enemyFactory.CreateEnemy(enemyType, spawnPoint.position, path);
            
            // Register for defeat event
            enemy.OnDefeated += EnemyDefeated;
            
            // Notify subscribers
            OnEnemySpawned?.Invoke(enemy);
        }
        #endregion
    }
}