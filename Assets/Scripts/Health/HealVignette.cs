using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealVignette : MonoBehaviour
{
    [Header("Настройки эффекта")]
    public Image vignetteImage;
    public float fadeInSpeed = 5f;   // Медленнее, чем у урона
    public float fadeOutSpeed = 1.5f; // Медленнее исчезает
    public float maxAlpha = 0.4f;    // Менее насыщенный, чем урон

    private Coroutine fadeCoroutine;

    public void TriggerEffect()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeEffect());
    }

    private IEnumerator FadeEffect()
    {
        // 1. Плавно появляется
        float currentAlpha = vignetteImage.color.a;
        while (currentAlpha < maxAlpha)
        {
            currentAlpha += Time.deltaTime * fadeInSpeed;
            Color color = vignetteImage.color;
            color.a = currentAlpha;
            vignetteImage.color = color;
            yield return null;
        }

        // 2. Плавно исчезает
        while (currentAlpha > 0)
        {
            currentAlpha -= Time.deltaTime * fadeOutSpeed;
            Color color = vignetteImage.color;
            color.a = currentAlpha;
            vignetteImage.color = color;
            yield return null;
        }

        Color finalColor = vignetteImage.color;
        finalColor.a = 0;
        vignetteImage.color = finalColor;
    }
}