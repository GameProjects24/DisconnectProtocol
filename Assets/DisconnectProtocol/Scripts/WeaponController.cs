using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public WeaponData currentWeapon;
    private float lastShootTime;

    public void Shoot()
    {
        if (Time.time > lastShootTime + 1f / currentWeapon.fireRate)
        {
            // Логика выстрела
            Debug.Log($"Shooting with {currentWeapon.weaponName}, Damage: {currentWeapon.damage}");
            lastShootTime = Time.time;
        }
    }

    public void Reload()
    {
        // Логика перезарядки
        Debug.Log($"Reloading {currentWeapon.weaponName}");
    }

    public void EquipWeapon(WeaponData newWeapon)
    {
        currentWeapon = newWeapon;
        // Замена визуальной модели оружия
        Debug.Log($"Equipped {newWeapon.weaponName}");
    }
}
