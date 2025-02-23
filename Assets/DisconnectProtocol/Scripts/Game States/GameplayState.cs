using UnityEngine;
using System.Collections;
using DisconnectProtocol;

public class GameplayState : MonoBehaviour
{
    [Header("State References")]
    [Tooltip("Объект, отвечающий за состояние смерти (экран смерти с анимациями и кнопками)")]
    public GameObject deathStateObject;
    
    [Tooltip("Скрипт игрока, в котором вызывается событие OnDie")]
    public Damageable damageble;
    
    private void OnEnable()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        // Подписываемся на событие смерти игрока
        if (damageble != null)
        {
            damageble.OnDie += OnPlayerDie;
        }
    }
    
    private void OnDisable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        // Отписываемся от события смерти
        if (damageble != null)
        {
            damageble.OnDie -= OnPlayerDie;
        }
    }
    
    private void Start()
    {
        Time.timeScale = 1f;
    }
    
    private void OnPlayerDie()
    {
        Time.timeScale = 0f;

        if (deathStateObject != null)
        {
            deathStateObject.SetActive(true);
        }
        
        gameObject.SetActive(false);
    }
}
