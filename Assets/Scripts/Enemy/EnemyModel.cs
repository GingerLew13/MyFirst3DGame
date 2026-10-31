using System;
using UnityEngine;

public class EnemyModel
{
    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0;

    public event Action<float, float> OnHealthChanged;

    public EnemyModel(float maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (IsDead) return;
        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }
}