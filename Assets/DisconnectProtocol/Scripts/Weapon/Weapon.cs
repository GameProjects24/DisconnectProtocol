using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Data")]
    public WeaponData weaponData;

    private WeaponFSM _weaponFSM;
    [SerializeField] private Transform _muzzle;
    public bool IsReloading { get; private set; }

    public int cageAmmo { get; private set; }
	public int reserveAmmo { get; private set; }

    public event Action OnReloadWeapon;
    public event Action OnShootWeapon;
    public event Action OnReloadCompleteWeapon;
	public event Action OnAmmoChanged;

    private void Awake()
    {
        _weaponFSM = new WeaponFSM(this);
        cageAmmo = weaponData.cageSize;
    }

    private void Start()
    {
        _weaponFSM.ActivateState(WeaponStateEnum.Idle);
    }

    public void StartFire()
    {
        _weaponFSM.StartFire();
    }

    public void StopFire()
    {
        _weaponFSM.StopFire();
    }

    private void Update()
    {
        _weaponFSM.Update();
    }

    public void Shoot()
    {
        if (CanFire())
        {
            --cageAmmo; // Уменьшаем патроны в магазине
			OnAmmoChanged?.Invoke();
            Debug.Log("Weapon shoot");
            weaponData.weaponShoot.Shoot(_muzzle.position, _muzzle.forward, weaponData.damage);
            OnShootWeapon?.Invoke();
        }
    }

    public void Reload()
    {
        if (cageAmmo < weaponData.cageSize && reserveAmmo > 0)
        {
            if (!IsReloading)
            {
                OnReloadWeapon?.Invoke(); // Вызываем событие
                _weaponFSM.ActivateState(WeaponStateEnum.Reload);
                IsReloading = true;
            }
        }
    }

    public void ReloadComplete()
    {
        IsReloading = false;
        Debug.Log("Weapon ReloadComplete");
		cageAmmo += TrySpendAmmo(weaponData.cageSize - cageAmmo);
		OnAmmoChanged?.Invoke();
        OnReloadCompleteWeapon?.Invoke();
    }

	private int TrySpendAmmo(int needed)
	{
		int res = Mathf.Min(needed, reserveAmmo);
		reserveAmmo -= res;
		return res;
	}

    public bool CanFire()
    {
        return cageAmmo > 0 && IsReloading == false;
    }

    public bool HasAmmo()
    {
        return cageAmmo > 0;
    }

	public void SetCageAmmo(int ammo)
	{
		cageAmmo = ammo;
		OnAmmoChanged?.Invoke();
	}

	public void SetReserveAmmo(int ammo)
	{
		reserveAmmo = ammo;
		OnAmmoChanged?.Invoke();
	}

    public Transform GetMuzzle() => _muzzle;
}
