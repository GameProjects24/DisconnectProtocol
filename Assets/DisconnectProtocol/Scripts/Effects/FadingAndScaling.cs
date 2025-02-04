using UnityEngine;
using DG.Tweening;

public class FadingAndScaling : MonoBehaviour
{
    public Vector3 startScale = Vector3.zero;
    public Vector3 targetScale = Vector3.one;
    public float duration = 2f;
    public Color startColor = Color.white;
    public Color endColor = new Color(1, 1, 1, 0);

    private Renderer objectRenderer;
    private Material material;

    private void OnEnable()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer == null)
        {
            Debug.LogError("Нет компонента Renderer на объекте!", gameObject);
            return;
        }

        material = objectRenderer.material;
        material.color = startColor;
        transform.localScale = startScale;

        // Анимация масштаба
        var scaleTween = transform.DOScale(targetScale, duration).SetEase(Ease.OutQuad);

        // Анимация цвета и прозрачности
        var colorTween = material.DOColor(endColor, duration).SetEase(Ease.OutQuad);

        // Отложенное удаление объекта, только когда завершены все анимации
        scaleTween.OnKill(() => Destroy(gameObject));
        colorTween.OnKill(() => Destroy(gameObject));
    }
}
