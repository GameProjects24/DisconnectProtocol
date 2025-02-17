using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private List<Weapon> weapons = new List<Weapon>();
    private Weapon currentWeapon;
    public event Action OnReload;
    public event Action OnShoot;
    public event Action OnReloadComplete;
    public delegate void WeaponChangedHandler(Weapon newWeapon);
    public event WeaponChangedHandler OnChangeWeapon;
    private int index = 0;


    private void Start()
    {
        // Получить все уже существующие оружия
        GetComponentsInChildren(true, weapons);

        // Если оружия уже есть, установить первое как активное
        if (weapons.Count > 0)
        {
            SetActiveWeapon();
        }
    }

    public void SetActiveWeapon()
    {
        if (index >= weapons.Count) return;

        index++;
        index = index % weapons.Count;
        if (currentWeapon != null)
        {
            currentWeapon.OnReloadWeapon -= HandleReloadWeapon;
            currentWeapon.OnShootWeapon -= HandleShootWeapon;
            currentWeapon.OnReloadCompleteWeapon -= HandleReloadWeapon;
            currentWeapon.gameObject.SetActive(false);
        }

        currentWeapon = weapons[index];
        currentWeapon.gameObject.SetActive(true);
        
        currentWeapon.OnReloadWeapon += HandleReloadWeapon;
        currentWeapon.OnShootWeapon += HandleShootWeapon;
        currentWeapon.OnReloadCompleteWeapon += HandleReloadCompleteWeapon;
        OnChangeWeapon?.Invoke(currentWeapon);
    }

    public bool CanFire()
	{
		return currentWeapon != null ? currentWeapon.CanFire() : false;
	}

	public int GetCurrentAmmo()
	{
		return currentWeapon != null ? currentWeapon.GetCurrentAmmo() : 0;
	}

	public int GetTotalAmmo()
	{
		return currentWeapon != null ? currentWeapon.GetTotalAmmo() : 0;
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
		currentWeapon?.Shoot();
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
