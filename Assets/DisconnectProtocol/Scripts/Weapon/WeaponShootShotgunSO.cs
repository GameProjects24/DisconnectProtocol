using UnityEngine;
using DisconnectProtocol;

[CreateAssetMenu(menuName = "Weapon/WeaponShootShotgunSO", fileName = "WeaponShootShotgunSO")]
public class WeaponShootShotgunSO : WeaponShootSO
{
    public int pelletCount = 8;
    public float spreadAngle = 10f;  // Разброс в градусах
    public float maxDistance = 50f;
    public LayerMask hitMask; // Слои, по которым может попадать дробовик
    public LineRenderer tracerPrefab;
    public GameObject hitEffect;

    public override void Shoot(Vector3 position, Vector3 direction, float damage)
    {
        for (int i = 0; i < pelletCount; i++)
        {
            // Генерируем случайный угол разброса
            Vector3 spread = GetSpreadDirection(direction, spreadAngle);

            Vector3 endPoint;

            // Выполняем Raycast
            if (Physics.Raycast(position, spread, out RaycastHit hit, maxDistance, hitMask))
            {
                Debug.Log("Дробинка попала в " + hit.collider.name);
                endPoint = hit.point; // Если попали, фиксируем точку попадания

                DamageablePart target = hit.collider.GetComponent<DamageablePart>();
                if (target != null)
                {
                    target.TakeDamage(damage / pelletCount); // Каждая дробинка наносит часть урона
                }
                Instantiate(hitEffect, endPoint, Quaternion.identity);
            }
            else
            {
                // Если ничего не задели, луч уходит на максимальную дальность
                endPoint = position + spread * maxDistance;
            }

            // Создаём линию
            if (tracerPrefab != null)
            {
                LineRenderer tracer = Instantiate(tracerPrefab);
                tracer.SetPosition(0, position);
                tracer.SetPosition(1, endPoint);
            }
        }
    }

    private Vector3 GetSpreadDirection(Vector3 direction, float spread)
    {
        float angleX = Random.Range(-spread, spread);
        float angleY = Random.Range(-spread, spread);
        Quaternion rotation = Quaternion.Euler(angleX, angleY, 0);
        return rotation * direction;
    }
}
