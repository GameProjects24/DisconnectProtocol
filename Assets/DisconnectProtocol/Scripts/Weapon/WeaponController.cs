using System.Collections.Generic;
using DisconnectProtocol;
using UnityEngine;


/// <summary>
/// Manages weapon arsenal. May work with inventory
/// and standalone
/// </summary>
public class WeaponController : MonoBehaviour
{
    public event System.Action OnReload;
    public event System.Action OnShoot;
    public event System.Action OnReloadComplete;
    public event System.Action<Weapon> OnChangeWeapon;

	private List<Weapon> _weapons = new List<Weapon>();
    private Weapon currentWeapon;
	private int _weaponIdx = 0;

    [SerializeField] private Inventory _inventory;
	[SerializeField] private Transform _weaponHolder;

    private void Start()
    {
		if (_weaponHolder == null)
		{
			_weaponHolder = transform;
		}

		GetComponentsInChildren(true, _weapons);
		if (_inventory == null)
		{
			_inventory = gameObject.GetComponentWherever<Inventory>();
		}
		if (_inventory != null)
		{
			foreach (var w in _weapons)
			{
				Destroy(w.gameObject);
			}
			_weapons = _inventory.weapons;
			_inventory.AddedWeapon += OnWeaponAdded;
			_inventory.Loaded += OnInventoryLoaded;
		}

		if (_weapons.Count > 0)
		{
			OnInventoryLoaded();
			SetActiveWeapon(_weapons[0]);
		}
    }

	private void OnInventoryLoaded()
	{
		foreach (var w in _weapons)
		{
			OnWeaponAdded(w);
		}
	}

	private void OnWeaponAdded(Weapon weapon)
	{
		weapon.gameObject.SetActive(false);
		var wt = weapon.transform;
		wt.SetParent(_weaponHolder);
		wt.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
	}

    public void SetActiveWeapon(Weapon weapon)
    {
		if (currentWeapon == weapon)
		{
			return;
		}
        if (currentWeapon != null)
        {
            UnsubscribeFromWeaponEvents();
            currentWeapon.gameObject.SetActive(false);
        }

        currentWeapon = weapon;
		SubscribeToWeaponEvents();
        currentWeapon.gameObject.SetActive(true);
        OnChangeWeapon?.Invoke(currentWeapon);
    }

    public void ChangeWeapon()
    {
        _weaponIdx = (_weaponIdx + 1) % _weapons.Count;
		SetActiveWeapon(_weapons[_weaponIdx]);
    }

    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public bool CanFire()
	{
		return currentWeapon != null && currentWeapon.cageAmmo > 0;
	}

	public int GetCurrentAmmo()
	{
		return currentWeapon != null ? currentWeapon.cageAmmo : 0;
	}

	public int GetTotalAmmo()
	{
		return currentWeapon != null ? currentWeapon.reserveAmmo : 0;
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
		if (CanFire())
        {
            currentWeapon?.Shoot();
        }
	}

    public void Reload()
    {
        currentWeapon?.Reload();
    }

    private void HandleReloadWeapon()
    {
        OnReload?.Invoke();
    }

    private void HandleShootWeapon()
    {
        OnShoot?.Invoke();
    }

    private void HandleReloadCompleteWeapon()
    {
        OnReloadComplete?.Invoke();
    }

    private void SubscribeToWeaponEvents()
    {
        currentWeapon.OnReloadWeapon += HandleReloadWeapon;
        currentWeapon.OnShootWeapon += HandleShootWeapon;
        currentWeapon.OnReloadCompleteWeapon += HandleReloadCompleteWeapon;
    }

    void UnsubscribeFromWeaponEvents()
    {
        currentWeapon.OnReloadWeapon -= HandleReloadWeapon;
        currentWeapon.OnShootWeapon -= HandleShootWeapon;
        currentWeapon.OnReloadCompleteWeapon -= HandleReloadCompleteWeapon;
    }
}
