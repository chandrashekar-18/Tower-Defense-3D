using TowerDefense.Core;
using UnityEngine;
using UnityEngine.UI;
using WordBoggle.UI.Handlers;

namespace WordBoggle.UI.Menus
{
    public class MainMenu : Menu
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button instructionsButton;
        [SerializeField] private Button quitButton;

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
    }
}