using UnityEngine;
using System;

public class EnemyHealthTracker : MonoBehaviour
{
    public event Action OnEnemyDied;
    private bool isDead = false;

    public void ReportDeath()
    {
        if (isDead) return;
        isDead = true;
        OnEnemyDied?.Invoke();
    }
}