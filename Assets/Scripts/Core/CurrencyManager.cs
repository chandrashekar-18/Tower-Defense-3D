using TowerDefense.Level;
using UnityEngine;

namespace TowerDefense.Core
{
    /// <summary>
    /// Manages player resources like currency.
    /// </summary>
    public class CurrencyManager : MonoBehaviour
    {
        #region Singleton
        public static CurrencyManager Instance { get; private set; }
        #endregion

        #region Variables
        private int currency;
        #endregion

        #region Properties
        public int Currency => currency;
        #endregion
        
        #region Events
        public delegate void CurrencyChangedDelegate(int newAmount);
        public static event CurrencyChangedDelegate OnCurrencyChanged;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Initialize();
        }
        #endregion

        #region Public Methods
        public void Initialize()
        {
            currency = LevelManager.Instance.CurrentLevelData.StartingCurrency;
            Debug.Log($"Starting Currency: {currency}");
            OnCurrencyChanged?.Invoke(currency);
        }

        public bool CanAfford(int cost)
        {
            return currency >= cost;
        }

        public bool SpendCurrency(int amount)
        {
            if (!CanAfford(amount)) return false;

            currency -= amount;
            OnCurrencyChanged?.Invoke(currency);
            return true;
        }

        public void AddCurrency(int amount)
        {
            currency += amount;
            OnCurrencyChanged?.Invoke(currency);
        }
        #endregion
    }
}