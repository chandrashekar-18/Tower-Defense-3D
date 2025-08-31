using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TowerDefense.Core;

namespace TowerDefense.UI.Menus
{
    /// <summary>
    /// Game over menu UI for the game.
    /// </summary>
    public class GameOverMenu : Menu
    {
        #region Variables
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Image resultImage;
        [SerializeField] private Button menuBtn;
        [SerializeField] private Button restartBtn;
        [SerializeField] private Sprite victoryIcon;
        [SerializeField] private Sprite defeatIcon;
        #endregion

        #region Base Methods
        public override void Open()
        {
            base.Open();
            menuBtn.onClick.AddListener(OnMenuBtnClicked);
            restartBtn.onClick.AddListener(OnRestartBtnClicked);
        }

        public override void Close()
        {
            base.Close();
            menuBtn.onClick.RemoveListener(OnMenuBtnClicked);
            restartBtn.onClick.RemoveListener(OnRestartBtnClicked);
        }
        #endregion

        #region Public Methods
        public void Initialize(bool isVictory)
        {
            titleText.text = isVictory ? "Victory!" : "Defeat!";
            scoreText.text = "Score: " + ScoreManager.Instance.CurrentScore;
            resultImage.sprite = isVictory ? victoryIcon : defeatIcon;
        }
        #endregion

        #region Callbacks
        private void OnMenuBtnClicked()
        {
            GameManager.Instance.ReturnToMainMenu();
        }

        private void OnRestartBtnClicked()
        {
            GameManager.Instance.RestartLevel();
        }
        #endregion
    }
}