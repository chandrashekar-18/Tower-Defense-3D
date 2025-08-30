using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TowerDefense.Core;
using TowerDefense.Towers;
using TowerDefense.UI.Handlers;

namespace TowerDefense.UI.Components
{
    /// <summary>
    /// UI button for selecting towers
    /// </summary>
    public class TowerButton : MonoBehaviour
    {
        #region Properties
        [SerializeField] private Image _towerIcon;
        [SerializeField] private TextMeshProUGUI _towerNameText;
        [SerializeField] private TextMeshProUGUI _towerCostText;
        [SerializeField] private Button _button;
        [SerializeField] private Image _selectionIndicator;

        private TowerData _towerData;

        public TowerData TowerData => _towerData;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            // Register for currency changes to update button interactability
            ResourceManager.OnCurrencyChanged += UpdateButtonInteractability;
            TowerController.OnTowerSelected += UpdateTowerInfo;
            _button.onClick.AddListener(OnButtonClicked);

            SetSelected(false);
        }

        private void OnDestroy()
        {
            ResourceManager.OnCurrencyChanged -= UpdateButtonInteractability;
            TowerController.OnTowerSelected -= UpdateTowerInfo;
            _button.onClick.RemoveListener(OnButtonClicked);
        }
        #endregion

        #region Public Methods
        public void Initialize(TowerData towerData)
        {
            _towerData = towerData;

            if (_towerNameText != null)
            {
                _towerNameText.text = towerData.TowerName;
            }

            if (_towerCostText != null)
            {
                _towerCostText.text = $"${towerData.Cost}";
            }

            if (_towerIcon != null && towerData.Icon != null)
            {
                _towerIcon.sprite = towerData.Icon;
                _towerIcon.preserveAspect = true;
            }

            UpdateButtonInteractability(ResourceManager.Instance.Currency);
        }

        public void SetSelected(bool isSelected)
        {
            if (_selectionIndicator != null)
            {
                _selectionIndicator.enabled = isSelected;
            }
        }
        #endregion

        #region Private Methods

        private void UpdateTowerInfo(TowerType towerType, TowerData towerData)
        {
            SetSelected(TowerData.TowerType == towerType);
        }

        private void OnButtonClicked()
        {
            // Select this tower
            GameplayUIHandler.Instance.SelectTower((int)TowerData.TowerType);

            // Start placement mode
            GameplayUIHandler.Instance.StartTowerPlacement();
        }

        private void UpdateButtonInteractability(int currentCurrency)
        {
            if (_button != null && _towerData != null)
            {
                _button.interactable = currentCurrency >= _towerData.Cost;
            }
        }
        #endregion
    }
}