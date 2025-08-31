using UnityEngine;

namespace TowerDefense.Core
{
    /// <summary>
    /// Manages player resources like currency.
    /// </summary>
    public class ResourceManager : MonoBehaviour
    {
        #region Singleton
        public static ResourceManager Instance { get; private set; }

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
        #endregion

        #region Variables
        [SerializeField] private int currency = 0;
        public int Currency => currency;
        #endregion

        #region Events
        public delegate void CurrencyChangedDelegate(int newAmount);
        public static event CurrencyChangedDelegate OnCurrencyChanged;
        #endregion

        #region Public Methods
        public void Initialize()
        {
            currency = 300; // Starting currency
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