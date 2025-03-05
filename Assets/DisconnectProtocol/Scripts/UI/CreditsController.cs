using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class CreditsController : MonoBehaviour
{
    [Header("Canvas Groups")]
    public CanvasGroup firstCanvasGroup;
    public float firstFadeDuration = 1f;
    public CanvasGroup secondCanvasGroup;
    public float secondFadeDuration = 1f;
    
    [Header("Scrolling Settings")]
    public RectTransform creditsText;
    public float scrollSpeed = 50f;
    public float acceleratedSpeed = 150f;
    public float waitBeforeScroll = 2f;
    public float targetYPosition;

    private Tween scrollTween;
    private bool isAccelerated = false;
    private PlayerControls playerControls;

    private void Start()
    {
        playerControls = PlayerControls.Instance;
    }

    private void OnEnable()
    {
        // Сброс начальных состояний
        firstCanvasGroup.alpha = 0f;
        secondCanvasGroup.alpha = 0f;
        creditsText.anchoredPosition = new Vector2(creditsText.anchoredPosition.x, 0);

        StartCreditsSequence();
    }

    private void StartCreditsSequence()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(firstCanvasGroup.DOFade(1f, firstFadeDuration).SetUpdate(true));
        sequence.Append(secondCanvasGroup.DOFade(1f, secondFadeDuration).SetUpdate(true));
        sequence.AppendInterval(waitBeforeScroll).OnComplete(StartScrolling);
    }

    private void StartScrolling()
    {
        float distance = targetYPosition - creditsText.anchoredPosition.y;
        float duration = distance / scrollSpeed;
        scrollTween?.Kill();
        scrollTween = DOTween.To(
            () => creditsText.anchoredPosition.y,
            y => creditsText.anchoredPosition = new Vector2(creditsText.anchoredPosition.x, y),
            targetYPosition,
            duration
        )
        .SetEase(Ease.Linear)
        .SetUpdate(true);
    }

    private void Update()
    {
        if (playerControls == null)
            return;

        if (playerControls.speedUpScroll)
        {
            if (!isAccelerated)
            {
                isAccelerated = true;
                RestartScrollTween(acceleratedSpeed);
            }
        }
        else
        {
            if (isAccelerated)
            {
                isAccelerated = false;
                RestartScrollTween(scrollSpeed);
            }
        }
    }

    private void RestartScrollTween(float newSpeed)
    {
        // Получаем текущую позицию и пересчитываем оставшуюся дистанцию и длительность tween'а
        float currentY = creditsText.anchoredPosition.y;
        float distance = targetYPosition - currentY;
        if (distance <= 0) return; // Если уже достигнута цель
        
        float duration = distance / newSpeed;
        scrollTween?.Kill();
        scrollTween = DOTween.To(
            () => creditsText.anchoredPosition.y,
            y => creditsText.anchoredPosition = new Vector2(creditsText.anchoredPosition.x, y),
            targetYPosition,
            duration
        )
        .SetEase(Ease.Linear)
        .SetUpdate(true);
    }
}
