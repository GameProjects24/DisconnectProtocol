using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }
    public AudioSource sfxSource;

    // Cловарь для хранения звуков по ключу
    public List<SFXEntry> sfxEntries;

    private Dictionary<string, SFXEntry> sfxDict;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSFX();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSFX()
    {
        sfxDict = new Dictionary<string, SFXEntry>();
        foreach (var entry in sfxEntries)
        {
            if (!sfxDict.ContainsKey(entry.key))
                sfxDict.Add(entry.key, entry);
        }
    }

    public void PlaySound(string key, Vector3? position = null)
    {
        if (!sfxDict.TryGetValue(key, out SFXEntry entry))
        {
            Debug.LogWarning($"SFXManager: Не найден звук с ключом {key}");
            return;
        }

        // Выбираем один из клипов случайным образом
        AudioClip clip = entry.clips[Random.Range(0, entry.clips.Length)];

        if (position.HasValue)
        {
            // Если позиция указана, воспроизводим звук в пространстве
            AudioSource.PlayClipAtPoint(clip, position.Value, entry.volume);
        }
        else
        {
            // Если позиция не указана, воспроизводим звук через локальный AudioSource (например, для звуков рядом с игроком)
            sfxSource.PlayOneShot(clip, entry.volume);
        }
    }


    // Дополнительные методы для управления звуком: затухание, включение и т.д.
}

[System.Serializable]
public class SFXEntry
{
    public string key;           // Идентификатор (например, "footstep", "shoot", "reload", "pickup", "jump", "fall", "damage", "death")
    public AudioClip[] clips;    // Один или несколько клипов для случайного выбора
    public float volume = 1f;    // Громкость данного эффекта
}
