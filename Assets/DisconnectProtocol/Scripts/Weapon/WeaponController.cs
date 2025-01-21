using UnityEngine;
using System.Collections.Generic;

public class WeaponController : MonoBehaviour
{
    public List<WeaponData> weaponDataList; // Список данных оружия
    private List<Weapon> weapons = new List<Weapon>(); // Список созданных оружий
    private Weapon currentWeapon; // Текущее оружие
    public event System.Action OnReload; // Событие для перезарядки
    public delegate void WeaponChangedHandler(Weapon newWeapon);
    public event WeaponChangedHandler OnChangeWeapon;


    private void Start()
    {
        // Получить все уже существующие оружия
        GetComponentsInChildren(true, weapons);

        // Если оружия уже есть, установить первое как активное
        if (weapons.Count > 0)
        {
            SetActiveWeapon(0);
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
            SetActiveWeapon(0);
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

    public void SetActiveWeapon(int index)
    {
        if (index < 0 || index >= weapons.Count) return;

        if (currentWeapon != null)
        {
            currentWeapon.gameObject.SetActive(false);
        }

        currentWeapon = weapons[index];
        currentWeapon.gameObject.SetActive(true);

        OnChangeWeapon?.Invoke(currentWeapon);
    }

    public void StartFire()
    {
        currentWeapon?.StartFire();
    }

    public void StopFire()
    {
        currentWeapon?.StopFire();
    }

    public void Reload()
    {
        currentWeapon?.Reload();
        OnReload?.Invoke(); // Вызываем событие
    }
}
