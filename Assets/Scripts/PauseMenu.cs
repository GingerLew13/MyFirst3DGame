using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Для Slider
using TMPro; // Для TextMeshPro
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Ссылки")]
    public GameObject pauseMenuPanel; // Сам объект PauseMenu
    public Slider volumeSlider;       // Ползунок громкости

    [Header("Настройки")]
    public string mainMenuSceneName = "MainMenu"; // Имя сцены меню

    private bool isPaused = false;

    private void Start()
    {
        // Принудительно возвращаем время и громкость в норму
        Time.timeScale = 1f;
        AudioListener.volume = 1f;

        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
    }

    private void Update()
    {
        // Проверка нажатия клавиши Escape (или P)
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    private void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f; // Останавливаем время в игре

        // Разблокируем курсор мыши (чтобы кликать по кнопкам)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Если у вас есть скрипт PlayerLook, его нужно отключить, чтобы камера не крутилась
        PlayerLook playerLook = FindObjectOfType<PlayerLook>();
        if (playerLook != null) playerLook.enabled = false;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f; // Возвращаем время

        // Блокируем курсор обратно (для FPS)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayerLook playerLook = FindObjectOfType<PlayerLook>();
        if (playerLook != null) playerLook.enabled = true;
    }

    // Метод для ползунка громкости
    public void OnVolumeChanged(float value)
    {
        // Меняем общую громкость игры
        AudioListener.volume = value;
    }

    // Метод для кнопки "В главное меню"
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Обязательно возвращаем время перед загрузкой сцены!
        SceneManager.LoadScene(mainMenuSceneName);
    }
}