using UnityEngine;

public class WeaponLookAtCenter : MonoBehaviour
{
    private WeaponTransformManager transformManager;
    public WeaponController weaponController;
    private Camera mainCamera;

    public float raycastDistance = 50f;
    public float minHitDistance = 2f;

    public float smoothRot = 12f;

    private Transform currentMuzzle;
    private Quaternion currentLookOffset = Quaternion.identity;

    private void Start()
    {
        mainCamera = Camera.main;
        transformManager = GetComponent<WeaponTransformManager>();
        weaponController.OnChangeWeapon += OnWeaponChanged;
    }

    private void OnDestroy()
    {
        weaponController.OnChangeWeapon -= OnWeaponChanged;
    }

    public void OnWeaponChanged(Weapon currentWeapon)
    {
        if (currentWeapon != null)
        {
            currentMuzzle = currentWeapon.GetMuzzle();
            if (currentMuzzle == null)
            {
                Debug.LogWarning("На оружии " + currentWeapon.name + " не найден объект 'Muzzle'.");
                currentMuzzle = null;
            }
        }
        else
        {
            currentMuzzle = null;
        }
    }

    private void Update()
    {
        if (mainCamera == null || transformManager == null || currentMuzzle == null)
            return;

        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        Vector3 aimPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
        {
            // Если расстояние меньше минимального, используем минимальное
            aimPoint = mainCamera.transform.position
                       + mainCamera.transform.forward
                       * Mathf.Max(hit.distance, minHitDistance);
        }
        else
        {
            aimPoint = mainCamera.transform.position + mainCamera.transform.forward * raycastDistance;
        }

        // Вычисляем направление от позиции Muzzle к точке попадания
        Vector3 direction = (aimPoint - currentMuzzle.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        Quaternion parentRotation = currentMuzzle.parent.rotation;

        Quaternion targetOffset = Quaternion.Inverse(transformManager.DefaultRotation) *
                                    Quaternion.Inverse(parentRotation) *
                                    targetRotation;

        // Плавно интерполируем текущее смещение к вычисленному
        currentLookOffset = Quaternion.Slerp(currentLookOffset, targetOffset, Time.deltaTime * smoothRot);

        // Преобразуем в Euler-углы (как ожидает метод AddRotationOffset) и передаем в менеджер
        Vector3 offsetEuler = currentLookOffset.eulerAngles;
        transformManager.AddRotationOffset(currentLookOffset);
    }
}
