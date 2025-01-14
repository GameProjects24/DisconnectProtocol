using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponData weaponData; // Скриптовый объект с данными оружия

    private int currentAmmo; // Текущий боезапас в магазине
    private int totalAmmo;   // Общий запас боеприпасов
    private float fireCooldown; // Время до следующего выстрела

    private WeaponFSM weaponFSM; // Ссылка на конечный автомат состояний

    private Animator _animator;
    private AudioSource _fireAudioSource;

    // для прицеливания (ToDo)
    private Vector3 _defaultPosition; // Позиция оружия без прицела
    private Quaternion _defaultRotation; // Ротация оружия без прицеливания
    [SerializeField] private float _aimSpeed = 10f; // Скорость перемещения при прицеливании

    void Awake()
    {
        // Инициализация FSM
        weaponFSM = new WeaponFSM(this);
    }

    void Start()
    {
        currentAmmo = weaponData.magazineSize;
        totalAmmo = weaponData.maxAmmo;
        fireCooldown = 0f;

        _animator = GetComponent<Animator>();

        // Добавляем AudioSource для воспроизведения звука стрельбы
        _fireAudioSource = GetComponent<AudioSource>();
        _fireAudioSource.clip = weaponData.fireSound;
        _fireAudioSource.loop = true;
        if (_fireAudioSource.isPlaying)
        {
            _fireAudioSource.Stop();
        }

        weaponFSM.ActivateState(WeaponStateEnum.Idle);
    }

    void Update()
    {
        fireCooldown -= Time.deltaTime;
        weaponFSM.Update(); // Обновление текущего состояния FSM
    }

    public bool CanFire()
    {
        return currentAmmo > 0 && fireCooldown <= 0f;
    }

    public void StartFiring()
    {
        if (!_fireAudioSource.isPlaying)
        {
            _fireAudioSource.Play();
        }
    }

    public void StopFiring()
    {
        if (_fireAudioSource.isPlaying)
        {
            _fireAudioSource.Stop();
        }
    }

    public void Shoot()
    {
        if (CanFire())
        {
            if (weaponData.projectilePrefab != null)
            {
                FireProjectile();
            }
            else
            {
                //FireHitscan();
            }

            currentAmmo--;
            PlayMuzzleEffect();
            PlayFireSound();

            currentAmmo--;
            fireCooldown = weaponData.fireRate;

            Debug.Log($"{weaponData.weaponName} fired! Ammo left: {currentAmmo}");
        }
        else
        {
            Debug.Log("Cannot fire: Out of ammo or on cooldown.");
        }
    }

    public void ReloadComplete()
    {
        int ammoNeeded = weaponData.magazineSize - currentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, totalAmmo);

        currentAmmo += ammoToReload;
        totalAmmo -= ammoToReload;

        Debug.Log($"{weaponData.weaponName} reloaded! Ammo: {currentAmmo}/{totalAmmo}");
    }

    public void StartReload()
    {
        if (currentAmmo < weaponData.magazineSize && totalAmmo > 0)
        {
            Debug.Log("Reloading...");
            weaponFSM.ActivateState(WeaponStateEnum.Reload);
        }
        else
        {
            Debug.Log("No need to reload.");
        }
    }

    public bool HasAmmo()
    {
        return currentAmmo > 0;
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
    //         var target = hit.collider.GetComponent<Target>();
    //         if (target != null)
    //         {
    //             target.TakeDamage(weaponData.damage);
    //         }
    //     }
    // }

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

    public void StartFire()
    {
        weaponFSM.StartFire();
    }

    public void StopFire()
    {
        weaponFSM.StopFire();
    }

    public void Reload()
    {
        weaponFSM.Reload();
    }

}
