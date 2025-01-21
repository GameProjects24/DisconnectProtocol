using UnityEngine;

public class WeaponShootBullet : MonoBehaviour, IWeaponShoot
{
	public Bullet prefab;
	public float power = 10f;

	public void Shoot(Vector3 position, Vector3 direction, float damage)
	{
		var bullet = Instantiate(prefab, position, Quaternion.LookRotation(direction));
		bullet.damage = damage;
		bullet.Fire(power);
	}
}
