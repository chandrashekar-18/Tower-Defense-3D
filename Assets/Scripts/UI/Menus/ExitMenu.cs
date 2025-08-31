using UnityEngine;
using UnityEngine.UI;
using TowerDefense.Core;

namespace TowerDefense.UI.Menus
{
    /// <summary>
    /// Exit menu UI for the game.
    /// </summary>
    public class ExitMenu : Menu
    {
        #region Variables
        [SerializeField] private Button yesBtn;
        [SerializeField] private Button noBtn;
        [SerializeField] private Button closeBtn;
        #endregion

        #region Base Methods
        public override void Open()
        {
            base.Open();
            yesBtn.onClick.AddListener(OnYesButtonClicked);
            noBtn.onClick.AddListener(OnNoButtonClicked);
            closeBtn.onClick.AddListener(OnNoButtonClicked);
        }

        public override void Close()
        {
            base.Close();
            yesBtn.onClick.RemoveListener(OnYesButtonClicked);
            noBtn.onClick.RemoveListener(OnNoButtonClicked);
            closeBtn.onClick.RemoveListener(OnNoButtonClicked);
        }
        #endregion

        #region Callbacks
        private void OnYesButtonClicked()
        {
            SceneLoader.Instance.LoadScene(GameConstants.MainMenuScene);
        }

        private void OnNoButtonClicked()
        {
            Close();
        }
        #endregion
    }
}