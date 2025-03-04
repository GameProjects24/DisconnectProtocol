using UnityEngine;
using DisconnectProtocol;
using UnityEngine.InputSystem;

public class GameplayState : GameState
{
    [Header("State References")]

    public GameObject uiPanel;
    public PlayerControls controls;
    public string inputMapName = "Gameplay";

    public override void OnEnter()
    {
        base.OnEnter();
        Time.timeScale = 1f;

        if (uiPanel != null) uiPanel.SetActive(true);
        if (controls != null) controls.SetInputMapState(inputMapName, true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public override void OnExit()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        if (uiPanel != null) uiPanel.SetActive(false);
        if (controls != null) controls.SetInputMapState(inputMapName, false);

        base.OnExit();
    }
}
