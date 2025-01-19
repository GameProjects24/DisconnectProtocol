using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/WeaponShootBulletSO", fileName = "WeaponShootBulletSO")]
public class WeaponShootBulletSO : WeaponShootSO
{
    public Bullet prefab;
    public float lifeTime = 2f;
    public float power = 10f;

    public override void Shoot(Vector3 position, Vector3 direction)
    {
        var bullet = Instantiate(prefab, position, Quaternion.LookRotation(direction));
        bullet.lifeTime = lifeTime;
        bullet.Fire(power);
    }
}
