using UnityEngine;
using TowerDefense.Core;
using TowerDefense.Enums;
using TowerDefense.Towers;
using TowerDefense.UI.Menus;

namespace TowerDefense.UI.Handlers
{
    /// <summary>
    /// Handles the gameplay UI interactions.
    /// </summary>
    public class GameplayUIHandler : MonoBehaviour
    {
        #region Variables
        [SerializeField] private MenuManager menuManager;
        [SerializeField] private TowerController towerController;
        #endregion

        #region Properties
        public static GameplayUIHandler Instance { get; private set; }
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
            ShowScreenForGameState(GameManager.Instance.CurrentGameState);
        }

        private void Start()
        {
            GameManager.OnGameStateChanged += HandleGameStateChanged;
        }

        private void OnDestroy()
        {
            GameManager.OnGameStateChanged -= HandleGameStateChanged;
        }
        #endregion

        #region Public Methods
        public void OpenMenu(MenuType type)
        {
            menuManager.OpenMenu(type);
        }
        public void SelectTower(string towerId)
        {
            towerController.SelectTower(towerId);
        }

        public void StartTowerPlacement()
        {
            towerController.StartPlacement();
        }

        public void CancelTowerPlacement()
        {
            towerController.CancelPlacement();
        }

        #endregion

        #region Private Methods
        private void HandleGameStateChanged(GameState newState)
        {
            ShowScreenForGameState(newState);
        }

        private void ShowScreenForGameState(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.Playing:
                    OpenMenu(MenuType.GameplayMenu);
                    break;
                case GameState.Paused:
                    OpenMenu(MenuType.PauseMenu);
                    break;
                case GameState.GameOver:
                    OpenMenu(MenuType.GameOverMenu);
                    break;
            }
        }
        #endregion
    }
}