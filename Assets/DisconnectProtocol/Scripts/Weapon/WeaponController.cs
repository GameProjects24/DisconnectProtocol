using System;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private Weapon currentWeapon;
    public event Action OnReload;
    public event Action OnShoot;
    public event Action OnReloadComplete;
    public delegate void WeaponChangedHandler(Weapon newWeapon);
    public event WeaponChangedHandler OnChangeWeapon;

    [SerializeField] private Inventory _inventory;
	public Inventory inventory {
		get {
			if (_inventory == null) {
				_inventory = GetComponentInChildren<Inventory>();
			}
			return _inventory;
		}
		set => _inventory = value;
	}


    private void Start()
    {
		if (inventory.TryGetCurWeapon(out var weapon))
		{
			SetActiveWeapon(weapon);
		}
    }

	public Inventory.InventoryData GetInventory()
	{
		return inventory.ToInventoryData();
	}

	public void SetInventory(Inventory.InventoryData data)
	{
		inventory.FromInventoryData(data);
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
        if (inventory.TryGetNextWeapon(out var weapon))
		{
			SetActiveWeapon(weapon);
		}
    }

    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public bool CanFire()
	{
		return currentWeapon != null && inventory.HasAmmo(currentWeapon);
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
