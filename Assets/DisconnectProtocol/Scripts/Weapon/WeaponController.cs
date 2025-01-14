using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Data")]
    [SerializeField] private List<WeaponData> weaponDataList; // Список данных для создания оружия

    private List<Weapon> weapons = new List<Weapon>(); // Список оружия
    private Weapon currentWeapon; // Текущее активное оружие

    private int currentWeaponIndex = -1; // Индекс текущего оружия (-1, если оружие не выбрано)

    private void Awake()
    {
        // Инициализация списка оружия
        foreach (var weaponData in weaponDataList)
        {
            var weaponPrefab = weaponData.prefab;
            if (weaponPrefab != null)
            {
                var weaponInstance = Instantiate(weaponPrefab, transform);
                var weaponComponent = weaponInstance.GetComponent<Weapon>();
                if (weaponComponent != null)
                {
                    weaponComponent.weaponData = weaponData;
                    weaponComponent.gameObject.SetActive(false); // Оружие неактивно до выбора
                    weapons.Add(weaponComponent);
                }
            }
        }

        // Установка первого оружия как активного
        SetActiveWeapon(0);
    }

    public void SetActiveWeapon(int index)
    {
        if (index >= 0 && index < weapons.Count)
        {
            // Деактивация текущего оружия
            if (currentWeapon != null)
            {
                currentWeapon.gameObject.SetActive(false);
            }

            // Активация нового оружия
            currentWeapon = weapons[index];
            currentWeapon.gameObject.SetActive(true);
            currentWeaponIndex = index;

            Debug.Log($"Active weapon set to: {currentWeapon.weaponData.weaponName}");
        }
        else
        {
            Debug.LogWarning($"Invalid weapon index: {index}");
        }
    }

    public void NextWeapon()
    {
        int nextIndex = (currentWeaponIndex + 1) % weapons.Count;
        SetActiveWeapon(nextIndex);
    }

    public void StartFire()
    {
        currentWeapon?.StartFire();
    }

    public void StopFire()
    {
        currentWeapon?.StopFire();
    }

    public void Reload()
    {
        currentWeapon?.Reload();
    }

    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }
}
