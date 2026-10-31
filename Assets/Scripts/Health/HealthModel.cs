using System;
using UnityEngine;

public class HealthModel
{
    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0;

    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath; // <--- НОВОЕ СОБЫТИЕ СМЕРТИ

    public HealthModel(float maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (IsDead) return;
        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        if (IsDead) OnDeath?.Invoke(); // Сообщаем о смерти
    }

    public void RestoreHealth(float healAmount)
    {
        if (IsDead) return;
        CurrentHealth = Mathf.Clamp(CurrentHealth + healAmount, 0, MaxHealth);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }
}