using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerInputs : MonoBehaviour
{
    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;
    public bool fire;
    public bool reload;
    public bool changeWeapon;
    public bool aim;
	public bool interact;

    [Header("Movement Settings")]
    public bool analogMovement;

#if ENABLE_INPUT_SYSTEM
    // Движение
    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    // Вращение камеры
    public void OnLook(InputValue value)
    {
        LookInput(value.Get<Vector2>());
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

	public void OnInteract(InputValue value)
	{
		InteractInput(value.isPressed);
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

	private void InteractInput(bool newInteractState)
	{
		interact = newInteractState;
	}

    // Установка состояния курсора
    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
