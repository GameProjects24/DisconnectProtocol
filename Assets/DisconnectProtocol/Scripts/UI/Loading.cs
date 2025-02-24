using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class Loading : MonoBehaviour
{
    private RectTransform rectComponent;
    private Image loadImage;
    private bool up;

    public float rotateTime = 1f;

    public float startSize = 0.02f;
    public float endSize = 0.8f;
    public float openSpeed = 0.005f;
    public float closeSpeed = 0.01f;
    private float currentSize;

    private void Start()
    {
        rectComponent = GetComponent<RectTransform>();
        loadImage = rectComponent.GetComponent<Image>();
        up = true;

        rectComponent.DORotate(new Vector3(0f, 0f, 360f), rotateTime, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.Linear)
            .SetUpdate(UpdateType.Normal, true);
    }

    private void Update()
    {
        changeSize();
    }

    private void changeSize()
    {
        if (rectComponent == null || loadImage == null) return;
        currentSize = loadImage.fillAmount;

        if (currentSize < endSize && up)
        {
            loadImage.DOFillAmount(loadImage.fillAmount + openSpeed, openSpeed).SetUpdate(UpdateType.Normal, true);
        }
        else if (currentSize >= endSize && up)
        {
            up = false;
        }
        else if (currentSize >= startSize && !up)
        {
            loadImage.DOFillAmount(loadImage.fillAmount - closeSpeed, closeSpeed).SetUpdate(UpdateType.Normal, true);
        }
        else if (currentSize < startSize && !up)
        {
            up = true;
        }
    }
}
