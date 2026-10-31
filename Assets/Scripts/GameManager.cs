using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Для перезагрузки сцены

public class GameManager : MonoBehaviour
{
    [Header("Ссылки")]
    public WaveSpawner waveSpawner;
    public PlayerHealthController playerHealth;
    public TextMeshProUGUI timerText;

    [Header("Настройки таймера")]
    [Tooltip("Начальное время в секундах")]
    public float startingTime = 60f;

    private float currentTime;
    private bool isGameOver = false;

    void Start()
    {
        currentTime = startingTime;

        // Подписываемся на события
        if (waveSpawner != null)
            waveSpawner.OnWaveCompleted += AddTime;

        if (playerHealth != null)
            playerHealth.OnPlayerDied += TriggerGameOver;
    }

    void Update()
    {
        if (isGameOver) return;

        // Уменьшаем время
        currentTime -= Time.deltaTime;

        // Обновляем UI
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(Mathf.Max(0, currentTime) / 60f);
            int seconds = Mathf.FloorToInt(Mathf.Max(0, currentTime) % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        // Проверка на проигрыш по времени
        if (currentTime <= 0)
        {
            TriggerGameOver();
        }
    }

    // Метод для добавления времени (вызывается из WaveSpawner)
    private void AddTime(float bonusSeconds)
    {
        if (isGameOver) return;
        currentTime += bonusSeconds;
        Debug.Log($"Получено время: +{bonusSeconds} сек. Текущее время: {currentTime}");
    }

    // Метод для окончания игры
    private void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("ИГРА ОКОНЧЕНА!");
        if (timerText != null) timerText.text = "GAME OVER";

        // Размораживаем время, чтобы сцена могла загрузиться
        Time.timeScale = 1f;

        // Загружаем меню через 3 секунды
        StartCoroutine(ReturnToMenuRoutine());
    }

    private System.Collections.IEnumerator ReturnToMenuRoutine()
    {
        yield return new WaitForSeconds(3f); // Ждем 3 секунды, пока игрок читает "GAME OVER"
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu"); // Загружаем меню
    }
}