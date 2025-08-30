using UnityEngine;
using TowerDefense.Enums;

namespace TowerDefense.UI.Handlers
{
    public class MainMenuUIHandler : MonoBehaviour
    {
        [SerializeField] private MenuManager _menuManager;

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
            _menuManager.OpenMenu(menuType);
        }
    }
}