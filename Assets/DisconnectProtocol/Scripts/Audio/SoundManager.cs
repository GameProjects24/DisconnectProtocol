using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    public AudioMixer audioMixer; // Главный аудиомикшер
    private float defaultSFXVolume = 1f; // Исходная громкость эффектов
    private Tween fadeTween;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioMixer.GetFloat("SFXVolume", out defaultSFXVolume);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Плавно уменьшает громкость всех звуков
    public void FadeOutSFX(float duration)
    {
        fadeTween?.Kill();
        fadeTween = DOTween.To(
            () => GetSFXVolume(),
            x => SetSFXVolume(x),
            -80f, // Полное заглушение звуков
            duration
        ).SetUpdate(UpdateType.Normal, true);
    }

    // Плавно возвращает громкость звуков к исходному значению
    public void FadeInSFX(float duration)
    {
        fadeTween?.Kill();
        fadeTween = DOTween.To(
            () => GetSFXVolume(),
            x => SetSFXVolume(x),
            defaultSFXVolume,
            duration
        ).SetUpdate(UpdateType.Normal, true);
    }

    // Устанавливает громкость звуков
    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
    }

    // Получает текущую громкость звуков
    private float GetSFXVolume()
    {
        audioMixer.GetFloat("SFXVolume", out float volume);
        return volume;
    }

    // Восстанавливаем первоначальные настройки громкости эффектов
    public void ResetSFXVolume()
    {
        audioMixer.SetFloat("SFXVolume", defaultSFXVolume);
    }
}
