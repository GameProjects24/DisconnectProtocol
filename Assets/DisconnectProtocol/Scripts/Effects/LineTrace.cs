using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineTrace : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public float fadeDuration = 2f; // Время, за которое объект станет прозрачным
    private float fadeTimer = 0f;

    void Start()
    {
        // Получаем компонент LineRenderer
        lineRenderer = GetComponent<LineRenderer>();
        
        // Убедимся, что LineRenderer имеет материал с поддержкой прозрачности
        if (lineRenderer.material.HasProperty("_Color"))
        {
            // Устанавливаем начальную прозрачность на 100%
            Color startColor = lineRenderer.material.color;
            startColor.a = 1f;
            lineRenderer.material.color = startColor;
        }
        else
        {
            Debug.LogError("Material of LineRenderer must support transparency (e.g., Standard Shader with Rendering Mode set to Transparent).");
        }
    }

    void Update()
    {
        // Увеличиваем таймер
        fadeTimer += Time.deltaTime;

        if (fadeTimer < fadeDuration)
        {
            // Рассчитываем текущую прозрачность
            float alpha = Mathf.Lerp(1f, 0f, fadeTimer / fadeDuration);

            // Обновляем прозрачность материала LineRenderer
            Color newColor = lineRenderer.material.color;
            newColor.a = alpha;
            lineRenderer.material.color = newColor;
        }
        else
        {
            // После завершения эффекта удаляем объект
            Destroy(gameObject);
        }
    }
}
