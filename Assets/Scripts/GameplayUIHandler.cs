using UnityEngine;
using WordBoggle.Enums;
using WordBoggle.UI.Menus;
using TowerDefense.Core;
using TowerDefense.Towers;

namespace WordBoggle.UI.Handlers
{
    public class GameplayUIHandler : MonoBehaviour
    {
        #region Variables

        [SerializeField] private MenuManager _menuManager;
        [SerializeField] private TowerController towerController;
        private GameplayMenu _menu;

                public static GameplayUIHandler Instance { get; private set; }

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

        #endregion

        #region Unity Callbacks

        private void Start()
        {
            _menu = _menuManager.GetMenu<GameplayMenu>(MenuType.GameplayMenu);
            GameManager.OnGameStateChanged += HandleGameStateChanged;
        }

        private void OnDestroy()
        {
            GameManager.OnGameStateChanged -= HandleGameStateChanged;
        }

        private void HandleGameStateChanged(GameState newState)
        {
            ShowScreenForGameState(newState);
        }

        #endregion

        #region Public Methods

        public void OpenMenu(MenuType type)
        {
            _menuManager.OpenMenu(type);
        }
        public void SelectTower(int towerTypeIndex)
        {
            towerController.SelectTower((TowerType)towerTypeIndex);
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
    }
}