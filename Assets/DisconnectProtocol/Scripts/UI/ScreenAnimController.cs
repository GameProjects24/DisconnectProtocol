using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class ScreenAnimController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI labelText;
    public GameObject buttonsGroup;
    private CanvasGroup buttonsCanvasGroup;

    [Header("Glitch Panel")]
    public GameObject glitchPanel;
    private Material glitchMaterial;
    private Image glitchImage;

    [Header("BG Panel")]
    public GameObject bgPanel;
    private Image bgImage;
    private float bgStartAlpha;

    [Header("Timing Settings")]
    public float initialDelay = 1f;         // Задержка перед анимацией 
    public float typeDuration = 2f;         // Длительность эффекта печатания
    public float buttonsFadeDuration = 1f;  // Длительность появления кнопок

    private string fullText;
    private int totalCharacters;
    private float timePerCharacter;

    private Sequence _sequence;
    private Coroutine typeTextCoroutine;

    private void OnEnable()
    {
        fullText = labelText.text;
        labelText.text = "";

        buttonsCanvasGroup = buttonsGroup.GetComponent<CanvasGroup>();
        if (buttonsCanvasGroup != null)
            buttonsCanvasGroup.alpha = 0f;
        buttonsGroup.SetActive(false);

        if (bgPanel != null)
        {
            bgImage = bgPanel.GetComponent<Image>();
            bgStartAlpha = bgImage.color.a;
            // Обнуляем прозрачность (alfa)
            bgImage.color = new Color(bgImage.color.r, bgImage.color.g, bgImage.color.b, 0f);
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
        _sequence?.Kill();
        _sequence = DOTween.Sequence().SetUpdate(UpdateType.Normal, true);

        // Если есть гличи, показываем их анимированно
        if (glitchPanel != null)
        {
            _sequence.AppendCallback(() =>
            {
                glitchPanel.SetActive(true);
                glitchMaterial.SetFloat("_DeadZone", 0f);
            });
            _sequence.Append(
                DOTween.To(
                    () => glitchMaterial.GetFloat("_DeadZone"),
                    x => glitchMaterial.SetFloat("_DeadZone", x),
                    1f,
                    initialDelay
                )
            );
            _sequence.AppendCallback(() =>
            {
                glitchPanel.SetActive(false);
            });
        }
        
        // Если есть задний фон, показываем его анимированно
        if (bgPanel != null)
        {
            _sequence.Append(bgImage.DOFade(bgStartAlpha, initialDelay));
        }
        else
        {
            _sequence.AppendInterval(initialDelay);
        }

        // Эффект печатания текста
        if (gameObject.activeInHierarchy)
        {
            _sequence.AppendCallback(() =>
            {
                if (typeTextCoroutine != null)
                {
                    StopCoroutine(typeTextCoroutine);
                }
                typeTextCoroutine = StartCoroutine(TypeText());
            });
            _sequence.AppendInterval(typeDuration);
        }

        _sequence.AppendCallback(() => { buttonsGroup.SetActive(true); });
        _sequence.Join(buttonsCanvasGroup.DOFade(1f, buttonsFadeDuration).SetUpdate(UpdateType.Normal, true));
    }

    private void OnDisable()
    {
        _sequence?.Kill();
        StopAllCoroutines();
        EventSystem.current.SetSelectedGameObject(null);
        labelText.text = fullText;
        if (bgImage != null)
            bgImage.color = new Color(bgImage.color.r, bgImage.color.g, bgImage.color.b, bgStartAlpha);
    }

    private IEnumerator TypeText()
    {
        totalCharacters = fullText.Length;
        timePerCharacter = typeDuration / totalCharacters;
        for (int i = 0; i <= totalCharacters; i++)
        {
            labelText.text = fullText.Substring(0, i);
            yield return new WaitForSecondsRealtime(timePerCharacter);
        }
    }
}
