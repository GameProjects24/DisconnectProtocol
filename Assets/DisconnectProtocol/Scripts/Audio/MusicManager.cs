using DG.Tweening;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }
    public AudioSource musicSource;       // Источник музыки
    private float defaultVolume = 1f;      // Исходный уровень громкости

    private Tween fadeTween;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            defaultVolume = musicSource.volume;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Плавно уменьшает громкость до 0, затем ставит музыку на паузу
    public void FadeOutAndPause(float duration)
    {
        fadeTween?.Kill();
        fadeTween = musicSource.DOFade(0f, duration)
        .SetUpdate(UpdateType.Normal, true)
        .OnComplete(() =>
        {
            musicSource.Pause();
        });
    }

    // Возобновляет музыку и плавно повышает громкость до исходного уровня
    public void ResumeAndFadeIn(float duration)
    {
        musicSource.UnPause();
        fadeTween?.Kill();
        fadeTween = musicSource.DOFade(defaultVolume, duration)
        .SetUpdate(UpdateType.Normal, true);
    }

    // Плавно меняет музыку: затухает текущую, меняет клип, затем плавно увеличивает громкость
    public void ChangeMusic(AudioClip newClip, float fadeDuration)
    {
        if (newClip == null)
        {
            Debug.LogWarning("New AudioClip is null!");
            return;
        }
        fadeTween?.Kill();
        fadeTween = musicSource.DOFade(0f, fadeDuration)
        .SetUpdate(UpdateType.Normal, true)
        .OnComplete(() =>
        {
            musicSource.clip = newClip;
            musicSource.Play();
            musicSource.DOFade(defaultVolume, fadeDuration)
            .SetUpdate(UpdateType.Normal, true);
        });
    }

    // Плавное затухание (без паузы)
    public void FadeOut(float duration)
    {
        fadeTween?.Kill();
        fadeTween = musicSource.DOFade(0f, duration)
        .SetUpdate(UpdateType.Normal, true);
    }

    // Плавное включение (без паузы)
    public void FadeIn(float duration)
    {
        fadeTween?.Kill();
        fadeTween = musicSource.DOFade(defaultVolume, duration)
        .SetUpdate(UpdateType.Normal, true);
    }

    // Устанавливает зацикленность источника музыки
    public void SetLoop(bool loop)
    {
        musicSource.loop = loop;
    }
}