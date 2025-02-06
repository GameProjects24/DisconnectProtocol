using System;
using UnityEngine;
using UnityEngine.LowLevelPhysics;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Data")]
    public WeaponData weaponData;

    private WeaponFSM _weaponFSM;
    public Transform _muzzle;
    private int _cageBullets;
    private int _bulletCount;
    private bool isReloading;

    // Публичное только для чтения свойство
    public bool IsReloading
    {
        get { return isReloading; }
    }
    
    public event Action OnReloadWeapon;
    public event Action OnShootWeapon;
    public event Action OnReloadCompleteWeapon;

    public bool CanFire()
    {
        return _cageBullets > 0;
    }

    public bool HasBullet()
    {
        return _bulletCount > 0;
    }

    private void Awake()
    {
        _weaponFSM = new WeaponFSM(this);
        _cageBullets = weaponData.cageSize;
        _bulletCount = _cageBullets * 2;
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
        --_cageBullets;

        Debug.Log("Weapon shoot");
        weaponData.weaponShoot.Shoot(_muzzle.position, _muzzle.forward, weaponData.damage);
        OnShootWeapon?.Invoke();
    }

    public void Reload()
    {
        if (_cageBullets < weaponData.cageSize && HasBullet())
        {
            if (!isReloading)
            {
                OnReloadWeapon?.Invoke(); // Вызываем событие
            }
            
            _weaponFSM.Reload();
            isReloading = true;
        }
    }

    public void ReloadComplete()
    {
        isReloading = false;
        Debug.Log("Weapon ReloadComplete");
        int neededAmmo = weaponData.cageSize - _cageBullets;
        int ammoToReload = Mathf.Min(neededAmmo, _bulletCount);

        _cageBullets += ammoToReload;
        _bulletCount -= ammoToReload;
        OnReloadCompleteWeapon?.Invoke();
    }

    public int GetCurrentAmmo() => _cageBullets;

    public int GetTotalAmmo() => _bulletCount;
}
