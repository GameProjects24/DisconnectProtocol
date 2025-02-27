using System;
using UnityEngine;
using UnityEngine.LowLevelPhysics;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Data")]
    public WeaponData weaponData;

    private WeaponFSM _weaponFSM;
    [SerializeField] private Transform _muzzle;
    public bool IsReloading { get; private set; }

    public Inventory inventory;
    private int _cageAmmo;

    public event Action OnReloadWeapon;
    public event Action OnShootWeapon;
    public event Action OnReloadCompleteWeapon;

    private void Awake()
    {
        _weaponFSM = new WeaponFSM(this, inventory);
        _cageAmmo = weaponData.cageSize;
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
            _cageAmmo--; // Уменьшаем патроны в магазине
            Debug.Log("Weapon shoot");
            weaponData.weaponShoot.Shoot(_muzzle.position, _muzzle.forward, weaponData.damage);
            OnShootWeapon?.Invoke();
        }
    }

    public void Reload()
    {
        if (_cageAmmo < weaponData.cageSize && inventory.HasAmmo(this))
        {
            if (!IsReloading)
            {
                OnReloadWeapon?.Invoke(); // Вызываем событие
            }

            _weaponFSM.ActivateState(WeaponStateEnum.Reload);
            IsReloading = true;
        }
    }

    public void ReloadComplete()
    {
        IsReloading = false;
        Debug.Log("Weapon ReloadComplete");
        int neededAmmo = weaponData.cageSize - _cageAmmo;
        int ammoToReload = Mathf.Min(neededAmmo, inventory.GetReserveAmmo(this));

        _cageAmmo += ammoToReload;
        inventory.SpendAmmo(this, ammoToReload);
        OnReloadCompleteWeapon?.Invoke();
    }

    public bool CanFire()
    {
        return _cageAmmo > 0 && IsReloading == false;
    }

    public bool HasAmmo()
    {
        return _cageAmmo > 0;
    }

    public void SetCageAmmo(int ammo)
    {
        _cageAmmo = ammo;
    }

    public int GetCageAmmo() => _cageAmmo;

    public int GetReserveAmmo() => inventory.GetReserveAmmo(this);

    public Transform GetMuzzle() => _muzzle;
}
