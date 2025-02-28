using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    public List<Weapon> weapons = new List<Weapon>();
    public Dictionary<WeaponData, int> reserveAmmoInventory = new Dictionary<WeaponData, int>();
    public Dictionary<WeaponData, int> cageAmmoInventory = new Dictionary<WeaponData, int>();
    public WeaponController weaponController;

    private void Start()
    {
        InitializeInventory();
    }

    // Инициализация инвентаря
    public void InitializeInventory()
    {
        foreach (Weapon weapon in weapons)
        {
            // Добавляем оружие в инвентарь, если его ещё нет
            if (!reserveAmmoInventory.ContainsKey(weapon.weaponData))
            {
                reserveAmmoInventory.Add(weapon.weaponData, Mathf.Min(weapon.weaponData.maxAmmo, weapon.weaponData.cageSize));
            }
        }
    }

    // Добавление оружия в инвентарь
    public void AddWeapon(Weapon weapon)
    {
        if (!weapons.Contains(weapon))
        {
            weapons.Add(weapon);
            reserveAmmoInventory[weapon.weaponData] = weapon.weaponData.maxAmmo / 4;
        }
    }

    // Получение информации о текущем количестве патронов для конкретного оружия
    public int GetReserveAmmo(Weapon weapon)
    {
        return reserveAmmoInventory.ContainsKey(weapon.weaponData) ? reserveAmmoInventory[weapon.weaponData] : 0;
    }

    // Пополнение патронов для оружия
    public void AddAmmo(Weapon weapon, int amount)
    {
        if (reserveAmmoInventory.ContainsKey(weapon.weaponData))
        {
            reserveAmmoInventory[weapon.weaponData] = Mathf.Min(reserveAmmoInventory[weapon.weaponData] + amount, weapon.weaponData.maxAmmo);
        }
    }

    public void SpendAmmo(Weapon weapon, int amount)
    {
        if (reserveAmmoInventory.ContainsKey(weapon.weaponData))
        {
            reserveAmmoInventory[weapon.weaponData] = Mathf.Max(reserveAmmoInventory[weapon.weaponData] - amount, 0);
        }
    }

    // Проверка, есть ли патроны для текущего оружия
    public bool HasAmmo(Weapon weapon)
    {
        return reserveAmmoInventory.ContainsKey(weapon.weaponData) && reserveAmmoInventory[weapon.weaponData] > 0;
    }

    // Сохранение состояния инвентаря
    public InventoryData SaveInventory()
    {
        InventoryData data = new InventoryData();

        // Сохраняем оружие
        foreach (Weapon weapon in weapons)
        {
            data.weaponList.Add(weapon.weaponData.weaponName);
            data.cageAmmoInventory.Add(weapon.weaponData.weaponName, weapon.GetCageAmmo());
        }

        // Сохраняем патроны для каждого оружия
        foreach (var ammo in reserveAmmoInventory)
        {
            data.reserveAmmoInventory.Add(ammo.Key.weaponName, ammo.Value);
        }

        return data;
    }

    // Восстановление состояния инвентаря
    public void LoadInventory(InventoryData data)
    {
        // Восстанавливаем оружие
        foreach (string weaponName in data.weaponList)
        {
            WeaponData weaponData = FindWeaponData(weaponName);
            if (weaponData != null)
            {
                Weapon newWeapon = CreateWeapon(weaponData);
                AddWeapon(newWeapon);
                if (data.cageAmmoInventory.ContainsKey(weaponName))
                {
                    newWeapon.SetCageAmmo(data.cageAmmoInventory[weaponName]);
                }
            }
        }

        // Восстанавливаем патроны
        foreach (var ammo in data.reserveAmmoInventory)
        {
            WeaponData weaponData = FindWeaponData(ammo.Key);
            if (weaponData != null)
            {
                reserveAmmoInventory[weaponData] = ammo.Value;
            }
        }
    }

    private WeaponData FindWeaponData(string weaponName)
    {
        foreach (WeaponData data in Resources.LoadAll<WeaponData>("Weapons"))
        {
            if (data.weaponName == weaponName)
            {
                return data;
            }
        }
        return null;
    }

    private Weapon CreateWeapon(WeaponData weaponData)
    {
        GameObject weaponObject = Instantiate(weaponData.prefab);
        Weapon newWeapon = weaponObject.GetComponent<Weapon>();
        newWeapon.weaponData = weaponData;
        return newWeapon;
    }
}

// Класс для сохранения инвентаря
[Serializable]
public class InventoryData
{
    public List<string> weaponList = new List<string>(); // Список имен оружий
    public Dictionary<string, int> reserveAmmoInventory = new Dictionary<string, int>(); // Количество патронов для каждого оружия
    public Dictionary<string, int> cageAmmoInventory = new Dictionary<string, int>();
}
