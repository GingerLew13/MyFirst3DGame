using UnityEngine;
using UnityEngine.SceneManagement; // Обязательно для работы со сценами

public class MainMenu : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Имя сцены, которая должна загрузиться при нажатии 'Играть'")]
    public string gameSceneName = "Game"; // <--- ВАЖНО: Напишите точное имя вашей игровой сцены!

    // Метод для кнопки "Играть"
    public void PlayGame()
    {
        // Загружаем игровую сцену
        SceneManager.LoadScene(gameSceneName);
    }

    // Метод для кнопки "Выйти"
    public void QuitGame()
    {
        Debug.Log("Игрок вышел из игры.");
        Application.Quit(); // Работает только в собранной игре (билде)

        // Эта строчка нужна только для того, чтобы в редакторе Unity вы тоже видели, что кнопка работает
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}