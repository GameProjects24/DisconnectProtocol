using UnityEditor.UI;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 5f;
    public ParticleSystem hitEffect;
    public float hitEffectDuration = 2f; // Длительность отображения искр

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Создаём искры в точке попадания
        if (hitEffect != null)
        {
            // Создание системы частиц
            ParticleSystem effect = Instantiate(hitEffect, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
            
            // Удаляем частицы после завершения их проигрывания
            Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
        }

        // Удаляем пулю после столкновения
        Destroy(gameObject);
    }
}
