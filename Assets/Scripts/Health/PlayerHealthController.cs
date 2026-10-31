using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealthController : MonoBehaviour
{
    [Header("Настройки")]
    public float maxHealth = 100f;

    [Header("Ссылки на UI")]
    public HealthView healthView;
    public DamageVignette damageVignette;
    public HealVignette healVignette;

    private HealthModel model;

    // Событие, на которое подпишется GameManager
    public event System.Action OnPlayerDied;

    private void Start()
    {
        model = new HealthModel(maxHealth);

        if (healthView != null)
        {
            model.OnHealthChanged += healthView.UpdateHealthDisplay;
            healthView.UpdateHealthDisplay(model.CurrentHealth, model.MaxHealth);
        }

        // Подписываемся на смерть модели
        model.OnDeath += () => OnPlayerDied?.Invoke();
    }

    public void TakeDamage(float damage)
    {
        float healthBefore = model.CurrentHealth;
        model.TakeDamage(damage);

        if (model.CurrentHealth < healthBefore)
        {
            if (damageVignette != null) damageVignette.TriggerEffect();

            // ЗВУК УРОНА
            if (AudioManager.Instance != null) AudioManager.Instance.PlayPlayerHurt();
        }
    }

    // НОВЫЙ МЕТОД: Лечение (вызывается спавнером в конце волны)
    public void HealPercent(float percent)
    {
        float healAmount = model.MaxHealth * (percent / 100f);

        float healthBefore = model.CurrentHealth;
        model.RestoreHealth(healAmount);

        if (model.CurrentHealth > healthBefore)
        {
            if (healVignette != null) healVignette.TriggerEffect();

            // ЗВУК ЛЕЧЕНИЯ
            if (AudioManager.Instance != null) AudioManager.Instance.PlayPlayerHeal();
        }
    }

    public float GetCurrentHealth() => model.CurrentHealth;

    private void OnDestroy()
    {
        if (model != null && healthView != null)
            model.OnHealthChanged -= healthView.UpdateHealthDisplay;
    }
}