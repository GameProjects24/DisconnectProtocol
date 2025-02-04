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
    public bool isAutomatic = true; // Автоматический или одиночный режим стрельбы
    public bool isAutoReload;

    [Header("Ammo Settings")]
    public int cageSize = 30; // Количество патронов в магазине
    public int maxAmmo = 120; // Максимальный запас патронов
    public float reloadTime
    {
        get
        {
            return reloadAnim != null ? reloadAnim.length : 2f; // Длительность анимации или значение по умолчанию
        }
    }

    [Header("Recoil")]
    public Vector3 recoilKickback = new Vector3(0f, 0.1f, -0.1f);
    public float recoilRecoveryTime = 0.2f;

    [Header("Effects")]
    public GameObject muzzleFlashEffect;
    public AudioClip fireSound;
    public AudioClip reloadSound;
    public AnimationClip reloadAnim;

    [Header("Shoot Settings")]
    public WeaponShootSO weaponShoot;
}
