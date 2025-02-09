using UnityEngine;

public class WeaponTransformManager : MonoBehaviour
{
    public Vector3 DefaultPosition { get; private set; }
    public Quaternion DefaultRotation { get; private set; }
    
    private Vector3 targetPosition;
    private Vector3 positionOffset;
    private Vector3 rotationOffset;

    private Vector3 velocity = Vector3.zero;
    public float smooth = 10f;
    public float smoothRot = 12f;

    private void Awake()
    {
        DefaultPosition = transform.localPosition;
        DefaultRotation = transform.localRotation;
    }

    private void Update()
    {
        ApplyPositionAndRotation();
    }

    public void SetTargetPosition(Vector3 newTargetPosition)
    {
        targetPosition = newTargetPosition;
    }

    public void AddPositionOffset(Vector3 offset)
    {
        positionOffset += offset;
    }

    public void AddRotationOffset(Vector3 offset)
    {
        rotationOffset += offset;
    }

    private void ApplyPositionAndRotation()
    {
        // **Складываем все смещения (sway + bobbing + aim)**
        Vector3 finalPosition = targetPosition + positionOffset;
        Quaternion finalRotation = Quaternion.Euler(rotationOffset) * DefaultRotation;

        // **Плавное движение и поворот**
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPosition, ref velocity, 0.1f);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRotation, Time.deltaTime * smoothRot);

        // **Очищаем смещения после каждого кадра, чтобы они обновлялись заново**
        positionOffset = Vector3.zero;
        rotationOffset = Vector3.zero;
    }
}
