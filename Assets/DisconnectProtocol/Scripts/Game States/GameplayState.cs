using UnityEngine;
using DisconnectProtocol;
using UnityEngine.InputSystem;

public class GameplayState : GameState
{
    [Header("State References")]

    public GameObject uiPanel;
    public InputActionAsset inputActions;
    public InputActionMap inputMap;

    private void Awake()
    {
        inputMap = inputActions.FindActionMap("Gameplay");
        //uiActionMap = inputActions.FindActionMap("UI");
    }

    public override void OnEnter()
    {
        base.OnEnter();
        Time.timeScale = 1f;

        if (uiPanel != null) uiPanel.SetActive(true);
        if (inputMap != null) inputMap.Enable();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public override void OnExit()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        if (uiPanel != null) uiPanel.SetActive(false);
        if (inputMap != null) inputMap.Disable();

        base.OnExit();
    }
}
