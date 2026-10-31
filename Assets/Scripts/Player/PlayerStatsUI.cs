using UnityEngine;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("Ссылки")]
    public PlayerStats playerStats;
    public TextMeshProUGUI killCounterText;

    private void Start()
    {
        if (playerStats != null)
        {
            // Подписываемся на улучшение
            playerStats.OnUpgrade += HandleUpgrade;

            // Обновляем текст при старте
            UpdateUI();
        }
    }

    private void Update()
    {
        // Обновляем текст каждый кадр (чтобы видеть, как растет счетчик)
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (killCounterText != null && playerStats != null)
        {
            int remaining = playerStats.killsPerUpgrade - playerStats.currentKills;
            killCounterText.text = $"Убийств: {playerStats.currentKills} / {playerStats.killsPerUpgrade}\nДо улучшения: {remaining}";
        }
    }

    private void HandleUpgrade(int newLevel)
    {
        // Здесь можно добавить эффект (вспышку), когда игрок получает улучшение
        Debug.Log($"Игрок достиг уровня {newLevel}!");
    }

    private void OnDestroy()
    {
        if (playerStats != null)
            playerStats.OnUpgrade -= HandleUpgrade;
    }
}