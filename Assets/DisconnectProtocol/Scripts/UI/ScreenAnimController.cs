using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenAnimController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI gameOverText;
    public GameObject restartButton;
    public GameObject mainMenuButton;
    private CanvasGroup restartButtonCanvasGroup;
    private CanvasGroup mainMenuButtonCanvasGroup;

    [Header("Glitch Panel")]
    public GameObject glitchPanel; // Тот самый объект с шейдером
    private Material glitchMaterial;
    private Image glitchImage;

    [Header("Timing Settings")]
    public float initialDelay = 1f;       // Задержка перед анимацией шейдера и печатанием
    public float typeDuration = 2f;       // Длительность эффекта печатания текста
    public float buttonsFadeDuration = 1f;// Длительность появления кнопок

    private string fullText;
    private int totalCharacters;
    private float timePerCharacter;

    private void OnEnable()
    {
        fullText = gameOverText.text;
        gameOverText.text = "";

        restartButtonCanvasGroup = restartButton.GetComponent<CanvasGroup>();
        mainMenuButtonCanvasGroup = mainMenuButton.GetComponent<CanvasGroup>();
        restartButtonCanvasGroup.alpha = 0f;
        mainMenuButtonCanvasGroup.alpha = 0f;
        restartButton.SetActive(false);
        mainMenuButton.SetActive(false);

        if (glitchPanel != null)
        {

            glitchImage = glitchPanel.GetComponent<Image>();
            glitchMaterial = glitchImage.material;

        }

        ShowScreen();
    }

    public void ShowScreen()
    {
        Sequence sequence = DOTween.Sequence();

        if (glitchPanel != null)
        {
            sequence.AppendCallback(() =>
            {
                glitchPanel.SetActive(true);
                glitchMaterial.SetFloat("_DeadZone", 0f);
            });

            sequence.Append(
                DOTween.To(
                    () => glitchMaterial.GetFloat("_DeadZone"),
                    x => glitchMaterial.SetFloat("_DeadZone", x),
                    1f,
                    initialDelay
                )
                .SetUpdate(true)
            );

            sequence.AppendCallback(() =>
            {
                glitchPanel.SetActive(false);
            });
        }
        else
        {
            sequence.AppendInterval(initialDelay);
        }

        // Эффект печатания текста
        sequence.AppendCallback(() => StartCoroutine(TypeText()));
        sequence.AppendInterval(typeDuration);

        // Появление кнопок
        sequence.AppendCallback(() =>
        {
            restartButton.SetActive(true);
            mainMenuButton.SetActive(true);
        });
        sequence.Append(restartButtonCanvasGroup.DOFade(1f, buttonsFadeDuration).SetUpdate(true));
        sequence.Join(mainMenuButtonCanvasGroup.DOFade(1f, buttonsFadeDuration).SetUpdate(true));
    }

    private IEnumerator TypeText()
    {
        totalCharacters = fullText.Length;
        timePerCharacter = typeDuration / totalCharacters;

        for (int i = 0; i <= totalCharacters; i++)
        {
            gameOverText.text = fullText.Substring(0, i);
            yield return new WaitForSecondsRealtime(timePerCharacter);
        }
    }
}
