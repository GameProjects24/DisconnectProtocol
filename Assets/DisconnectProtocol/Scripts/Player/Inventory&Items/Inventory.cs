using System.Collections.Generic;
using System.Data;
using UnityEngine;

/// <summary>
/// Stores and allows to save and load info about items inside.
/// Also allows pickup items. No more
/// </summary>
public class Inventory : MonoBehaviour
{
	[System.Serializable]
	public class WeaponInfo
	{
		public string name;
		public int reserveAmmo;
		public int cageAmmo;

		public WeaponInfo(Weapon weapon)
		{
			name = weapon.weaponData.weaponName;
			cageAmmo = weapon.cageAmmo;
			reserveAmmo = weapon.reserveAmmo;
		}
	}

	[System.Serializable]
	public class InventoryData
	{
		public List<WeaponInfo> weapons = new List<WeaponInfo>();
	}

	public List<WeaponData> initialWeapons = new List<WeaponData>();
	public List<Weapon> weapons = new List<Weapon>();
	private WeaponData[] m_weaponRes;

    public event System.Action<Weapon> AddedWeapon;
	public event System.Action Loaded;
	
	private void Awake()
	{
		m_weaponRes = Resources.LoadAll<WeaponData>("Weapons");
		foreach (var wd in initialWeapons)
		{
			var weapon = wd.CreateWeapon();
			weapons.Add(weapon);
		}
	}

	public void AddWeapon(Weapon weapon)
	{
		// how about adding ammos?
		if (HasWeapon(weapon))
		{
			return;
		}
		weapons.Add(weapon);
		AddedWeapon?.Invoke(weapon);
	}

	private bool HasWeapon(WeaponData wd)
	{
		return HasWeapon(wd.weaponName);
	}

	public bool HasWeapon(Weapon weapon)
	{
		return HasWeapon(weapon.weaponData.weaponName);
	}

	private bool HasWeapon(string weaponName)
	{
		foreach (var w in weapons)
		{
			if (w.weaponData.weaponName == weaponName)
			{
				return true;
			}
		}
		return false;
	}

	public InventoryData ToInventoryData()
	{
		var data = new InventoryData();
		foreach (var w in weapons)
		{
			data.weapons.Add(new WeaponInfo(w));
		}
		return data;
	}

	// maybe pool?
	public void FromInventoryData(InventoryData data)
	{
		weapons.ForEach(w => Destroy(w.gameObject));
		weapons.Clear();
		foreach (var wi in data.weapons)
		{
			if (TryFindWeaponData(wi.name, out var wd) && !HasWeapon(wd))
			{
				var weapon = wd.CreateWeapon();
				weapon.SetCageAmmo(wi.cageAmmo);
				weapon.SetReserveAmmo(wi.reserveAmmo);
				weapons.Add(weapon);
			}
		}
		Loaded?.Invoke();
	}

	private bool TryFindWeaponData(string weaponName, out WeaponData weaponData)
	{
		weaponData = null;
		// Поиск WeaponData по имени оружия (можно настроить в редакторе или через массив)
		foreach (var data in m_weaponRes)
		{
			if (data.weaponName == weaponName)
			{
				weaponData = data;
				return true;
			}
		}
		return false;
	}
}

[System.Serializable]
public class SerDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
	public List<TKey> keys = new List<TKey>();
	public List<TValue> values = new List<TValue>();
	public Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();

	public void OnBeforeSerialize()
	{
		keys.Clear();
		values.Clear();
		foreach (var kvp in dict)
		{
			keys.Add(kvp.Key);
			values.Add(kvp.Value);
		}
	}

	public void OnAfterDeserialize()
	{
		dict = new Dictionary<TKey, TValue>();
		if (keys.Count != values.Count)
		{
			throw new DataException("Some of TKey or TValue don't Serializable");
		}
		for (int i = 0; i < keys.Count; i++)
		{
			dict.Add(keys[i], values[i]);
		}
	}
}
