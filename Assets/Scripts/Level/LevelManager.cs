using UnityEngine;
using TowerDefense.Core;
using System.Collections;
using System.Collections.Generic;

namespace TowerDefense.Level
{
    /// <summary>
    /// Manages level loading and level data.
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

        #region Variables
        [SerializeField] private List<LevelData> levels = new List<LevelData>();
        [SerializeField] private LevelData currentLevelData;
        #endregion

        #region Properties
        public List<LevelData> AllLevels => levels;
        public int MaxLevel => levels.Count;
        public LevelData CurrentLevelData => currentLevelData;
        #endregion

        #region Events
        public delegate void LevelLoadedDelegate(LevelData levelData);
        public event LevelLoadedDelegate OnLevelLoaded;
        #endregion

        #region Public Methods
        public void LoadLevel(int levelIndex)
        {
            if (levelIndex <= 0 || levelIndex > levels.Count)
            {
                Debug.LogError($"Invalid level index: {levelIndex}");
                return;
            }

            // Load the level scene if needed
            SceneLoader.Instance.LoadScene(GameConstants.GameplayScene);

            StartCoroutine(LoadLevelRoutine(levelIndex));
        }
        #endregion

        #region Private Methods
        private void LoadLevelData()
        {
            // Load level data from resources or JSON
            levels.Clear();

            // Try to load from Resources first
            LevelData[] resourceLevels = Resources.LoadAll<LevelData>("Levels");
            if (resourceLevels != null && resourceLevels.Length > 0)
            {
                levels.AddRange(resourceLevels);
            }

            // If we don't have at least 3 levels, load from JSON
            if (levels.Count < 3)
            {
                LoadLevelsFromJSON();
            }

            // If we still don't have levels, create defaults
            if (levels.Count < 3)
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
                levels.Add(level);
            }
        }

        private IEnumerator LoadLevelRoutine(int levelIndex)
        {
            // Wait for scene to load
            yield return new WaitForEndOfFrame();

            currentLevelData = levels[levelIndex - 1];
            
            // Notify subscribers
            OnLevelLoaded?.Invoke(currentLevelData);
        }
        #endregion
    }
}