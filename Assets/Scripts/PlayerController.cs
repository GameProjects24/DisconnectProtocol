using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputActionAsset inputActions;
    private InputAction m_moveAction;
    private InputAction m_aimAction;
    public Player target;

    public CamerasManager camerasManager;

    public void SetTarget(Player target)
    {
        this.target = target;
        camerasManager.SetTarget(target.transform);
    }
    
    void Start()
    {
        var map = inputActions.FindActionMap("Player");
        map.Enable();

        m_moveAction = map.FindAction("Move");
        m_aimAction = map.FindAction("Aim");

        m_aimAction.performed += OnAimPerformed;
        m_aimAction.canceled += OnAimCanceled;
    }

    private void OnAimCanceled(InputAction.CallbackContext context)
    {
        camerasManager.ChangeCamera(CameraType.TPC);
    }

    private void OnAimPerformed(InputAction.CallbackContext context)
    {
        camerasManager.ChangeCamera(CameraType.Aim);
    }

    private void Update()
    {
        Vector2 move = m_moveAction.ReadValue<Vector2>();
        target.Move(move);
    }
}
