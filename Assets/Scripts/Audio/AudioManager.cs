using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Источники звука")]
    public AudioSource musicSource;

    [Header("Звуковые эффекты (SFX)")]
    public AudioClip swordSwingClip;
    public AudioClip enemyHitClip;
    public AudioClip playerHurtClip;
    public AudioClip playerHealClip;
    public AudioClip waveStartClip;

    [Header("Музыка")]
    public AudioClip backgroundMusic;

    [Header("Настройки громкости музыки")]
    [Range(0f, 1f)] public float startMusicVolume = 1f;
    [Range(0f, 1f)] public float targetMusicVolume = 0.3f;
    public float musicFadeOutDuration = 5f;

    [Header("Настройки питча (Pitch)")]
    public float basePitch = 1f;
    public float playerHurtPitchVariation = 0.2f;
    public float enemyHitPitchVariation = 0.15f;

    // --- НАСТРОЙКИ ФЕЙДА ДЛЯ КАЖДОГО ЗВУКА ---
    [Header("Настройки фейда для SFX (0 = без фейда)")]
    [Tooltip("Время затухания для звука взмаха мечом")]
    public float swordSwingFadeOut = 0f;

    [Tooltip("Время затухания для звука попадания по врагу")]
    public float enemyHitFadeOut = 0f;

    [Tooltip("Время затухания для звука урона игрока")]
    public float playerHurtFadeOut = 0.3f;

    [Tooltip("Время затухания для звука лечения")]
    public float playerHealFadeOut = 0.5f;

    [Tooltip("Время затухания для звука начала волны")]
    public float waveStartFadeOut = 0f;

    // Пул AudioSource для SFX (чтобы не создавать новые каждый раз)
    private List<AudioSource> sfxPool = new List<AudioSource>();
    private const int POOL_SIZE = 10;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Создаем пул AudioSource
        for (int i = 0; i < POOL_SIZE; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            sfxPool.Add(source);
        }
    }

    private void Start()
    {
        if (backgroundMusic != null && musicSource != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.volume = startMusicVolume;
            musicSource.Play();
            StartCoroutine(FadeOutMusic());
        }
    }

    private IEnumerator FadeOutMusic()
    {
        float t = 0f;
        while (t < musicFadeOutDuration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startMusicVolume, targetMusicVolume, t / musicFadeOutDuration);
            yield return null;
        }
        musicSource.volume = targetMusicVolume;
    }

    // --- ВСПОМОГАТЕЛЬНЫЙ МЕТОД ДЛЯ ВОСПРОИЗВЕДЕНИЯ С ФЕЙДОМ ---
    private void PlaySound(AudioClip clip, float pitchVariation, float fadeOutDuration)
    {
        if (clip == null) return;

        // Берем свободный AudioSource из пула
        AudioSource source = GetFreeSource();
        if (source == null) return; // Если пул переполнен — пропускаем звук

        // Настраиваем питч
        float randomPitch = basePitch + Random.Range(-pitchVariation, pitchVariation);
        source.pitch = randomPitch;
        source.clip = clip;
        source.volume = 1f; // Начинаем с полной громкости

        source.Play();

        // Если нужен фейд — запускаем корутину
        if (fadeOutDuration > 0f)
        {
            StartCoroutine(FadeOutSource(source, fadeOutDuration));
        }
    }

    // Корутина для затухания конкретного AudioSource
    private IEnumerator FadeOutSource(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        source.Stop();
        source.volume = 1f; // Возвращаем громкость для следующего использования
    }

    // Получаем свободный AudioSource из пула
    private AudioSource GetFreeSource()
    {
        foreach (var source in sfxPool)
        {
            if (!source.isPlaying)
                return source;
        }
        return null; // Все заняты
    }

    // --- МЕТОДЫ ВОСПРОИЗВЕДЕНИЯ ---

    public void PlaySwordSwing() => PlaySound(swordSwingClip, 0.1f, swordSwingFadeOut);
    public void PlayEnemyHit() => PlaySound(enemyHitClip, enemyHitPitchVariation, enemyHitFadeOut);
    public void PlayPlayerHurt() => PlaySound(playerHurtClip, playerHurtPitchVariation, playerHurtFadeOut);
    public void PlayPlayerHeal() => PlaySound(playerHealClip, 0.1f, playerHealFadeOut);
    public void PlayWaveStart() => PlaySound(waveStartClip, 0f, waveStartFadeOut);
}