using UnityEngine;
using TowerDefense.Enums;

namespace TowerDefense.UI.Menus
{
    public class Menu : MonoBehaviour
    {
        public MenuType menuType;
        public bool shouldClosePreviousMenu = true;
        public bool isOpen => gameObject.activeSelf;

        public virtual void Open() => gameObject.SetActive(true);
        public virtual void Close() => gameObject.SetActive(false);
    }
}