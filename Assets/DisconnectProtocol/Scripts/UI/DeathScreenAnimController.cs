using DG.Tweening;
using TMPro;
using UnityEngine;
using System.Collections;

public class DeathScreenAnimController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI gameOverText;
    public GameObject restartButton;
    public GameObject mainMenuButton;
    private CanvasGroup restartButtonCanvasGroup;
    private CanvasGroup mainMenuButtonCanvasGroup;

    [Header("Timing Settings")]
    public float initialDelay = 1f;       // Задержка перед началом эффекта печатания
    public float typeDuration = 2f; // Длительность эффекта печатания текста
    public float buttonsFadeDuration = 1f; // Длительность появления кнопок

    private string fullText;
    private int totalCharacters;
    private float timePerCharacter;

    private void OnEnable()
    {
        fullText = gameOverText.text;
        // Изначально скрываем текст и кнопки.
        gameOverText.text = "";

        restartButtonCanvasGroup = restartButton.GetComponent<CanvasGroup>();
        mainMenuButtonCanvasGroup = mainMenuButton.GetComponent<CanvasGroup>();
        restartButtonCanvasGroup.alpha = 0f;
        mainMenuButtonCanvasGroup.alpha = 0f;
        restartButton.SetActive(false);
        mainMenuButton.SetActive(false);
        
        ShowDeathScreen();
    }

    // Этот метод вызывается, когда наступает экран смерти.
    public void ShowDeathScreen()
    {
        // Создаем последовательность DOTween
        Sequence sequence = DOTween.Sequence();

        // Ждем initialDelay секунд
        sequence.AppendInterval(initialDelay);


        // Эффект печатания текста
        sequence.AppendCallback(() => StartCoroutine(TypeText()));
        sequence.AppendInterval(typeDuration + initialDelay);


        restartButton.SetActive(true);
        mainMenuButton.SetActive(true);
        sequence.Append(restartButtonCanvasGroup.DOFade(1f, buttonsFadeDuration));
        sequence.Join(mainMenuButtonCanvasGroup.DOFade(1f, buttonsFadeDuration));
    }

    private IEnumerator TypeText()
    {
        totalCharacters = fullText.Length;
        timePerCharacter = typeDuration / totalCharacters;

        for (int i = 0; i <= totalCharacters; i++)
        {
            gameOverText.text = fullText.Substring(0, i);
            yield return new WaitForSeconds(timePerCharacter);
        }
    }
}
