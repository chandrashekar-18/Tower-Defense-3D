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
        [SerializeField] private Image selectionIndicator;

        private TowerData towerData;
        #endregion

        #region Properties
        public TowerData TowerData => towerData;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            CurrencyManager.OnCurrencyChanged += UpdateButtonInteractability;
            TowerController.OnTowerSelected += UpdateTowerInfo;
            button.onClick.AddListener(OnButtonClicked);

            SetSelected(false);
        }

        private void OnDestroy()
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

            if (towerNameText != null)
            {
                towerNameText.text = towerData.TowerName;
            }

            if (towerCostText != null)
            {
                towerCostText.text = $"${towerData.Cost}";
            }

            if (towerIcon != null && towerData.Icon != null)
            {
                towerIcon.sprite = towerData.Icon;
                towerIcon.preserveAspect = true;
            }

            UpdateButtonInteractability(CurrencyManager.Instance.Currency);
        }

        public void SetSelected(bool isSelected)
        {
            if (selectionIndicator != null)
            {
                selectionIndicator.enabled = isSelected;
            }
        }
        #endregion

        #region Callbacks
        private void UpdateTowerInfo(string towerID, TowerData towerData)
        {
            SetSelected(TowerData.TowerID == towerID);
        }

        private void OnButtonClicked()
        {
            // Select this tower
            GameplayUIHandler.Instance.SelectTower(TowerData.TowerID);

            // Start placement mode
            GameplayUIHandler.Instance.StartTowerPlacement();
        }

        private void UpdateButtonInteractability(int currentCurrency)
        {
            if (button != null && towerData != null)
            {
                button.interactable = currentCurrency >= towerData.Cost;
            }
        }
        #endregion
    }
}