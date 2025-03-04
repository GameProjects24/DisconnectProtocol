using UnityEngine;

public class PauseState : GameState
{
    [Header("State References")]

    public GameObject uiPanel;

    public override void OnEnter()
    {
        base.OnEnter();
        Time.timeScale = 0f;

        if (uiPanel != null) uiPanel.SetActive(true);
    }

    public override void OnExit()
    {
        if (uiPanel != null) uiPanel.SetActive(false);

        base.OnExit();
    }
}
