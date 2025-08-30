using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WordBoggle.Constants;
using WordBoggle.Core;

namespace TowerDefense.Core
{
    /// <summary>
    /// Central manager for game state and flow
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        public static GameManager Instance { get; private set; }

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

        void Start()
        {
            InitializeGame();
        }
        #endregion

        #region Properties
        [SerializeField] private GameState _currentGameState = GameState.MainMenu;
        public GameState CurrentGameState => _currentGameState;
        
        [SerializeField] private int _currentLevel = 0;
        public int CurrentLevel => _currentLevel;

        [SerializeField] private int _playerLives = 20;
        public int PlayerLives => _playerLives;
        
        [SerializeField] private bool _isPaused = false;
        public bool IsPaused => _isPaused;
        #endregion

        #region Events
        public delegate void GameStateChangedDelegate(GameState newState);
        public static event GameStateChangedDelegate OnGameStateChanged;
        
        public delegate void PlayerLivesChangedDelegate(int newLives);
        public static event PlayerLivesChangedDelegate OnPlayerLivesChanged;
        
        public delegate void LevelChangedDelegate(int newLevel);
        public static event LevelChangedDelegate OnLevelChanged;
        #endregion

        #region Game Flow Methods
        private void InitializeGame()
        {
            // Initialize subsystems
            ResourceManager.Instance.Initialize();
            
            ChangeGameState(GameState.MainMenu);
        }

        public void StartGame(int level)
        {
            _currentLevel = level;
            _playerLives = 20;
            
            ChangeGameState(GameState.Playing);
            OnLevelChanged?.Invoke(_currentLevel);
            OnPlayerLivesChanged?.Invoke(_playerLives);
            
            // Start the level
            LevelManager.Instance.LoadLevel(_currentLevel);
        }
        
        public void PauseGame()
        {
            if (_currentGameState != GameState.Playing) return;
            
            _isPaused = true;
            Time.timeScale = 0f;
            ChangeGameState(GameState.Paused);
        }
        
        public void ResumeGame()
        {
            if (_currentGameState != GameState.Paused) return;
            
            _isPaused = false;
            Time.timeScale = 1f;
            ChangeGameState(GameState.Playing);
        }
        
        public void GameOver(bool victory = false)
        {
            ChangeGameState(victory ? GameState.Victory : GameState.GameOver);
        }
        
        public void ReturnToMainMenu()
        {
            Time.timeScale = 1f;
            _isPaused = false;
            ChangeGameState(GameState.MainMenu);
            SceneLoader.Instance.LoadScene(GameConstants.MAIN_MENU_SCENE);
        }
        
        public void QuitGame()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
        
        public void LoadNextLevel()
        {
            _currentLevel++;
            OnLevelChanged?.Invoke(_currentLevel);
            
            if (_currentLevel > LevelManager.Instance.MaxLevel)
            {
                GameOver(true); // Victory
                return;
            }
            
            LevelManager.Instance.LoadLevel(_currentLevel);
            ChangeGameState(GameState.Playing);
        }
        
        public void RestartLevel()
        {
            LevelManager.Instance.LoadLevel(_currentLevel);
            ChangeGameState(GameState.Playing);
        }
        
        public void ReducePlayerLives(int amount = 1)
        {
            _playerLives -= amount;
            OnPlayerLivesChanged?.Invoke(_playerLives);
            
            if (_playerLives <= 0)
            {
                GameOver(false);
            }
        }
        #endregion

        #region Helper Methods
        private void ChangeGameState(GameState newState)
        {
            _currentGameState = newState;
            OnGameStateChanged?.Invoke(newState);
        }
        #endregion
    }

    /// <summary>
    /// Enum representing possible game states
    /// </summary>
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver,
        Victory
    }
}