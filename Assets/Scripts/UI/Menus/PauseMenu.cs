using UnityEngine;
using UnityEngine.UI;
using TowerDefense.Core;
using TowerDefense.UI.Handlers;

namespace TowerDefense.UI.Menus
{
    public class PauseMenu : Menu
    {
        #region Variables
        [SerializeField] private Button resumeBtn;
        [SerializeField] private Button restartBtn;
        [SerializeField] private Button quitBtn;
        [SerializeField] private Button closeBtn;
        #endregion

        #region Base Methods    
        public override void Open()
        {
            base.Open();
            GameManager.Instance.PauseGame();
            resumeBtn.onClick.AddListener(OnResumeBtnClicked);
            restartBtn.onClick.AddListener(OnRestartBtnClicked);
            quitBtn.onClick.AddListener(OnQuitBtnClicked);
            closeBtn.onClick.AddListener(OnResumeBtnClicked);
        }

        public override void Close()
        {
            base.Close();
            GameManager.Instance.ResumeGame();
            resumeBtn.onClick.RemoveListener(OnResumeBtnClicked);
            restartBtn.onClick.RemoveListener(OnRestartBtnClicked);
            quitBtn.onClick.RemoveListener(OnQuitBtnClicked);
            closeBtn.onClick.RemoveListener(OnResumeBtnClicked);
        }
        #endregion

        #region Callbacks
        private void OnResumeBtnClicked()
        {
            Close();
        }

        private void OnRestartBtnClicked()
        {
            GameManager.Instance.RestartLevel();
        }

        private void OnQuitBtnClicked()
        {
            GameplayUIHandler.Instance.OpenMenu(Enums.MenuType.ExitMenu);
        }
        #endregion
    }
}