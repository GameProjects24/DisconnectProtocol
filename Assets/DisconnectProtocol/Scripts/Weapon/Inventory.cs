using System.Collections.Generic;
using System.Data;
using UnityEngine;


public class Inventory : MonoBehaviour
{
	[System.Serializable]
	public class WeaponInfo
	{
		public string name;
		public int reserveAmmo;
		public int cageAmmo;
	}

	[System.Serializable]
	public class InventoryData
	{
		public List<WeaponInfo> weapons = new List<WeaponInfo>();
	}

	[Header("Weapons")]
	public Transform weaponHolder;
	public List<WeaponData> initialWeapons;
	public Dictionary<Weapon, WeaponInfo> weapons = new Dictionary<Weapon, WeaponInfo>();
	private List<Weapon> m_factWeapons = new List<Weapon>();
	private int m_weaponIdx = 0;
	private WeaponData[] m_weaponRes;

	private void Awake()
	{
		m_weaponRes = Resources.LoadAll<WeaponData>("Weapons");
		if (weaponHolder == null)
		{
			weaponHolder = transform;
		}
		foreach (var wd in initialWeapons)
		{
			AddWeapon(CreateWeapon(wd));
		}
	}

	private void NewWeapon(Weapon weapon)
	{
		weapon.inventory = this;
		weapon.transform.SetParent(weaponHolder);
		weapon.gameObject.SetActive(false);
		m_factWeapons.Add(weapon);
	}

	// Добавление оружия в инвентарь
	public void AddWeapon(Weapon weapon)
	{
		// how about adding ammos?
		if (weapon == null || weapons.ContainsKey(weapon))
		{
			return;
		}
		var wd = weapon.weaponData;
		var info = new WeaponInfo();
		info.name = wd.weaponName;
		info.reserveAmmo = Mathf.Min(wd.cageSize, wd.maxAmmo);
		weapons.Add(weapon, info);
		
		NewWeapon(weapon);
	}

	public bool TryGetNextWeapon(out Weapon weapon)
	{
		weapon = null;
		if (weapons.Count == 0)
		{
			return false;
		}
		if (weapons.Count == 1)
		{
			weapon = m_factWeapons[m_weaponIdx];
			return true;
		}
		int prev = m_weaponIdx;
		do {
			m_weaponIdx = (m_weaponIdx + 1) % m_factWeapons.Count;
			if (weapons.ContainsKey(m_factWeapons[m_weaponIdx]))
			{
				weapon = m_factWeapons[m_weaponIdx];
				break;
			}
		} while (m_weaponIdx != prev);
		return true;
	}

	// Получение информации о текущем количестве патронов для конкретного оружия
	// why is it in inventory???
	public int GetReserveAmmo(Weapon weapon)
	{
		return weapons.TryGetValue(weapon, out var wi) ? wi.reserveAmmo : 0;
	}

	// Пополнение патронов для оружия
	public void AddAmmo(Weapon weapon, int amount)
	{
		if (weapons.TryGetValue(weapon, out var wi))
		{
			wi.reserveAmmo = Mathf.Min(wi.reserveAmmo + amount, weapon.weaponData.maxAmmo);
		}
	}

	// how about combine GetReserveAmmo and SpendAmmo into
	// int TrySpendAmmo(Weapon, int) ?
	public void SpendAmmo(Weapon weapon, int amount)
	{
		if (weapons.TryGetValue(weapon, out var wi))
		{
			wi.reserveAmmo = Mathf.Max(wi.reserveAmmo - amount, 0);
		}
	}

	// Проверка, есть ли патроны для текущего оружия
	public bool HasAmmo(Weapon weapon)
	{
		return weapons.TryGetValue(weapon, out var wi) && wi.reserveAmmo > 0;
	}

	// Сохранение состояния инвентаря (например, в чекпоинте)
	// should consider moving
	// cageAmmo counting in logic of other methods
	public InventoryData ToInventoryData()
	{
		var data = new InventoryData();
		foreach ((var weapon, var info) in weapons)
		{
			info.cageAmmo = weapon.GetCageAmmo();
			data.weapons.Add(info);
		}
		return data;
	}

	public void FromInventoryData(InventoryData data)
	{
		weapons.Clear();
		foreach (var wi in data.weapons)
		{
			Weapon weapon = null;
			if (!TryFindWeapon(wi.name, out weapon))
			{
				if (TryFindWeaponData(wi.name, out var wd))
				{
					weapon = CreateWeapon(wd);
				}
			}
			if (weapon != null)
			{
				weapon.SetCageAmmo(wi.cageAmmo);
				weapons.Add(weapon, wi);
				NewWeapon(weapon);
			}
		}
	}

	private bool TryFindWeapon(string name, out Weapon weapon)
	{
		weapon = null;
		foreach (var w in m_factWeapons)
		{
			if (w.weaponData.weaponName == name)
			{
				weapon = w;
				return true;
			}
		}
		return false;
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

	private Weapon CreateWeapon(WeaponData weaponData)
	{
		GameObject weaponObject = Instantiate(weaponData.prefab, weaponHolder);
		Weapon newWeapon = weaponObject.GetComponent<Weapon>();
		newWeapon.weaponData = weaponData;
		return newWeapon;
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