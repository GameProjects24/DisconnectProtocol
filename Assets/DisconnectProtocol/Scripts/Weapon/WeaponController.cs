using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private readonly List<Weapon> _weapons = new List<Weapon>();
    public List<WeaponData> _data; // Список данных оружия (ScriptableObject)
    private Weapon _currentWeapon;

    private void Awake()
    {
        // Находим все оружия, уже находящиеся в объекте (например, предзагруженные в сцену)
        GetComponentsInChildren(true, _weapons);
        _weapons.ForEach(x => x.gameObject.SetActive(false));

        SetActiveWeapon(0);
    }

    private void Start()
    {
        // Инстанцируем оружия из списка данных и добавляем их в список _weapons
        foreach (var data in _data)
        {
            var go = Instantiate(data.prefab, transform);
            go.SetActive(false);
            if (go.TryGetComponent<Weapon>(out var weapon))
            {
                _weapons.Add(weapon);
            }
        }

        SetActiveWeapon(0);
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            _currentWeapon?.Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            _currentWeapon?.Reload();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            CycleWeapon();
        }
    }

    private void SetActiveWeapon(int weaponIndex)
    {
        if (weaponIndex < 0 || weaponIndex >= _weapons.Count)
            return;

        // Деактивируем текущее оружие
        if (_currentWeapon != null)
        {
            _currentWeapon.gameObject.SetActive(false);
        }

        // Активируем новое оружие
        _currentWeapon = _weapons[weaponIndex];
        _currentWeapon.gameObject.SetActive(true);
    }

    private void CycleWeapon()
    {
        int nextWeaponIndex = (_weapons.IndexOf(_currentWeapon) + 1) % _weapons.Count;
        SetActiveWeapon(nextWeaponIndex);
    }
}
