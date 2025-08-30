using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TowerDefense.Core;

namespace TowerDefense.UI.Components
{
    public class LevelUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelNameText;
        [SerializeField] private Button playButton;

        private int level;

        private void OnEnable() => playButton.onClick.AddListener(OnPlayButtonClicked);
        private void OnDisable() => playButton.onClick.RemoveListener(OnPlayButtonClicked);

        public void SetUp(int level)
        {
            this.level = level;
            levelNameText.text = $"Level {level}";
        }

        private void OnPlayButtonClicked()
        {
            GameManager.Instance.StartGame(level);
        }
    }
}