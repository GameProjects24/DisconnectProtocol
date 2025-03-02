using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    public List<WeaponData> weaponDataList = new List<WeaponData>();
    public Dictionary<WeaponData, int> reserveAmmoInventory = new Dictionary<WeaponData, int>();
    public Dictionary<WeaponData, int> cageAmmoInventory = new Dictionary<WeaponData, int>();
    public event Action<WeaponData> OnWeaponPicked;
    public event Action OnAmmoChanged;

    private void Start()
    {
        InitializeInventory();
    }

    // Инициализация инвентаря
    public void InitializeInventory()
    {
        Weapon[] weapons = GetComponentsInChildren<Weapon>(true); // true - ищет даже отключенные объекты

        foreach (Weapon weapon in weapons)
        {
            if (!weaponDataList.Contains(weapon.weaponData))
            {
                weaponDataList.Add(weapon.weaponData);
            }

            // Добавляем начальные патроны, если их ещё нет
            if (!reserveAmmoInventory.ContainsKey(weapon.weaponData))
            {
                reserveAmmoInventory.Add(weapon.weaponData, weapon.weaponData.maxAmmo / 4);
            }
        }
    }

    // Добавление оружия в инвентарь
    public void AddWeapon(WeaponData weaponData)
    {
        if (!weaponDataList.Contains(weaponData))
        {
            weaponDataList.Add(weaponData);
            reserveAmmoInventory[weaponData] = weaponData.maxAmmo / 4;
            OnWeaponPicked?.Invoke(weaponData);
        }
    }

    // Получение информации о текущем количестве патронов для конкретного оружия
    public int GetReserveAmmo(WeaponData weaponData)
    {
        return reserveAmmoInventory.ContainsKey(weaponData) ? reserveAmmoInventory[weaponData] : 0;
    }

    // Пополнение патронов для оружия
    public void AddAmmo(WeaponData weaponData, int amount)
    {
        if (reserveAmmoInventory.ContainsKey(weaponData))
        {
            reserveAmmoInventory[weaponData] = Mathf.Min(reserveAmmoInventory[weaponData] + amount, weaponData.maxAmmo);
            OnAmmoChanged?.Invoke();
            Debug.Log($"Было добавлено {amount} патронов для {weaponData.name}");
        }
    }

    public void SpendAmmo(WeaponData weaponData, int amount)
    {
        if (reserveAmmoInventory.ContainsKey(weaponData))
        {
            OnAmmoChanged?.Invoke();
            reserveAmmoInventory[weaponData] = Mathf.Max(reserveAmmoInventory[weaponData] - amount, 0);
        }
    }

    // Проверка, есть ли патроны для текущего оружия
    public bool HasAmmo(Weapon weapon)
    {
        return reserveAmmoInventory.ContainsKey(weapon.weaponData) && reserveAmmoInventory[weapon.weaponData] > 0;
    }

    public bool ReserveAmmoFull(WeaponData weaponData)
    {
        if (reserveAmmoInventory[weaponData] < weaponData.maxAmmo)
            return false;
        else
            return true;
    }

    // Сохранение состояния инвентаря
    public InventoryData SaveInventory()
    {
        InventoryData data = new InventoryData();

        // Сохраняем оружие
        foreach (WeaponData weaponData in weaponDataList)
        {
            data.weaponList.Add(weaponData.weaponName);
            if (cageAmmoInventory.ContainsKey(weaponData))
                data.cageAmmoInventory.Add(weaponData.weaponName, cageAmmoInventory[weaponData]);
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
        weaponDataList.Clear();
        reserveAmmoInventory.Clear();
        cageAmmoInventory.Clear();
        // Восстанавливаем оружие
        foreach (string weaponName in data.weaponList)
        {
            WeaponData weaponData = FindWeaponData(weaponName);
            if (weaponData != null)
            {
                AddWeapon(weaponData);
                if (data.cageAmmoInventory.ContainsKey(weaponName))
                    cageAmmoInventory[weaponData] = data.cageAmmoInventory[weaponName];
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
