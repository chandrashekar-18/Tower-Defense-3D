using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TowerDefense.Core
{
    /// <summary>
    /// Manages player resources like currency
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

        #region Properties
        [SerializeField] private int _currency = 0;
        public int Currency => _currency;
        #endregion

        #region Events
        public delegate void CurrencyChangedDelegate(int newAmount);
        public static event CurrencyChangedDelegate OnCurrencyChanged;
        #endregion

        #region Public Methods
        public void Initialize()
        {
            _currency = 300; // Starting currency
            OnCurrencyChanged?.Invoke(_currency);
        }
        
        public bool CanAfford(int cost)
        {
            return _currency >= cost;
        }
        
        public bool SpendCurrency(int amount)
        {
            if (!CanAfford(amount)) return false;
            
            _currency -= amount;
            OnCurrencyChanged?.Invoke(_currency);
            return true;
        }
        
        public void AddCurrency(int amount)
        {
            _currency += amount;
            OnCurrencyChanged?.Invoke(_currency);
        }
        #endregion
    }
}