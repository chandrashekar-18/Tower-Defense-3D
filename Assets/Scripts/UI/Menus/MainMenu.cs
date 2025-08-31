using UnityEngine;
using UnityEngine.UI;
using TowerDefense.Core;
using TowerDefense.UI.Handlers;

namespace TowerDefense.UI.Menus
{
    /// <summary>
    /// Main menu UI for the game.
    /// </summary>
    public class MainMenu : Menu
    {
        #region Variables
        [SerializeField] private Button playButton;
        [SerializeField] private Button instructionsButton;
        [SerializeField] private Button quitButton;
        #endregion

        #region Base Methods
        public override void Open()
        {
            base.Open();
            playButton.onClick.AddListener(OnPlayBtnClicked);
            instructionsButton.onClick.AddListener(OnInstructionsBtnClicked);
            quitButton.onClick.AddListener(OnQuitBtnClicked);
        }

        public override void Close()
        {
            base.Close();
            playButton.onClick.RemoveListener(OnPlayBtnClicked);
            instructionsButton.onClick.RemoveListener(OnInstructionsBtnClicked);
            quitButton.onClick.RemoveListener(OnQuitBtnClicked);
        }
        #endregion

        #region Private Methods
        private void OnPlayBtnClicked()
        {
            MainMenuUIHandler.Instance.OpenMenu(Enums.MenuType.LevelSelectionMenu);
        }

        private void OnInstructionsBtnClicked()
        {
            MainMenuUIHandler.Instance.OpenMenu(Enums.MenuType.InstructionsMenu);
        }

        private void OnQuitBtnClicked()
        {
            GameManager.Instance.QuitGame();
        }
        #endregion
    }
}