using UnityEngine;
using UnityEngine.UI;
using TowerDefense.Level;
using TowerDefense.Enums;
using TowerDefense.UI.Handlers;
using TowerDefense.UI.Components;
using System.Collections.Generic;

namespace TowerDefense.UI.Menus
{
    /// <summary>
    /// Menu for selecting levels.
    /// </summary>
    public class LevelSelectionMenu : Menu
    {
        #region Variables
        [SerializeField] private GameObject levelPrefab;
        [SerializeField] private Transform levelsParent;
        [SerializeField] private Button closeButton;
        #endregion

        #region Base Methods
        public override void Open()
        {
            base.Open();
            closeButton.onClick.AddListener(OnCloseButtonClicked);
            SpawnLevels(LevelManager.Instance.AllLevels);
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

        private void SpawnLevels(List<LevelData> allLevels)
        {
            foreach (Transform child in levelsParent) Destroy(child.gameObject);

            for (int i = 0; i < allLevels.Count; i++)
            {
                GameObject obj = Instantiate(levelPrefab, levelsParent);
                LevelUI levelUI = obj.GetComponent<LevelUI>();
                levelUI.SetUp(allLevels[i].LevelNumber);
            }
        }
        #endregion
    }
}