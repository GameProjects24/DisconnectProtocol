using UnityEngine;

public class WeaponTransformManager : MonoBehaviour
{
    public Vector3 DefaultPosition { get; private set; }
    public Quaternion DefaultRotation { get; private set; }

    private Vector3 targetPosition;
    public Quaternion targetRotation;

    private Vector3 positionOffset;
    private Quaternion rotationOffset;

    private Vector3 velocity = Vector3.zero;
    public float smoothRot = 12f;

    private void Awake()
    {
        DefaultPosition = transform.localPosition;
        DefaultRotation = transform.localRotation;

        targetPosition = DefaultPosition;
        targetRotation = DefaultRotation;
    }

    private void Update()
    {
        ApplyTransform();
    }

    public void SetTargetPosition(Vector3 newTargetPosition)
    {
        targetPosition = newTargetPosition;
    }

    public void SetTargetRotation(Quaternion newTargetRotation)
    {
        targetRotation = newTargetRotation;
    }

    public void AddPositionOffset(Vector3 offset)
    {
        positionOffset += offset;
    }

    public void AddRotationOffset(Quaternion offset)
    {
        rotationOffset *= offset; // Корректное применение кватерниона
    }

    public void AddRotationOffset(Vector3 eulerOffset)
    {
        AddRotationOffset(Quaternion.Euler(eulerOffset)); // Конвертируем углы Эйлера в кватернион
    }

    private void ApplyTransform()
    {
        // Складываем все смещения
        Vector3 finalPosition = targetPosition + positionOffset;
        Quaternion finalRotation = targetRotation * rotationOffset;

        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPosition, ref velocity, 0.1f);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRotation, Time.deltaTime * smoothRot);

        // Очищаем смещения после каждого кадра
        positionOffset = Vector3.zero;
        rotationOffset = Quaternion.identity;
    }

}
