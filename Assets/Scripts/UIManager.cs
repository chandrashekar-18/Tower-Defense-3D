using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using TowerDefense.Core;
using TowerDefense.Towers;
using TowerDefense.Level;
using System.Collections.Generic;

namespace TowerDefense.UI
{
    /// <summary>
    /// Manages all UI elements and screens
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        #region Singleton
        public static UIManager Instance { get; private set; }

        // private void Awake()
        // {
        //     if (Instance != null && Instance != this)
        //     {
        //         Destroy(gameObject);
        //         return;
        //     }
            
        //     Instance = this;
        // }
        #endregion

        #region Properties
        [Header("UI Screens")]
        [SerializeField] private GameObject _mainMenuScreen;
        [SerializeField] private GameObject _gameScreen;
        [SerializeField] private GameObject _pauseScreen;
        [SerializeField] private GameObject _gameOverScreen;
        [SerializeField] private GameObject _victoryScreen;
        [SerializeField] private GameObject _instructionsScreen;
        
        [Header("HUD Elements")]
        [SerializeField] private TextMeshProUGUI _currencyText;
        [SerializeField] private TextMeshProUGUI _livesText;
        [SerializeField] private TextMeshProUGUI _waveText;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _nextWaveTimerText;
        [SerializeField] private GameObject _nextWaveTimerPanel;
        
        [Header("Tower Selection")]
        [SerializeField] private GameObject _towerSelectionPanel;
        [SerializeField] private Transform _towerButtonsContainer;
        [SerializeField] private GameObject _towerButtonPrefab;
        [SerializeField] private TowerController _towerController;
        
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            // Initialize UI
            // HideAllScreens();
            
            // Register for events
            // GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
            // GameManager.Instance.OnPlayerLivesChanged += UpdateLivesDisplay;
            // GameManager.Instance.OnLevelChanged += UpdateLevelDisplay;
            
            // ResourceManager.Instance.OnCurrencyChanged += UpdateCurrencyDisplay;
        
            
            // WaveManager.Instance.OnWaveStarted += HandleWaveStarted;
            // WaveManager.Instance.OnTimeBetweenWavesChanged += UpdateNextWaveTimer;
            
            // Show initial screen based on current game state
            // ShowScreenForGameState(GameManager.Instance.CurrentGameState);
            
