using UnityEngine;
using TowerDefense.Core;
using System.Collections;
using System.Collections.Generic;

namespace TowerDefense.Level
{
    /// <summary>
    /// Manages level loading and level data
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        #region Singleton
        public static LevelManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadLevelData();
        }
        #endregion

        #region Properties
        [SerializeField] private List<LevelData> _levels = new List<LevelData>();
        [SerializeField] private LevelData _currentLevelData;

        public List<LevelData> AllLevels => _levels;
        public int MaxLevel => _levels.Count;
        public LevelData CurrentLevelData => _currentLevelData;
        #endregion

        #region Events
        public delegate void LevelLoadedDelegate(LevelData levelData);
        public event LevelLoadedDelegate OnLevelLoaded;
        #endregion

        #region Public Methods
        public void LoadLevel(int levelIndex)
        {
            if (levelIndex <= 0 || levelIndex > _levels.Count)
            {
                Debug.LogError($"Invalid level index: {levelIndex}");
                return;
            }

            // Load the level scene if needed
            SceneLoader.Instance.LoadScene(GameConstants.GAMEPLAY_SCENE);

            StartCoroutine(LoadLevelRoutine(levelIndex));
        }
        #endregion

        #region Private Methods
        private void LoadLevelData()
        {
            // Load level data from resources or JSON
            _levels.Clear();

            // Try to load from Resources first
            LevelData[] resourceLevels = Resources.LoadAll<LevelData>("Levels");
            if (resourceLevels != null && resourceLevels.Length > 0)
            {
                _levels.AddRange(resourceLevels);
            }

            // If we don't have at least 3 levels, load from JSON
            if (_levels.Count < 3)
            {
                LoadLevelsFromJSON();
            }

            // If we still don't have levels, create defaults
            if (_levels.Count < 3)
            {
                CreateDefaultLevels();
            }
        }

        private void LoadLevelsFromJSON()
        {
            // Implementation for loading levels from JSON
            // This would be implemented as part of the LevelEditor
        }

        private void CreateDefaultLevels()
        {
            // Create 3 default levels
            for (int i = 0; i < 3; i++)
            {
                LevelData level = ScriptableObject.CreateInstance<LevelData>();
                level.LevelNumber = i + 1;
                level.InitializeDefaultValues();
                _levels.Add(level);
            }
        }

        private IEnumerator LoadLevelRoutine(int levelIndex)
        {
            // Wait for scene to load
            yield return new WaitForEndOfFrame();

            _currentLevelData = _levels[levelIndex - 1];

            // Setup the grid
            // GridManager.Instance.GenerateGrid(_currentLevelData);

            // Setup wave manager
            // WaveManager.Instance.InitializeWaves(_currentLevelData);

            // Notify subscribers
            OnLevelLoaded?.Invoke(_currentLevelData);
        }
        #endregion
    }
}