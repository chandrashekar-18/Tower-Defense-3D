using UnityEngine;
using UnityEngine.UI;
using TowerDefense.Core;

namespace TowerDefense.UI.Menus
{
    public class ExitMenu : Menu
    {
        [SerializeField] private Button _yesBtn;
        [SerializeField] private Button _noBtn;
        [SerializeField] private Button _closeBtn;

        public override void Open()
        {
            base.Open();
            _yesBtn.onClick.AddListener(OnYesButtonClicked);
            _noBtn.onClick.AddListener(OnNoButtonClicked);
            _closeBtn.onClick.AddListener(OnNoButtonClicked);
        }

        public override void Close()
        {
            base.Close();
            _yesBtn.onClick.RemoveListener(OnYesButtonClicked);
            _noBtn.onClick.RemoveListener(OnNoButtonClicked);
            _closeBtn.onClick.RemoveListener(OnNoButtonClicked);
        }

        private void OnYesButtonClicked()
        {
            SceneLoader.Instance.LoadScene(GameConstants.MAIN_MENU_SCENE);
        }

        private void OnNoButtonClicked()
        {
            Close();
        }
    }
}