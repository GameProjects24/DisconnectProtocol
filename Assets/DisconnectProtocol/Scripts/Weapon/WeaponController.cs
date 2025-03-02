using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Inventory inventory;
    public Transform weaponHolder;

    private Weapon currentWeapon;
    private Weapon newWeapon;
    private List<Weapon> weaponInstances = new List<Weapon>();
    public event Action OnReload;
    public event Action OnShoot;
    public event Action OnReloadComplete;
    public event Action<Weapon> OnChangeWeapon;
    private int index;


    private void Start()
    {
        inventory.OnWeaponPicked += HandleWeaponPicked;
        foreach (Transform child in weaponHolder)
        {
            Weapon weapon = child.GetComponent<Weapon>();
            if (weapon != null)
            {
                weaponInstances.Add(weapon);
                weapon.gameObject.SetActive(false);
            }
        }

        if (weaponInstances.Count > 0)
            SetActiveWeapon(weaponInstances[0]);
    }

    private void OnDisable()
    {
        if (currentWeapon != null)
        {
            UnsubscribeFromWeaponEvents();
        }
        if (inventory != null)
        {
            inventory.OnWeaponPicked -= HandleWeaponPicked;
        }
    }

    private void SetActiveWeapon(Weapon weapon)
    {
        if (currentWeapon == weapon) return;
        
        if (currentWeapon != null)
        {
            UnsubscribeFromWeaponEvents();
            currentWeapon.gameObject.SetActive(false);
        }

        currentWeapon = weapon;
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;
        currentWeapon.gameObject.SetActive(true);
        SubscribeToWeaponEvents();
        OnChangeWeapon?.Invoke(currentWeapon);
    }

    public void ChangeWeapon()
    {
        if (weaponInstances.Count == 0)
            return;
        index = (index + 1) % weaponInstances.Count;
        SetActiveWeapon(weaponInstances[index]);
    }

    public void HandleWeaponPicked(WeaponData weaponData)
    {
        newWeapon = Instantiate(weaponData.prefab, weaponHolder).GetComponent<Weapon>();
        weaponInstances.Add(newWeapon);
        SetActiveWeapon(newWeapon);
        index = weaponInstances.Count - 1;
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
