using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Настройки улучшений")]
    [Tooltip("Сколько врагов нужно убить для улучшения")]
    public int killsPerUpgrade = 5;

    [Tooltip("Во сколько раз увеличиваются статы (1.2 = +20%)")]
    public float statMultiplier = 1.2f;

    [Header("Текущий прогресс")]
    public int currentKills = 0;
    public int upgradeLevel = 1; // Уровень улучшения (начинается с 1)

    // Событие, чтобы UI мог обновиться (например, показать "Уровень 2")
    public event System.Action<int> OnUpgrade;

    // Метод, который вызывается, когда враг умирает
    public void RegisterKill()
    {
        currentKills++;
        Debug.Log($"Убито врагов: {currentKills} / {killsPerUpgrade}");

        if (currentKills >= killsPerUpgrade)
        {
            UpgradeStats();
        }
    }

    private void UpgradeStats()
    {
        currentKills = 0; // Сбрасываем счетчик
        upgradeLevel++;

        // Применяем множитель к игроку
        ApplyStatsToPlayer();

        Debug.Log($"=== УЛУЧШЕНИЕ! Уровень {upgradeLevel}. Статы умножены на {statMultiplier} ===");

        // Оповещаем UI
        OnUpgrade?.Invoke(upgradeLevel);
    }

    private void ApplyStatsToPlayer()
    {
        // 1. Увеличиваем урон оружия
        PlayerCombat combat = GetComponent<PlayerCombat>();
        if (combat != null && combat.equippedWeapon != null)
        {
            combat.equippedWeapon.damage *= statMultiplier;
        }

        // 2. Увеличиваем максимальное здоровье (опционально)
        PlayerHealthController health = GetComponent<PlayerHealthController>();
        if (health != null)
        {
            // Увеличиваем максимальное здоровье и лечим на столько же
            float extraHealth = health.maxHealth * (statMultiplier - 1f);
            health.maxHealth *= statMultiplier;
            health.HealPercent(100f); // Лечим до полного (или можно добавить метод AddMaxHealth)
        }
    }
}