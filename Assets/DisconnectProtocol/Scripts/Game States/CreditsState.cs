using DG.Tweening;
using DisconnectProtocol;
using UnityEngine;

public class CreditsState : GameState
{
    [Header("State References")]
    public CreditsController creditsController;
    public GameObject uiPanel;
    public GameObject blackPanel;
    public AudioClip creditsClip;
    public float panelFadeDuration;

    private MusicManager musicManager;
    private Sequence _sequence;

    public override void OnEnter()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        base.OnEnter();
        creditsController.OnCreditsEnd += HandlerCreditsEnd;
        blackPanel.SetActive(false);
        CanvasGroup blackPanelGroup = blackPanel.GetComponent<CanvasGroup>();

        _sequence?.Kill();
        _sequence = DOTween.Sequence().SetUpdate(UpdateType.Normal, true);
        _sequence.AppendCallback(() =>
            {
                musicManager = MusicManager.Instance;
                musicManager.ChangeMusic(creditsClip, panelFadeDuration);
                blackPanel.SetActive(true);
                blackPanelGroup.alpha = 0f;
            });

        _sequence.Append(blackPanelGroup.DOFade(1f, panelFadeDuration));
        
        _sequence.AppendCallback(() =>
        {
            blackPanel.SetActive(false);
            uiPanel.SetActive(true);
        });
    }

    private void HandlerCreditsEnd()
    {
        OnExit();
    }

    public override void OnExit()
    {
        musicManager.FadeOut(panelFadeDuration);
        Destroy(transform.root.gameObject, panelFadeDuration);
        GameController.instance.ErasePlayerData();
        GameController.instance.ChangeScene("MainMenu");

        base.OnExit();
    }
}
