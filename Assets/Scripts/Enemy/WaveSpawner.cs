using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveSpawner : MonoBehaviour
{
    [Header("Настройки волн")]
    public GameObject enemyPrefab;
    public Transform player;
    public int baseEnemiesPerWave = 3;
    public float timeBetweenSpawns = 1f;
    public float timeBetweenWaves = 5f;

    [Header("Настройки спавна")]
    public float spawnRadius = 15f;
    public float minDistanceFromPlayer = 8f;

    [Header("Прогрессия сложности")]
    public int wavesForDifficultyIncrease = 5;
    public float statMultiplier = 1.5f;

    [Header("Награды за волну")]
    [Tooltip("Сколько секунд добавляется к общему таймеру в конце каждой волны")]
    public float timeBonusPerWave = 15f;

    [Tooltip("Сколько процентов здоровья восстанавливается в конце волны")]
    public float healPercentPerWave = 50f;

    private int currentWave = 0;
    private float currentHealthMultiplier = 1f;
    private float currentDamageMultiplier = 1f;
    private List<GameObject> activeEnemies = new List<GameObject>();

    private PlayerHealthController playerHealth;
    private bool isGameOver = false;

    // Событие для GameManager
    public event System.Action<float> OnWaveCompleted;

    void Start()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null) playerHealth = player.GetComponent<PlayerHealthController>();

        if (playerHealth != null)
        {
            playerHealth.OnPlayerDied += () => isGameOver = true;
        }

        StartCoroutine(WaveRoutine());
    }

    private IEnumerator WaveRoutine()
    {
        while (!isGameOver)
        {
            Debug.Log($"Отдых перед волной {currentWave + 1}...");
            yield return new WaitForSeconds(timeBetweenWaves);

            if (isGameOver) break; // Проверка на случай, если игрок умер во время отдыха

            currentWave++;
            Debug.Log($"=== НАЧАЛАСЬ ВОЛНА {currentWave} ===");

            if (AudioManager.Instance != null) AudioManager.Instance.PlayWaveStart();

            if (currentWave > 1 && (currentWave - 1) % wavesForDifficultyIncrease == 0)
            {
                currentHealthMultiplier *= statMultiplier;
                currentDamageMultiplier *= statMultiplier;
            }

            int enemiesThisWave = baseEnemiesPerWave + ((currentWave - 1) / wavesForDifficultyIncrease);

            for (int i = 0; i < enemiesThisWave; i++)
            {
                if (isGameOver) yield break;
                SpawnSingleEnemy();
                yield return new WaitForSeconds(timeBetweenSpawns);
            }

            // Ждем, пока всех убьют или игрок умрет
            yield return new WaitUntil(() => activeEnemies.Count == 0 || isGameOver);

            if (!isGameOver)
            {
                Debug.Log($"Волна {currentWave} пройдена! Награда: +{timeBonusPerWave} сек, +{healPercentPerWave}% HP");

                // Лечим игрока
                if (playerHealth != null) playerHealth.HealPercent(healPercentPerWave);

                // Сообщаем GameManager, что нужно добавить время
                OnWaveCompleted?.Invoke(timeBonusPerWave);
            }
        }
    }

    private void SpawnSingleEnemy()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minDistanceFromPlayer, spawnRadius);
        Vector3 spawnPosition = player.position + new Vector3(randomDirection.x, 0f, randomDirection.y) * randomDistance;

        if (Physics.Raycast(spawnPosition + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 20f))
        {
            GameObject newEnemy = Instantiate(enemyPrefab, hit.point, Quaternion.identity);
            activeEnemies.Add(newEnemy);

            EnemyAI enemyAI = newEnemy.GetComponent<EnemyAI>();
            if (enemyAI != null) enemyAI.Setup(currentHealthMultiplier, currentDamageMultiplier);

            EnemyHealthTracker tracker = newEnemy.AddComponent<EnemyHealthTracker>();
            tracker.OnEnemyDied += () => activeEnemies.Remove(newEnemy);
        }
    }
}