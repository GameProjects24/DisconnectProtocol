using System;
using UnityEngine;

public class PlayerWeaponAnimationController : MonoBehaviour
{
    private Animator _animator;
    private WeaponController _weaponController;
    private string _currentAnimation;

    private void Awake()
    {
        _weaponController = GetComponent<WeaponController>();

        if (_weaponController == null)
        {
            Debug.LogError("WeaponController not found!");
        }
    }

    private void OnEnable()
    {
        // Подписываемся на события оружия
        if (_weaponController != null)
        {
            _weaponController.OnReload += HandleReload;
            _weaponController.OnChangeWeapon += HandleOnChangeWeapon;
        }
    }

    private void OnDisable()
    {
        // Отписываемся от событий
        if (_weaponController != null)
        {
            _weaponController.OnReload -= HandleReload;
            _weaponController.OnChangeWeapon -= HandleOnChangeWeapon;
        }
    }

    private void HandleReload()
    {
        PlayAnimation("SimpleReloadAnim");
    }

    private void HandleOnChangeWeapon(Weapon newWeapon)
    {
        if (newWeapon != null)
        {
            // Получаем новый Animator от текущего оружия
            _animator = newWeapon.GetComponent<Animator>();
        }
    }

    private void PlayAnimation(string animation)
    {
        //if(_currentAnimation == animation) return;
    
        _animator.Play(animation);
        _currentAnimation = animation;
    }
}
