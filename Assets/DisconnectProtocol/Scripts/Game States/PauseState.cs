using UnityEngine;

public class PauseState : GameState
{
    [Header("State References")]

    public GameObject uiPanel;

    private MusicManager musicManager;

    public override void OnEnter()
    {
        base.OnEnter();
        Time.timeScale = 0f;

        musicManager = MusicManager.Instance;
        musicManager.FadeOutAndPause(0.5f);

        if (uiPanel != null) uiPanel.SetActive(true);
    }

    public override void OnExit()
    {
        if (uiPanel != null) uiPanel.SetActive(false);

        musicManager.ResumeAndFadeIn(0.5f);

        base.OnExit();
    }
}
