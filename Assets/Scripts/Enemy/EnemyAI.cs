using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("Базовые статы (до множителей)")]
    public float baseMaxHealth = 50f;
    public float baseDamage = 10f;

    [Header("Настройки ИИ")]
    public float detectionRange = 20f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;

    [Header("Ссылки")]
    public Transform player;
    public EnemyView view;

    private NavMeshAgent agent;
    private EnemyModel model;
    private float lastAttackTime;
    private float currentDamage;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Если враг поставлен вручную (не через спавнер), инициализируем его со стандартными статами
        if (model == null)
        {
            Setup(1f, 1f);
        }

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    // Вызывается спавнером при создании
    public void Setup(float healthMultiplier, float damageMultiplier)
    {
        float finalHealth = baseMaxHealth * healthMultiplier;
        currentDamage = baseDamage * damageMultiplier;

        model = new EnemyModel(finalHealth);

        if (view != null)
        {
            model.OnHealthChanged += view.UpdateHealthDisplay;
            view.UpdateHealthDisplay(model.CurrentHealth, model.MaxHealth);
        }
    }

    private void Update()
    {
        if (model == null || model.IsDead || player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            agent.isStopped = true;
            TryAttack();

            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
        }
        else if (distanceToPlayer <= detectionRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.isStopped = true;
        }
    }

    private void TryAttack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;

            PlayerHealthController playerController = player.GetComponent<PlayerHealthController>();
            if (playerController != null)
            {
                playerController.TakeDamage(currentDamage);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (model == null || model.IsDead) return;

        model.TakeDamage(amount);
        if (view != null) view.PlayDamageEffect();

        if (model.IsDead)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Враг побежден!");
        agent.isStopped = true;
        GetComponent<Collider>().enabled = false;

        // Сообщаем спавнеру, что мы умерли
        EnemyHealthTracker tracker = GetComponent<EnemyHealthTracker>();
        if (tracker != null) tracker.ReportDeath();

        // НОВОЕ: Сообщаем игроку, что мы умерли
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerStats stats = player.GetComponent<PlayerStats>();
            if (stats != null) stats.RegisterKill();
        }

        Destroy(gameObject, 2f);
    }

    private void OnDestroy()
    {
        if (model != null && view != null)
            model.OnHealthChanged -= view.UpdateHealthDisplay;
    }
}