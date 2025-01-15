using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Data")]
    public WeaponData weaponData; // Данные оружия, включая информацию о патронах, уроне, скорости перезарядки и т.д.

    private WeaponFSM weaponFSM; // FSM для управления состояниями оружия
    private int currentAmmo; // Текущее количество патронов в оружии
    private bool isReloading; // Флаг, который указывает, что оружие в процессе перезарядки
    private bool isFiring; // Флаг, который указывает, что оружие стреляет
    private float fireTimer; // Таймер для автоматической стрельбы

    private void Start()
    {
        // Инициализация оружия
        currentAmmo = weaponData.magazineSize; // Заполнение магазина патронами
        weaponFSM = new WeaponFSM(this); // Инициализация FSM с текущим оружием
        weaponFSM.ActivateState(WeaponStateEnum.Idle); // Инициализация состояния "Idle"
    }

    private void Update()
    {
        weaponFSM.Update(); // Обновляем FSM (переходы между состояниями)

        // Если мы стреляем в автоматическом режиме (удерживаем кнопку)
        if (isFiring && CanFire())
        {
            fireTimer += Time.deltaTime; // Увеличиваем таймер

            if (fireTimer >= weaponData.fireRate) // Проверяем, прошло ли время между выстрелами
            {
                fireTimer = 0f; // Сбросить таймер
                StartShooting(); // Совершаем выстрел
            }
        }
    }

    // Проверка, можно ли стрелять
    public bool CanFire()
    {
        return currentAmmo > 0 && !isReloading;
    }

    // Проверка, можно ли перезаряжать оружие
    public bool CanReload()
    {
        return currentAmmo < weaponData.magazineSize && !isReloading;
    }

    // Метод для начала стрельбы
    public void StartShooting()
    {
        if (CanFire())
        {
            isFiring = true;
            Debug.Log("Shooting started...");
            currentAmmo--;
            // Тут можно добавить логику выстрела (эффекты, звук, пули и т.д.)
            // Например, если используем снаряды, создаём объект пули
            //Instantiate(weaponData.projectilePrefab, transform.position, transform.rotation);

            FireProjectile();
            // Если патроны закончились, переходим в состояние Empty
            if (currentAmmo <= 0)
            {
                weaponFSM.ActivateState(WeaponStateEnum.Empty);
            }
        }
        else
        {
            Debug.Log("No ammo or reloading in progress.");
        }
    }

    // Метод для остановки стрельбы
    public void StopShooting()
    {
        isFiring = false;
        Debug.Log("Shooting stopped...");
        // Возможно, нужно остановить какие-то эффекты или анимации выстрела
        weaponFSM.ActivateState(WeaponStateEnum.Idle); // Переход в состояние "Idle"
    }

    // Метод для начала перезарядки
    public void StartReloading()
    {
        if (CanReload())
        {
            isReloading = true;
            Debug.Log("Reloading started...");
            weaponFSM.ActivateState(WeaponStateEnum.Reload); // Переход в состояние "Reload"
        }
    }

    // Метод, вызываемый по завершении перезарядки
    public void CompleteReload()
    {
        currentAmmo = weaponData.magazineSize; // Заполняем магазин
        isReloading = false; // Сбрасываем флаг перезарядки
        Debug.Log("Reloading complete!");
        weaponFSM.ActivateState(WeaponStateEnum.Idle); // Возвращаемся в состояние "Idle"
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

    // Вспомогательные методы для активации состояний из других классов
    public void StartFire() => weaponFSM.StartFire();
    public void Reload() => weaponFSM.Reload();
}
