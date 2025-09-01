using UnityEngine;
using TowerDefense.Towers;
using UnityEngine.UI;

namespace TowerDefense.UI.Components
{
    public class TowerHealthUI : MonoBehaviour
    {
        [Header("Health Bar")]
        [SerializeField] private Image healthBar;
        private Tower targetTower;

        public void Initialize(Tower tower)
        {
            targetTower = tower;
            targetTower.OnHealthChanged += UpdateHealthBar;

            UpdateHealthBar(tower, tower.CurrentHealth, tower.MaxHealth);
        }

        private void UpdateHealthBar(Tower tower, int currentHealth, int maxHealth)
        {
            float healthPercent = (float)currentHealth / maxHealth;
            healthBar.fillAmount = healthPercent;
        }

        private void OnDestroy()
        {
            targetTower.OnHealthChanged -= UpdateHealthBar;
        }
    }
}