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
		SetActiveWeapon();
    }

	public Inventory.InventoryData GetInventory()
	{
		return inventory.ToInventoryData();
	}

	public void SetInventory(Inventory.InventoryData data)
	{
		inventory.FromInventoryData(data);
	}

    public void SetActiveWeapon()
    {
		Weapon nextWeapon;
        if (!inventory.TryGetNextWeapon(out nextWeapon))
		{
			return;
		}
        if (currentWeapon != null)
        {
            currentWeapon.OnReloadWeapon -= HandleReloadWeapon;
            currentWeapon.OnShootWeapon -= HandleShootWeapon;
            currentWeapon.OnReloadCompleteWeapon -= HandleReloadCompleteWeapon;
            currentWeapon.gameObject.SetActive(false);
        }

        currentWeapon = nextWeapon;
        currentWeapon.gameObject.SetActive(true);
        
        currentWeapon.OnReloadWeapon += HandleReloadWeapon;
        currentWeapon.OnShootWeapon += HandleShootWeapon;
        currentWeapon.OnReloadCompleteWeapon += HandleReloadCompleteWeapon;
        OnChangeWeapon?.Invoke(currentWeapon);
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
		return currentWeapon != null ? currentWeapon.GetCageAmmo() : 0;
	}

	public int GetTotalAmmo()
	{
		return currentWeapon != null ? currentWeapon.GetReserveAmmo() : 0;
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
}
