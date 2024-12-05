using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputActionAsset inputActions;
    private InputAction m_moveAction;
    private InputAction m_lookAction;
    private InputAction m_sprintAction;

    public Player target;

    public void SetTarget(Player target)
    {
        this.target = target;
    }

    void Start()
    {
        var map = inputActions.FindActionMap("Player");
        map.Enable();

        m_moveAction = map.FindAction("Move");
        m_lookAction = map.FindAction("Look");
        m_sprintAction = map.FindAction("Sprint");

        m_sprintAction.performed += OnSprintPerformed;
        m_sprintAction.canceled += OnSprintCanceled;
    }

    private void OnSprintPerformed(InputAction.CallbackContext context)
    {
        target.SetSprint(true);
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        target.SetSprint(false);
    }

    private void Update()
    {
        // Чтение ввода
        Vector2 moveInput = m_moveAction.ReadValue<Vector2>();
        Vector2 lookInput = m_lookAction.ReadValue<Vector2>();

        // Передвижение и поворот камеры
        target.Move(moveInput);
        target.Look(lookInput);
    }
}
