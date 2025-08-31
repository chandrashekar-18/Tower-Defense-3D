using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TowerDefense.Core;

namespace TowerDefense.UI.Components
{
    /// <summary>
    /// Handles the UI display and interactions for a single level.
    /// </summary>
    public class LevelUI : MonoBehaviour
    {
        #region Variables
        [SerializeField] private TextMeshProUGUI levelNameText;
        [SerializeField] private Button playButton;

        private int level;
        #endregion

        #region Unity Lifecycle
        private void OnEnable() => playButton.onClick.AddListener(OnPlayButtonClicked);

        private void OnDisable() => playButton.onClick.RemoveListener(OnPlayButtonClicked);
        #endregion

        #region Public Methods
        public void SetUp(int level)
        {
            this.level = level;
            levelNameText.text = $"Level {level}";
        }
        #endregion

        #region Callbacks
        private void OnPlayButtonClicked()
        {
            GameManager.Instance.StartGame(level);
        }
        #endregion
    }
}
