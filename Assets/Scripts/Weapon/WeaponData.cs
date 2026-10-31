using UnityEngine;

public class WeaponData : MonoBehaviour
{
    [Header("Характеристики")]
    public float damage = 25f;
    public float attackRange = 2.5f;
    public float attackRate = 1.5f; // Атак в секунду

    [Header("Слои (LayerMask)")]
    public LayerMask enemyLayer; // Сюда нужно добавить слой "Enemy"

    [HideInInspector] public float nextAttackTime = 0f;
}