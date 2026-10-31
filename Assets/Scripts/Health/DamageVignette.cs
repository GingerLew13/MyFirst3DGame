using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageVignette : MonoBehaviour
{
    [Header("Настройки эффекта")]
    public Image vignetteImage;
    public float fadeInSpeed = 10f;
    public float fadeOutSpeed = 2f;
    public float maxAlpha = 0.6f; // Максимальная непрозрачность красного

    private Coroutine fadeCoroutine;

    // Этот метод мы будем вызывать из контроллера при получении урона
    public void TriggerEffect()
    {
        // Если эффект уже играет, перезапускаем его
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeEffect());
    }

    private IEnumerator FadeEffect()
    {
        // 1. Резко краснеет (Fade In)
        float currentAlpha = vignetteImage.color.a;
        while (currentAlpha < maxAlpha)
        {
            currentAlpha += Time.deltaTime * fadeInSpeed;
            Color color = vignetteImage.color;
            color.a = currentAlpha;
            vignetteImage.color = color;
            yield return null;
        }

        // 2. Плавно исчезает (Fade Out)
        while (currentAlpha > 0)
        {
            currentAlpha -= Time.deltaTime * fadeOutSpeed;
            Color color = vignetteImage.color;
            color.a = currentAlpha;
            vignetteImage.color = color;
            yield return null;
        }

        // На всякий случай фиксируем прозрачность в 0
        Color finalColor = vignetteImage.color;
        finalColor.a = 0;
        vignetteImage.color = finalColor;
    }
}