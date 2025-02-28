using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    public List<Weapon> weapons = new List<Weapon>(); // Список оружия
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

    // Сохранение состояния инвентаря (например, в чекпоинте)
    public InventoryData SaveInventory()
    {
        InventoryData data = new InventoryData();

        // Сохраняем оружие
        foreach (Weapon weapon in weapons)
        {
            var info = new WeaponInfo();
            info.cageAmmo = weapon.GetCageAmmo();
            info.reserveAmmo = reserveAmmoInventory[weapon.weaponData];
            data.weapons.Add(weapon.weaponData.weaponName, info);
        }

        return data;
    }

    // Восстановление состояния инвентаря
    public void LoadInventory(InventoryData data)
    {
        // Восстанавливаем оружие
        foreach ((var name, var info) in data.weapons)
        {
            WeaponData weaponData = FindWeaponData(name);
            if (weaponData != null)
            {
                Weapon newWeapon = CreateWeapon(weaponData);
                newWeapon.SetCageAmmo(info.cageAmmo);
                reserveAmmoInventory[weaponData] = info.reserveAmmo;
                AddWeapon(newWeapon);
            }
        }
    }

    private WeaponData FindWeaponData(string weaponName)
    {
        // Поиск WeaponData по имени оружия (можно настроить в редакторе или через массив)
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
        GameObject weaponObject = Instantiate(weaponData.prefab); // Создаём экземпляр оружия
        Weapon newWeapon = weaponObject.GetComponent<Weapon>();
        newWeapon.weaponData = weaponData;
        return newWeapon;
    }
}

// Класс для сохранения инвентаря
[Serializable]
public class InventoryData
{
    public Dictionary<string, WeaponInfo> weapons = new Dictionary<string, WeaponInfo>();
}

[Serializable]
public class WeaponInfo
{
    public int reserveAmmo;
    public int cageAmmo;
}

public class SerDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    public List<TKey> keys = new List<TKey>();
    public List<TValue> values = new List<TValue>();
    public Dictionary<TKey, TValue>  myDictionary = new Dictionary<TKey, TValue>();

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (var kvp in myDictionary)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        myDictionary = new Dictionary<TKey, TValue>();
        if (keys.Count != values.Count)
        {
            throw new DataException("Some of TKey or TValue don't Serializable");
        }
        for (int i = 0; i < keys.Count; i++)
        {
            myDictionary.Add(keys[i], values[i]);
        }
    }
}