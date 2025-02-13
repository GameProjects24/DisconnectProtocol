using UnityEngine;

public class WeaponLookAtCenter : MonoBehaviour
{
    public Camera playerCamera;
    private WeaponTransformManager transformManager;

    [Header("Correction Settings")]
    public float maxCorrectionAngle = 5f;  // Максимальный угол коррекции
    public float correctionSpeed = 8f;     // Скорость поворота
    public float maxDistance = 50f;        // Максимальная дистанция для корректировки

    private Quaternion defaultRotation;

    private void Start()
    {
        transformManager = GetComponent<WeaponTransformManager>();
        defaultRotation = transformManager.DefaultRotation; // Запоминаем начальную ротацию
    }

    private void Update()
    {
        AdjustWeaponRotation();
    }

    private void AdjustWeaponRotation()
    {
        // Выпускаем луч из центра экрана
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        Quaternion targetRotation;
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            // Если попали в объект, направляем оружие к нему
            Vector3 directionToTarget = (hit.point - transform.position).normalized;
            targetRotation = Quaternion.LookRotation(directionToTarget);
        }
        else
        {
            // Если объект слишком далеко или ничего не найдено — оставляем стандартный поворот
            targetRotation = defaultRotation;
        }

        // Ограничиваем угол поворота
        targetRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxCorrectionAngle);

        // Плавно интерполируем поворот
        Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * correctionSpeed);

        // Передаём в WeaponTransformManager именно целевую ротацию
        transformManager.SetTargetRotation(finalRotation);
    }
}