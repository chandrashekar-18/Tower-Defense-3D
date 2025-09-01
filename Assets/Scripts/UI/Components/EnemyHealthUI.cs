using UnityEngine;
using UnityEngine.UI;
using TowerDefense.Enemies;

namespace TowerDefense.UI.Components
{
    public class EnemyHealthUI : MonoBehaviour
    {
        [Header("Health Bar")]
        [SerializeField] private Image fillBar;
        private Enemy targetEnemy;

        public void Initialize(Enemy enemy)
        {
            targetEnemy = enemy;
            targetEnemy.OnHealthChanged += UpdateHealthBar;
            targetEnemy.OnDefeated += HandleEnemyDefeated;
        }

        private void OnDestroy()
        {
            targetEnemy.OnHealthChanged -= UpdateHealthBar;
            targetEnemy.OnDefeated -= HandleEnemyDefeated;
        }

        private void UpdateHealthBar(int currentHealth, int maxHealth)
        {
            float healthPercent = (float)currentHealth / maxHealth;
            fillBar.fillAmount = healthPercent;
        }

        private void HandleEnemyDefeated(Enemy enemy)
        {
            Destroy(gameObject);
        }
    }
}