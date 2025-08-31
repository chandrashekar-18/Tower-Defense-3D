using UnityEngine;
using UnityEngine.UI;
using TowerDefense.Enums;
using TowerDefense.UI.Handlers;

namespace TowerDefense.UI.Menus
{
    /// <summary>
    /// Instructions menu UI for the game.
    /// </summary>
    public class InstructionsMenu : Menu
    {
        #region Variables
        [SerializeField] private Button closeButton;
        #endregion

        #region Base Methods
        public override void Open()
        {
            base.Open();
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }

        public override void Close()
        {
            base.Close();
            closeButton.onClick.RemoveListener(OnCloseButtonClicked);
        }
        #endregion

        #region Private Methods
        private void OnCloseButtonClicked()
        {
            MainMenuUIHandler.Instance.OpenMenu(MenuType.MainMenu);
        }
        #endregion
    }
}