using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TowerDefense.Core;
using TowerDefense.Towers;
using TowerDefense.UI.Handlers;

namespace TowerDefense.UI.Components
{
    /// <summary>
    /// UI button for selecting towers.
    /// </summary>
    public class TowerButton : MonoBehaviour
    {
        #region Variables
        [SerializeField] private Image towerIcon;
        [SerializeField] private TextMeshProUGUI towerNameText;
        [SerializeField] private TextMeshProUGUI towerCostText;
        [SerializeField] private Button button;
        [SerializeField] private GameObject selectionIndicator;

        private TowerData towerData;
        #endregion

        #region Properties
        public TowerData TowerData => towerData;
        #endregion

        #region Unity Lifecycle
        private void OnEnable()
        {
            CurrencyManager.OnCurrencyChanged += UpdateButtonInteractability;
            TowerController.OnTowerSelected += UpdateTowerInfo;
            button.onClick.AddListener(OnButtonClicked);

            SetSelected(false);
        }

        private void OnDisable()
        {
            CurrencyManager.OnCurrencyChanged -= UpdateButtonInteractability;
            TowerController.OnTowerSelected -= UpdateTowerInfo;
            button.onClick.RemoveListener(OnButtonClicked);
        }
        #endregion

        #region Public Methods
        public void Initialize(TowerData towerData)
        {
            this.towerData = towerData;

            towerNameText.text = towerData.TowerName;
            towerCostText.text = $"${towerData.Cost}";

            if (towerIcon != null && towerData.Icon != null)
            {
                towerIcon.sprite = towerData.Icon;
                towerIcon.preserveAspect = true;
            }

            UpdateButtonInteractability(CurrencyManager.Instance.Currency);
        }

        public void SetSelected(bool isSelected)
        {
            selectionIndicator.SetActive(isSelected);
        }
        #endregion

        #region Callbacks
        private void UpdateTowerInfo(string towerID, TowerData towerData)
        {
            SetSelected(TowerData.TowerID == towerID);
        }

        private void OnButtonClicked()
        {
            GameplayUIHandler.Instance.SelectTower(TowerData.TowerID);
            GameplayUIHandler.Instance.StartTowerPlacement();
        }

        private void UpdateButtonInteractability(int currentCurrency)
        {
            Debug.Log($"Current Currency: {currentCurrency}, Tower Cost: {towerData.Cost}");
            button.interactable = currentCurrency >= towerData.Cost;
        }
        #endregion
    }
}