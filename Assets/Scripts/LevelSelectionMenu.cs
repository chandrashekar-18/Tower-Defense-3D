using UnityEngine;
using UnityEngine.UI;
using WordBoggle.Enums;
using WordBoggle.UI.Handlers;
using WordBoggle.UI.Components;
using System.Collections.Generic;
using TowerDefense.Level;
using TowerDefense.Core;

namespace WordBoggle.UI.Menus
{
    public class LevelSelectionMenu : Menu
    {
        [SerializeField] private GameObject levelPrefab;
        [SerializeField] private Transform levelsParent;
        [SerializeField] private Button closeButton;


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
    }
}