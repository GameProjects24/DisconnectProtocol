using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Data")]
    public WeaponData weaponData;

    private WeaponFSM _weaponFSM;
    public Transform _muzzle;
    private int _cageBullets;
    private int _bulletCount;
    private bool _isReloading;
    private bool _isFiring;
    
    public event System.Action OnReloadWeapon; // Событие для перезарядки

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
    }

    private void Start()
    {
        _cageBullets = weaponData.cageSize;
        _bulletCount = _cageBullets * 2;

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

    public void Reload()
    {
        if (_cageBullets != weaponData.cageSize && HasBullet())
        {
            _weaponFSM.Reload();
            OnReloadWeapon?.Invoke(); // Вызываем событие
        }
    }

    private void Update()
    {
        _weaponFSM.Update();
    }

    public void Shoot()
    {
        --_cageBullets;
        --_bulletCount;

        Debug.Log("Weapon shoot");
        weaponData.weaponShoot.Shoot(_muzzle.position, _muzzle.forward);
    }

    public void ReloadComplete()
    {
        Debug.Log("Weapon ReloadComplete");
        _cageBullets = Mathf.Min(weaponData.cageSize, _bulletCount);
    }
}
