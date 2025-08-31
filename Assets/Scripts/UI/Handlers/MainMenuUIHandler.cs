using UnityEngine;
using TowerDefense.Enums;

namespace TowerDefense.UI.Handlers
{
    /// <summary>
    /// Handles the main menu UI interactions.
    /// </summary>
    public class MainMenuUIHandler : MonoBehaviour
    {
        [SerializeField] private MenuManager menuManager;

        public static MainMenuUIHandler Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            OpenMenu(MenuType.MainMenu);
        }

        public void OpenMenu(MenuType menuType)
        {
            menuManager.OpenMenu(menuType);
        }
    }
}