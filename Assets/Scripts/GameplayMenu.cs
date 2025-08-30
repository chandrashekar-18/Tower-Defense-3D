using System.Collections.Generic;
using TMPro;
using TowerDefense.Core;
using TowerDefense.Level;
using TowerDefense.Towers;
using TowerDefense.UI;
using UnityEngine;
using UnityEngine.UI;
using WordBoggle.Enums;
using WordBoggle.UI.Handlers;

namespace WordBoggle.UI.Menus
{
    public class GameplayMenu : Menu
    {
        #region Private Methods

        private void OnPauseBtnClicked()
        {
            GameplayUIHandler.Instance.OpenMenu(MenuType.PauseMenu);
        }

        [SerializeField] private TextMeshProUGUI currencyText;
        [SerializeField] private TextMeshProUGUI livesText;
        [SerializeField] private TextMeshProUGUI waveText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI nextWaveTimerText;
        [SerializeField] private GameObject nextWaveTimerPanel;
        [SerializeField] private Button pauseBtn;

        [Header("Tower Selection")]
        [SerializeField] private GameObject towerSelectionPanel;
        [SerializeField] private Transform towerButtonsContainer;
        [SerializeField] private GameObject towerButtonPrefab;

        #endregion

        public override void Open()
        {
            base.Open();
            ResourceManager.OnCurrencyChanged += UpdateCurrencyDisplay;
            TowerDefense.Core.GameManager.OnPlayerLivesChanged += UpdateLivesDisplay;
            TowerDefense.Core.GameManager.OnLevelChanged += UpdateLevelDisplay;
            ScoreManager.OnScoreChanged += UpdateScoreDisplay;
            WaveManager.OnWaveStarted += HandleWaveStarted;
            WaveManager.OnTimeBetweenWavesChanged += UpdateNextWaveTimer;
            pauseBtn.onClick.AddListener(OnPauseBtnClicked);
            PopulateTowerButtons();
        }

        public override void Close()
        {
            base.Close();
            ResourceManager.OnCurrencyChanged -= UpdateCurrencyDisplay;
            TowerDefense.Core.GameManager.OnPlayerLivesChanged -= UpdateLivesDisplay;
            TowerDefense.Core.GameManager.OnLevelChanged -= UpdateLevelDisplay;
            ScoreManager.OnScoreChanged -= UpdateScoreDisplay;
            WaveManager.OnWaveStarted -= HandleWaveStarted;
            WaveManager.OnTimeBetweenWavesChanged -= UpdateNextWaveTimer;
            pauseBtn.onClick.RemoveListener(OnPauseBtnClicked);
        }


        private void UpdateCurrencyDisplay(int newAmount)
        {
            currencyText.text = $"$ {newAmount}";
        }

        private void UpdateLivesDisplay(int newLives)
        {
            livesText.text = $"Lives: {newLives}";
        }

        private void UpdateLevelDisplay(int newLevel)
        {
            levelText.text = $"Level {newLevel}";
        }

        private void UpdateScoreDisplay(int newScore)
        {
            scoreText.text = $"Score: {newScore}";
        }

        private void HandleWaveStarted(int waveIndex, WaveData waveData)
        {
            waveText.text = $"Wave: {waveIndex + 1}/{WaveManager.Instance.TotalWaves}";
            nextWaveTimerPanel.SetActive(false);
        }

        private void UpdateNextWaveTimer(float remainingTime)
        {
            nextWaveTimerPanel.SetActive(true);
            nextWaveTimerText.text = $"Next Wave: {Mathf.CeilToInt(remainingTime)}s";
        }

        private void PopulateTowerButtons()
        {
            foreach (Transform child in towerButtonsContainer)
            {
                Destroy(child.gameObject);
            }

            TowerFactory towerFactory = FindObjectOfType<TowerFactory>();
            List<TowerData> availableTowers = towerFactory.GetAvailableTowers();
            foreach (TowerData towerData in availableTowers)
            {
                GameObject buttonObj = Instantiate(towerButtonPrefab, towerButtonsContainer);
                TowerButton towerButton = buttonObj.GetComponent<TowerButton>();
                towerButton.Initialize(towerData);
            }
        }
    }
}