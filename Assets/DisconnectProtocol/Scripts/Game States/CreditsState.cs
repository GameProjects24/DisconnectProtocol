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
        base.OnEnter();
        creditsController.OnCreditsEnd += HandlerCreditsEnd;
		creditsController.gameObject.SetActive(true);
		uiPanel.SetActive(false);
		blackPanel.SetActive(false);

        _sequence?.Kill();
        _sequence = DOTween.Sequence().SetUpdate(UpdateType.Normal, true);
        _sequence.AppendCallback(() =>
            {
                musicManager = MusicManager.Instance;
                musicManager.ChangeMusic(creditsClip, panelFadeDuration);
                blackPanel.SetActive(true);
            });

        CanvasGroup blackPanelGroup = blackPanel.GetComponent<CanvasGroup>();
        if (blackPanelGroup != null)
        {
            _sequence.JoinCallback(() =>
            {
                blackPanelGroup.alpha = 0f;
                _sequence.Join(blackPanelGroup.DOFade(1f, panelFadeDuration).SetUpdate(UpdateType.Normal, true));
            });
        }

        if (uiPanel != null) uiPanel.SetActive(true);
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
