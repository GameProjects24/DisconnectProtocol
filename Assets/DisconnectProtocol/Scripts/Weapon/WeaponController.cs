using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public List<WeaponData> weaponDataList; // Список данных оружия
    private List<Weapon> weapons = new List<Weapon>(); // Список созданных оружий
    private Weapon currentWeapon; // Текущее оружие
    public event System.Action OnReload; // Событие для перезарядки
    public delegate void WeaponChangedHandler(Weapon newWeapon);
    public event WeaponChangedHandler OnChangeWeapon;
    private int counter = 0;
    private int index = 0;


    private void Start()
    {
        // Получить все уже существующие оружия
        GetComponentsInChildren(true, weapons);

        // Если оружия уже есть, установить первое как активное
        if (weapons.Count > 0)
        {
            SetActiveWeapon();
            return;
        }

        // Создать оружие из данных, если его нет
        foreach (var weaponData in weaponDataList)
        {
            CreateWeapon(weaponData);
        }

        // Установить первое оружие активным
        if (weapons.Count > 0)
        {
            SetActiveWeapon();
        }
    }

    private void CreateWeapon(WeaponData weaponData)
    {
        if (weaponData == null || weaponData.prefab == null)
        {
            Debug.LogWarning("Invalid weapon data or prefab.");
            return;
        }

        // Создать оружие и присоединить его к контроллеру
        GameObject weaponObj = Instantiate(weaponData.prefab, transform);
        Weapon weapon = weaponObj.GetComponent<Weapon>();

        if (weapon != null)
        {
            weapons.Add(weapon);
            weaponObj.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Prefab does not have a Weapon component.");
            Destroy(weaponObj);
        }
    }

    public void SetActiveWeapon()
    {
        if (index >= weapons.Count) return;

        index++;
        index = index % weapons.Count;
        if (currentWeapon != null)
        {
            currentWeapon.OnReloadWeapon -= HandleReloadWeapon;
            currentWeapon.gameObject.SetActive(false);
        }

        currentWeapon = weapons[index];
        currentWeapon.gameObject.SetActive(true);
        
        currentWeapon.OnReloadWeapon += HandleReloadWeapon;
        OnChangeWeapon?.Invoke(currentWeapon);
    }

	public bool CanFire()
	{
		return currentWeapon != null ? currentWeapon.CanFire() : false;
	}

	public int GetCurrentAmmo()
	{
		return currentWeapon != null ? currentWeapon.GetCurrentAmmo() : 0;
	}

	public int GetTotalAmmo()
	{
		return currentWeapon != null ? currentWeapon.GetTotalAmmo() : 0;
	}

    public bool IsCurWeaponReloading()
    {
        return currentWeapon.IsReloading;
    }

    public void StartFire()
    {
        currentWeapon?.StartFire();
    }

    public void StopFire()
    {
        currentWeapon?.StopFire();
    }

	public void Shoot()
	{
		currentWeapon?.Shoot();
	}

    public void Reload()
    {
        currentWeapon?.Reload();
    }

    private void HandleReloadWeapon()
    {
        OnReload?.Invoke(); // Вызываем событие
    }
}
