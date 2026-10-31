using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Настройки спавна")]
    public GameObject enemyPrefab; // Сюда перетаскиваем префаб врага
    public Transform player;       // Сюда перетаскиваем игрока

    [Tooltip("Время в секундах между появлением врагов")]
    public float spawnInterval = 5f;

    [Tooltip("Радиус круга вокруг игрока, где будут появляться враги")]
    public float spawnRadius = 10f;

    [Tooltip("Минимальная дистанция от игрока, чтобы враг не появился прямо на нем")]
    public float minDistanceFromPlayer = 5f;

    private float timer;

    void Start()
    {
        // Автоматически находим игрока, если он не назначен
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
            else Debug.LogError("EnemySpawner: Игрок не найден! Назначьте его вручную или поставьте тег 'Player'.");
        }

        timer = spawnInterval;
    }

    void Update()
    {
        if (player == null || enemyPrefab == null) return;

        // Тикает таймер
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnEnemy();
            timer = spawnInterval; // Сбрасываем таймер
        }
    }

    private void SpawnEnemy()
    {
        // 1. Выбираем случайное направление
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        // 2. Выбираем случайную дистанцию от игрока (между minDistance и spawnRadius)
        float randomDistance = Random.Range(minDistanceFromPlayer, spawnRadius);

        // 3. Вычисляем точку спавна
        Vector3 spawnPosition = player.position + new Vector3(randomDirection.x, 0f, randomDirection.y) * randomDistance;

        // 4. Проверяем, есть ли под этой точкой земля (чтобы враг не упал с неба)
        RaycastHit hit;
        if (Physics.Raycast(spawnPosition + Vector3.up * 10f, Vector3.down, out hit, 20f))
        {
            spawnPosition = hit.point; // Ставим врага ровно на поверхность

            // 5. Создаем врага
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            Debug.Log($"Спавн врага в точке: {spawnPosition}");
        }
        else
        {
            Debug.LogWarning("Не удалось найти землю под точкой спавна, враг не создан.");
        }
    }

    // Чтобы видеть радиус спавна в редакторе Unity
    private void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(player.position, spawnRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, minDistanceFromPlayer);
    }
}