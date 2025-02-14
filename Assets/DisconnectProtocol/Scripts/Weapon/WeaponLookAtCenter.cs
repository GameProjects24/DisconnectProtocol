using UnityEngine;

public class WeaponLookAtCenter : MonoBehaviour
{
    [Header("Ссылки")]
    private WeaponTransformManager transformManager;
    private Camera mainCamera;

    [Header("Настройки Raycast")]
    public float raycastDistance = 50f;

    [Header("Настройки сглаживания")]
    public float smoothRot = 12f;

    private Quaternion currentLookOffset = Quaternion.identity;

    private void Start()
    {
        mainCamera = Camera.main;
        transformManager = GetComponent<WeaponTransformManager>();
        
    }

    private void Update()
    {
        if (mainCamera == null || transformManager == null)
            return;

        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        Vector3 aimPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
        {
            aimPoint = hit.point;
        }
        else
        {
            aimPoint = mainCamera.transform.position + mainCamera.transform.forward * raycastDistance;
        }

        // Вычисляем направление от оружия к точке попадания
        Vector3 direction = (aimPoint - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        Quaternion headRotation = transform.parent.rotation;

        Quaternion targetOffset = Quaternion.Inverse(transformManager.DefaultRotation) * Quaternion.Inverse(headRotation) * targetRotation;

        currentLookOffset = Quaternion.Slerp(currentLookOffset, targetOffset, Time.deltaTime * smoothRot);

        // Преобразуем в Euler-углы (как ожидает WeaponTransformManager) и передаем смещение
        Vector3 offsetEuler = currentLookOffset.eulerAngles;
        transformManager.AddRotationOffset(offsetEuler);
    }
}
