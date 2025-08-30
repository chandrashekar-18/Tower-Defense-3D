using UnityEngine;
using UnityEngine.UI;
using TowerDefense.Enums;
using TowerDefense.UI.Handlers;

namespace TowerDefense.UI.Menus
{
    public class InstructionsMenu : Menu
    {
        [SerializeField] private Button closeButton;

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

        private void OnCloseButtonClicked()
        {
            MainMenuUIHandler.Instance.OpenMenu(MenuType.MainMenu);
        }
    }
}