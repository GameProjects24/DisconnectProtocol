using UnityEngine;
using DisconnectProtocol;

public class Bullet : MonoBehaviour, IDamager
{
    public float lifeTime;
    public ParticleSystem hitEffect;
	public float damage { private get; set; }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Создаём искры в точке попадания
        if (hitEffect != null)
        {
            ParticleSystem effect = Instantiate(hitEffect, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
            
            // Удаляем частицы после завершения их проигрывания
            Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
        }
        // Удаляем пулю после столкновения
        Destroy(gameObject);
    }

    public void Fire(float power)
    {
        var body = GetComponent<Rigidbody>();
        body.linearVelocity = transform.forward * power;
    }

	public float Damage()
	{
		return damage;
	}
}
