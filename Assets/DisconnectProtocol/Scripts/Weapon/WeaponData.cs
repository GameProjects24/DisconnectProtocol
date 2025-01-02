using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Basic Info")]
    
    public GameObject prefab;
    public string weaponName;
    public Sprite weaponIcon;

    [Header("Weapon Stats")]
    public float damage = 10f;
    public float fireRate = 0.5f; // Время между выстрелами
    public float range = 50f;

    [Header("Ammo Settings")]
    public int magazineSize = 30; // Количество патронов в магазине
    public int maxAmmo = 120; // Максимальный запас патронов
    public float reloadTime = 2f;

    [Header("Recoil")]
    public Vector3 recoilKickback = new Vector3(0f, 0.1f, -0.1f);
    public float recoilRecoveryTime = 0.2f;

    [Header("Effects")]
    public GameObject muzzleFlashEffect;
    public AudioClip fireSound;
    public AudioClip reloadSound;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 20f;
    
    public bool isAutomatic = true; // Автоматический или одиночный режим стрельбы
}
