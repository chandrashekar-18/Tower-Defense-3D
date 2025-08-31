using UnityEngine;
using TowerDefense.Enums;
using TowerDefense.UI.Menus;
using System.Collections.Generic;

namespace TowerDefense.UI
{
    /// <summary>
    /// Manages opening and closing of UI menus.
    /// </summary>
    public class MenuManager : MonoBehaviour
    {
        #region Variables
        [SerializeField] private Menu[] menus;

        private Dictionary<MenuType, Menu> menuDict;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            menuDict = new Dictionary<MenuType, Menu>();
            foreach (var menu in menus)
            {
                menuDict.TryAdd(menu.menuType, menu);
            }
        }

        private void OnDestroy()
        {
            foreach (var menu in menus)
            {
                CloseMenu(menu);
            }
        }
        #endregion

        #region Public Methods
        public void OpenMenu(MenuType menuType)
        {
            foreach (var menu in menus)
            {
                if (menu.menuType == menuType)
                    menu.Open();
                else if (menu.isOpen && menu.shouldClosePreviousMenu)
                    menu.Close();
            }
        }

        public T GetMenu<T>(MenuType menuType) where T : Menu
        {
            if (menuDict.TryGetValue(menuType, out var menu) && menu is T typed)
                return typed;

            return null;
        }
        #endregion

        #region Private Methods
        private void OpenMenu(Menu menuToOpen)
        {
            foreach (var menu in menus)
            {
                if (menu.isOpen)
                    CloseMenu(menu);
            }

            menuToOpen.Open();
        }

        private void CloseMenu(Menu menu)
        {
            menu.Close();
        }
        #endregion
    }
}