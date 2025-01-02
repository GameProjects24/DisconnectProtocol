using System;
using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponData weaponData;

    private int currentAmmo;
    private int totalAmmo;
    private bool isReloading;
    private float lastFireTime;
    private bool isFiring; // Для автоматического огня

    private void Start()
    {
        currentAmmo = weaponData.magazineSize;
        totalAmmo = weaponData.maxAmmo;
        isReloading = false;
        isFiring = false;
    }

    private void Update()
    {
        if (isFiring && CanShoot())
        {
            Shoot();
        }
    }

    public void StartFiring()
    {
        isFiring = true;
    }

    public void StopFiring()
    {
        isFiring = false;
    }

    public void Shoot()
    {
        if (isReloading || Time.time - lastFireTime < weaponData.fireRate || currentAmmo <= 0)
            return;

        lastFireTime = Time.time;

        if (weaponData.projectilePrefab != null)
        {
            FireProjectile();
        }
        else
        {
            Debug.LogWarning("Projectile not found!");
            return;
            // FireHitscan();
        }

        currentAmmo--;
        PlayMuzzleEffect();
        PlayFireSound();
    }

    private void FireProjectile()
    {
        GameObject projectile = Instantiate(weaponData.projectilePrefab, transform.position, transform.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(transform.forward * weaponData.projectileSpeed, ForceMode.VelocityChange);
        }
    }

    // private void FireHitscan()
    // {
    //     RaycastHit hit;
    //     if (Physics.Raycast(transform.position, transform.forward, out hit, weaponData.range))
    //     {
    //         // Example: Apply damage to the target if it has a health component
    //         var target = hit.collider.GetComponent<Target>();
    //         if (target != null)
    //         {
    //             target.TakeDamage(weaponData.damage);
    //         }
    //     }
    // }

    public void Reload()
    {
        if (isReloading || currentAmmo == weaponData.magazineSize || totalAmmo <= 0)
            return;

        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        PlayReloadSound();
        yield return new WaitForSeconds(weaponData.reloadTime);

        int ammoNeeded = weaponData.magazineSize - currentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, totalAmmo);
        currentAmmo += ammoToReload;
        totalAmmo -= ammoToReload;

        isReloading = false;
    }

    private void PlayMuzzleEffect()
    {
        if (weaponData.muzzleFlashEffect != null)
        {
            Instantiate(weaponData.muzzleFlashEffect, transform.position, transform.rotation);
        }
    }

    private void PlayFireSound()
    {
        if (weaponData.fireSound != null)
        {
            AudioSource.PlayClipAtPoint(weaponData.fireSound, transform.position);
        }
    }

    private void PlayReloadSound()
    {
        if (weaponData.reloadSound != null)
        {
            AudioSource.PlayClipAtPoint(weaponData.reloadSound, transform.position);
        }
    }

    public bool CanShoot()
    {
        return !isReloading && currentAmmo > 0 && Time.time - lastFireTime >= weaponData.fireRate;
    }

    public int GetCurrentAmmo() => currentAmmo;

    public int GetTotalAmmo() => totalAmmo;
}
