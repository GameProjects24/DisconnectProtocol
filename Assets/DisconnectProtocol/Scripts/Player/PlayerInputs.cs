using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerInputs : MonoBehaviour
{
    [Header("Character Input Values")]
    public Vector2 move;       // Движение
    public Vector2 look;       // Повороты камеры
    public bool jump;          // Прыжок
    public bool sprint;        // Спринт
    public bool fire;          // Стрельба
    public bool reload;        // Перезарядка
    public bool changeWeapon;    // Переключение на следующее оружие
    public bool aim;

    [Header("Movement Settings")]
    public bool analogMovement;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
    // Движение
    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    // Вращение камеры
    public void OnLook(InputValue value)
    {
        if (cursorInputForLook)
        {
            LookInput(value.Get<Vector2>());
        }
    }

    // Прыжок
    public void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);
    }

    // Спринт
    public void OnSprint(InputValue value)
    {
        SprintInput(value.isPressed);
    }

    // Стрельба
    public void OnFire(InputValue value)
    {
        FireInput(value.isPressed);
    }

    // Перезарядка
    public void OnReload(InputValue value)
    {
        ReloadInput(value.isPressed);
    }

    // Смена оружия
    public void OnChangeWeapon(InputValue value)
    {
        ChangeWeaponInput(value.isPressed);
    }

    public void OnAim(InputValue value)
    {
        AimInput(value.isPressed);
    }
#endif

    // Обработка движения
    public void MoveInput(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    }

    // Обработка вращения камеры
    public void LookInput(Vector2 newLookDirection)
    {
        look = newLookDirection;
    }

    // Обработка прыжка
    public void JumpInput(bool newJumpState)
    {
        jump = newJumpState;
    }

    // Обработка спринта
    public void SprintInput(bool newSprintState)
    {
        sprint = newSprintState;
    }

    // Обработка стрельбы
    public void FireInput(bool newFireState)
    {
        fire = newFireState;
    }

    // Обработка перезарядки
    public void ReloadInput(bool newReloadState)
    {
        reload = newReloadState;
    }

    // Обработка смены оружия
    public void ChangeWeaponInput(bool newChangeWeaponState)
    {
        changeWeapon = newChangeWeaponState;
    }

    private void AimInput(bool newAimState)
    {
        aim = newAimState;
    }

    // Фокус приложения и настройка курсора
    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    // Установка состояния курсора
    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
