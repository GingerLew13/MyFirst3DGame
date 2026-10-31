using UnityEngine;
using UnityEngine.UI;
using TMPro; // <--- Обязательно добавьте это, если используете TextMeshPro

public class HealthView : MonoBehaviour
{
    [Header("UI Ссылки")]
    public Image frontHealthBar;
    public Image backHealthBar;
    public TextMeshProUGUI healthText; // <--- НОВАЯ ССЫЛКА (или public Text healthText;)

    [Header("Настройки анимации")]
    public float chipSpeed = 2f;

    private float targetFill = 1f;
    private float frontFill = 1f;
    private float backFill = 1f;
    private float lerpTimer;

    // Этот метод вызывается из контроллера при изменении здоровья
    public void UpdateHealthDisplay(float currentHealth, float maxHealth)
    {
        targetFill = currentHealth / maxHealth;
        lerpTimer = 0f;

        // ОБНОВЛЯЕМ ТЕКСТ
        if (healthText != null)
        {
            healthText.text = $"{Mathf.RoundToInt(currentHealth)} / {Mathf.RoundToInt(maxHealth)}";
        }
    }

    private void Update()
    {
        // ... (Остальной код анимации полосок остается без изменений)
        if (Mathf.Abs(backFill - targetFill) > 0.001f)
        {
            backFill = Mathf.MoveTowards(backFill, targetFill, (1f / chipSpeed) * Time.deltaTime);
            backHealthBar.fillAmount = backFill;
        }

        if (Mathf.Abs(frontFill - targetFill) > 0.001f)
        {
            if (frontFill > targetFill)
            {
                frontFill = targetFill;
                frontHealthBar.fillAmount = frontFill;
                backHealthBar.color = Color.red;
            }
            else if (frontFill < targetFill)
            {
                backFill = targetFill;
                backHealthBar.fillAmount = backFill;
                backHealthBar.color = Color.green;

                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / chipSpeed;
                percentComplete = percentComplete * percentComplete;
                frontFill = Mathf.Lerp(frontFill, targetFill, percentComplete);
                frontHealthBar.fillAmount = frontFill;
            }
        }
    }
}