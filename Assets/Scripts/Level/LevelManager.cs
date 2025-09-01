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
        #endregion

        #region Variables
        private List<LevelData> levels = new List<LevelData>();
        private LevelData currentLevelData;
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

        #region Unity Lifecycle
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

        #region Public Methods
        public void LoadLevel(int levelIndex)
        {
            if (levelIndex <= 0 || levelIndex > levels.Count)
            {
                Debug.LogError($"Invalid level index: {levelIndex}");
                return;
            }
            currentLevelData = levels[levelIndex - 1];
            currentLevelData.DebugPrintGrid();
            OnLevelLoaded?.Invoke(currentLevelData);
            SceneLoader.Instance.LoadScene(GameConstants.GameplayScene);
        }
        #endregion

        #region Private Methods
        private void LoadLevelData()
        {
            levels.Clear();

            LevelData[] resourceLevels = Resources.LoadAll<LevelData>("Levels");
            if (resourceLevels != null && resourceLevels.Length > 0)
            {
                levels.AddRange(resourceLevels);
            }
        }
        #endregion
    }
}