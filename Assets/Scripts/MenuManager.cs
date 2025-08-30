using UnityEngine;
using WordBoggle.Enums;
using System.Collections.Generic;

namespace WordBoggle.UI
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private Menu[] menus;

        private Dictionary<MenuType, Menu> _menuDict;

        private void Awake()
        {
            _menuDict = new Dictionary<MenuType, Menu>();
            foreach (var menu in menus)
            {
                _menuDict.TryAdd(menu.menuType, menu);
            }
        }

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
            if (_menuDict.TryGetValue(menuType, out var menu) && menu is T typed)
                return typed;

            return null;
        }

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
    }
}