using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenAnimController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI gameOverText;
    public GameObject Button1;
    public GameObject Button2;
    private CanvasGroup Button1CanvasGroup;
    private CanvasGroup Button2CanvasGroup;

    [Header("Glitch Panel")]
    public GameObject glitchPanel;
    private Material glitchMaterial;
    private Image glitchImage;

    [Header("BG Panel")]
    public GameObject bgPanel;
    private Material bgMaterial;
    private Image bgImage;
    private float bgStartAlfa;


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

        Button1CanvasGroup = Button1.GetComponent<CanvasGroup>();
        Button2CanvasGroup = Button2.GetComponent<CanvasGroup>();
        Button1CanvasGroup.alpha = 0f;
        Button2CanvasGroup.alpha = 0f;
        Button1.SetActive(false);
        Button2.SetActive(false);

        if (bgPanel != null)
        {
            bgImage = bgPanel.GetComponent<Image>();
            bgMaterial = bgImage.material;
            bgStartAlfa = bgImage.color.a;
            // Обнуляем прозрачность (alfa)
            bgImage.color = new Color (bgImage.color.r, bgImage.color.g, bgImage.color.b, 0f);
        }

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

        // Если есть гличи, показываем их анимированно
        if (glitchPanel != null)
        {
            sequence.AppendCallback(() =>
            {
                glitchPanel.SetActive(true);
                glitchMaterial.SetFloat("_DeadZone", 0f);
            }).SetUpdate(UpdateType.Normal, true);

            sequence.Append(
                DOTween.To(
                    () => glitchMaterial.GetFloat("_DeadZone"),
                    x => glitchMaterial.SetFloat("_DeadZone", x),
                    1f,
                    initialDelay
                ).SetUpdate(UpdateType.Normal, true)
            );

            sequence.AppendCallback(() =>
            {
                glitchPanel.SetActive(false);
            }).SetUpdate(UpdateType.Normal, true);
        }

        // Если есть задний фон, показываем его анимированно
        else if (bgPanel != null)
        {
            sequence.Append(
                bgImage.DOFade(bgStartAlfa, initialDelay)
                .SetUpdate(UpdateType.Normal, true)
            );
        }

        else
        {
            sequence.AppendInterval(initialDelay).SetUpdate(UpdateType.Normal, true);
        }

        // Эффект печатания текста
        sequence.AppendCallback(() => StartCoroutine(TypeText()));
        sequence.AppendInterval(typeDuration);

        // Появление кнопок
        sequence.AppendCallback(() =>
        {
            Button1.SetActive(true);
            Button2.SetActive(true);
        });
        sequence.Append(Button1CanvasGroup.DOFade(1f, buttonsFadeDuration).SetUpdate(UpdateType.Normal, true));
        sequence.Join(Button2CanvasGroup.DOFade(1f, buttonsFadeDuration).SetUpdate(UpdateType.Normal, true));
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
