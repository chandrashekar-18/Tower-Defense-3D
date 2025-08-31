using UnityEngine;
using TowerDefense.Level;
using TowerDefense.Enums;

namespace TowerDefense.Core
{
    /// <summary>
    /// Central manager for game state and flow.
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

        private void Start()
        {
            InitializeGame();
        }
        #endregion

        #region Variables
        [SerializeField] private GameState currentGameState = GameState.MainMenu;
        [SerializeField] private int currentLevel = 0;
        [SerializeField] private int playerLives = 20;
        [SerializeField] private bool isPaused = false;
        #endregion

        #region Properties
        public GameState CurrentGameState => currentGameState;

        public int CurrentLevel => currentLevel;

        public int PlayerLives => playerLives;

        public bool IsPaused => isPaused;
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
            currentLevel = level;
            playerLives = 20;

            ChangeGameState(GameState.Playing);
            OnLevelChanged?.Invoke(currentLevel);
            OnPlayerLivesChanged?.Invoke(playerLives);

            // Start the level
            LevelManager.Instance.LoadLevel(currentLevel);
        }

        public void PauseGame()
        {
            if (currentGameState != GameState.Playing) return;

            isPaused = true;
            Time.timeScale = 0f;
            ChangeGameState(GameState.Paused);
        }

        public void ResumeGame()
        {
            if (currentGameState != GameState.Paused) return;

            isPaused = false;
            Time.timeScale = 1f;
            ChangeGameState(GameState.Playing);
        }

        public void GameOver(bool victory = false)
        {
            ChangeGameState(GameState.GameOver);
        }

        public void ReturnToMainMenu()
        {
            Time.timeScale = 1f;
            isPaused = false;
            ChangeGameState(GameState.MainMenu);
            SceneLoader.Instance.LoadScene(GameConstants.MainMenuScene);
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
            currentLevel++;
            OnLevelChanged?.Invoke(currentLevel);

            if (currentLevel > LevelManager.Instance.MaxLevel)
            {
                GameOver(true); // Victory
                return;
            }

            LevelManager.Instance.LoadLevel(currentLevel);
            ChangeGameState(GameState.Playing);
        }

        public void RestartLevel()
        {
            LevelManager.Instance.LoadLevel(currentLevel);
        }

        public void ReducePlayerLives(int amount = 1)
        {
            playerLives -= amount;
            OnPlayerLivesChanged?.Invoke(playerLives);

            if (playerLives <= 0)
            {
                GameOver(false);
            }
        }
        #endregion

        #region Helper Methods
        private void ChangeGameState(GameState newState)
        {
            currentGameState = newState;
            OnGameStateChanged?.Invoke(newState);
        }
        #endregion
    }
}