            // Initialize tower selection
            // PopulateTowerButtons();
        }
        
        private void OnDestroy()
        {
            // Unregister from events
            // if (GameManager.Instance != null)
            // {
            //     GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
            //     GameManager.Instance.OnPlayerLivesChanged -= UpdateLivesDisplay;
            //     GameManager.Instance.OnLevelChanged -= UpdateLevelDisplay;
            // }
            
            // if (ResourceManager.Instance != null)
            // {
            //     ResourceManager.Instance.OnCurrencyChanged -= UpdateCurrencyDisplay;
            // }
            
            // if (WaveManager.Instance != null)
            // {
            //     WaveManager.Instance.OnWaveStarted -= HandleWaveStarted;
            //     WaveManager.Instance.OnTimeBetweenWavesChanged -= UpdateNextWaveTimer;
            // }
        }
        #endregion

        #region Public Methods
        
        // public void PauseGame()
        // {
        //     GameManager.Instance.PauseGame();
        // }
        
        // public void ResumeGame()
        // {
        //     GameManager.Instance.ResumeGame();
        // }
        
        // public void RestartLevel()
        // {
        //     GameManager.Instance.RestartLevel();
        // }
        
        // public void ReturnToMainMenu()
        // {
        //     GameManager.Instance.ReturnToMainMenu();
        // }
        
        // public void ShowInstructions()
        // {
        //     HideAllScreens();
        //     _instructionsScreen.SetActive(true);
        // }
        
        // public void HideInstructions()
        // {
        //     _instructionsScreen.SetActive(false);
        //     ShowScreenForGameState(GameManager.Instance.CurrentGameState);
        // }
        
        // public void SelectTower(int towerTypeIndex)
        // {
        //     if (_towerController != null)
        //     {
        //         _towerController.SelectTower((TowerType)towerTypeIndex);
        //     }
        // }
        
        // public void StartTowerPlacement()
        // {
        //     if (_towerController != null)
        //     {
        //         _towerController.StartPlacement();
        //     }
        // }
        
        // public void CancelTowerPlacement()
        // {
        //     if (_towerController != null)
        //     {
        //         _towerController.CancelPlacement();
        //     }
        // }
        #endregion

        #region Private Methods
        // private void HideAllScreens()
        // {
        //     if (_mainMenuScreen != null) _mainMenuScreen.SetActive(false);
        //     if (_gameScreen != null) _gameScreen.SetActive(false);
        //     if (_pauseScreen != null) _pauseScreen.SetActive(false);
        //     if (_gameOverScreen != null) _gameOverScreen.SetActive(false);
        //     if (_victoryScreen != null) _victoryScreen.SetActive(false);
        //     if (_instructionsScreen != null) _instructionsScreen.SetActive(false);
        // }
        
        // private void ShowScreenForGameState(GameState gameState)
        // {
        //     HideAllScreens();
            
        //     switch (gameState)
        //     {
        //         case GameState.MainMenu:
        //             _mainMenuScreen.SetActive(true);
        //             break;
        //         case GameState.Playing:
        //             _gameScreen.SetActive(true);
        //             break;
        //         case GameState.Paused:
        //             _gameScreen.SetActive(true);
        //             _pauseScreen.SetActive(true);
        //             break;
        //         case GameState.GameOver:
        //             _gameOverScreen.SetActive(true);
        //             break;
        //         case GameState.Victory:
        //             _victoryScreen.SetActive(true);
        //             break;
        //     }
        // }
        
        // private void HandleGameStateChanged(GameState newState)
        // {
        //     ShowScreenForGameState(newState);
        // }
        
        // private void UpdateCurrencyDisplay(int newAmount)
        // {
        //     if (_currencyText != null)
        //     {
        //         _currencyText.text = $"$ {newAmount}";
        //     }
        // }
        
        // private void UpdateLivesDisplay(int newLives)
        // {
        //     if (_livesText != null)
        //     {
        //         _livesText.text = $"Lives: {newLives}";
        //     }
        // }
        
        // private void UpdateLevelDisplay(int newLevel)
        // {
        //     if (_levelText != null)
        //     {
        //         _levelText.text = $"Level {newLevel}";
        //     }
        // }
        
        // private void UpdateScoreDisplay(int newScore)
        // {
        //     if (_scoreText != null)
        //     {
        //         _scoreText.text = $"Score: {newScore}";
        //     }
        // }
        
        // private void HandleWaveStarted(int waveIndex, WaveData waveData)
        // {
        //     if (_waveText != null)
        //     {
        //         _waveText.text = $"Wave: {waveIndex + 1}/{WaveManager.Instance.TotalWaves}";
        //     }
            
        //     if (_nextWaveTimerPanel != null)
        //     {
        //         _nextWaveTimerPanel.SetActive(false);
        //     }
        // }
        
        // private void UpdateNextWaveTimer(float remainingTime)
        // {
        //     if (_nextWaveTimerPanel != null)
        //     {
        //         _nextWaveTimerPanel.SetActive(true);
        //     }
            
        //     if (_nextWaveTimerText != null)
        //     {
        //         _nextWaveTimerText.text = $"Next Wave: {Mathf.CeilToInt(remainingTime)}s";
        //     }
        // }
        
        // private void PopulateTowerButtons()
        // {
        //     // Clear existing buttons
        //     foreach (Transform child in _towerButtonsContainer)
        //     {
        //         Destroy(child.gameObject);
        //     }
            
        //     // Get available towers
        //     TowerFactory towerFactory = FindObjectOfType<TowerFactory>();
        //     if (towerFactory == null)
        //         return;
                
        //     List<TowerData> availableTowers = towerFactory.GetAvailableTowers();
            
        //     // Create buttons for each tower
        //     foreach (TowerData towerData in availableTowers)
        //     {
        //         GameObject buttonObj = Instantiate(_towerButtonPrefab, _towerButtonsContainer);
        //         TowerButton towerButton = buttonObj.GetComponent<TowerButton>();
                
        //         if (towerButton != null)
        //         {
        //             towerButton.Initialize(towerData);
        //         }
        //     }
        // }
        
        #endregion
    }
}