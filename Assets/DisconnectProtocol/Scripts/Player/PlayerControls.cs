using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
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
    public bool pause;

    [Header("Movement Settings")]
    public bool analogMovement;

#if ENABLE_INPUT_SYSTEM
    public static PlayerControls Instance { get; private set; }

    private PlayerInputs _inputActions;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _inputActions = new PlayerInputs();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        _inputActions.Enable();

        // Move
        _inputActions.Gameplay.Move.performed += context => move = context.ReadValue<Vector2>();
        _inputActions.Gameplay.Move.canceled += context => move = Vector2.zero;

        // Look
        _inputActions.Gameplay.Look.performed += context => look = context.ReadValue<Vector2>();
        _inputActions.Gameplay.Look.canceled += context => look = Vector2.zero;

        // Jump
        _inputActions.Gameplay.Jump.performed += context => jump = context.ReadValueAsButton();
        _inputActions.Gameplay.Jump.canceled += context => jump = false;

        // Sprint
        _inputActions.Gameplay.Sprint.performed += context => sprint = context.ReadValueAsButton();
        _inputActions.Gameplay.Sprint.canceled += context => sprint = false;

        // Fire
        _inputActions.Gameplay.Fire.performed += context => fire = context.ReadValueAsButton();
        _inputActions.Gameplay.Fire.canceled += context => fire = false;

        // Reload
        _inputActions.Gameplay.Reload.performed += context => reload = context.ReadValueAsButton();
        _inputActions.Gameplay.Reload.canceled += context => reload = false;

        // Change weapon
        _inputActions.Gameplay.ChangeWeapon.performed += context => changeWeapon = context.ReadValueAsButton();
        _inputActions.Gameplay.ChangeWeapon.canceled += context => changeWeapon = false;

        // Aim
        _inputActions.Gameplay.Aim.performed += context => aim = context.ReadValueAsButton();
        _inputActions.Gameplay.Aim.canceled += context => aim = false;

        // Interact
        _inputActions.Gameplay.Interact.performed += context => interact = context.ReadValueAsButton();
        _inputActions.Gameplay.Interact.canceled += context => interact = false;

        // Pause
        _inputActions.UI.Pause.performed += context => pause = context.ReadValueAsButton();
        _inputActions.UI.Pause.canceled += context => pause = false;
    }

    public void SetInputMapState(string mapName, bool enable)
    {
        if (_inputActions == null) return;

        var actionMap = _inputActions.asset.FindActionMap(mapName);
        if (actionMap != null)
        {
            if (enable)
                actionMap.Enable();
            else
                actionMap.Disable();
        }
        else
        {
            Debug.LogWarning($"Input Action Map '{mapName}' not found!");
        }
    }

    private void OnDisable() => _inputActions.Disable();

#endif
}
