using UnityEngine;
using UnityEngine.UI;

public class EnemyView : MonoBehaviour
{
    [Header("UI (опционально)")]
    public Image healthBarFill; // Если у врага есть полоска здоровья над головой

    [Header("Эффекты")]
    public Renderer enemyRenderer;
    public Color damageColor = Color.red;
    private Color originalColor;
    private float flashTimer = 0f;

    private void Start()
    {
        if (enemyRenderer != null)
            originalColor = enemyRenderer.material.color;
    }

    public void UpdateHealthDisplay(float current, float max)
    {
        if (healthBarFill != null)
            healthBarFill.fillAmount = current / max;
    }

    public void PlayDamageEffect()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = damageColor;
            flashTimer = 0.2f; // Мигаем 0.2 секунды
        }
    }

    private void Update()
    {
        // Возвращаем цвет обратно
        if (flashTimer > 0)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0 && enemyRenderer != null)
                enemyRenderer.material.color = originalColor;
        }
    }
}