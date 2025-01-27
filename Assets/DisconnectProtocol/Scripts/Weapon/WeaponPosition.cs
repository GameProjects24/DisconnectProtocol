using System;
using UnityEngine;

public class WeaponPosition : MonoBehaviour
{
    public PlayerController playerController;
    public PlayerInputs inputs; 

    [Header("Sway Settings")]
    public float step = 0.01f;
    public float maxStepDistance = 0.06f;
    private Vector3 swayPos;

    [Header("Sway Rotation Settings")]
    public float rotationStep = 4f;
    public float maxRotationStep = 5f;
    private Vector3 swayEulerRot;

    public float smooth = 10f;
    private float smoothRot = 12f;

    [Header("Bobbing Settings")]
    public float speedCurve;
    private float curveSin => Mathf.Sin(speedCurve);
    private float curveCos => Mathf.Cos(speedCurve);

    public Vector3 travelLimit = Vector3.one * 0.025f;
    public Vector3 bobLimit = Vector3.one * 0.01f;
    private Vector3 bobPosition;

    public float bobExaggeration;

    [Header("Bob Rotation Settings")]
    public Vector3 multiplier;
    private Vector3 bobEulerRotation;

    [Header("Aim Settings")]
    public Vector3 aimPosition; // Позиция оружия при прицеливании
    public float aimSpeed = 8f; // Скорость перехода между позициями
    private Vector3 defaultPosition; // Позиция оружия в обычном состоянии
    private bool isAiming; // Флаг для проверки состояния прицеливания

    private void Start()
    {
        // Запоминаем стартовую позицию как позицию по умолчанию
        defaultPosition = transform.localPosition;

        // Подписываемся на событие прицеливания
        playerController.OnAim += ToggleAim;
    }

    private void OnDestroy()
    {
        // Отписываемся от события
        playerController.OnAim -= ToggleAim;
    }

    private void Update()
    {
        GetInput();

        Sway();
        SwayRotation();
        BobOffset();
        BobRotation();
        CompositePositionRotation();
    }

    private Vector2 walkInput;
    private Vector2 lookInput;

    private void GetInput()
    {
        walkInput = inputs.move.normalized;
        lookInput = inputs.look.normalized;
    }

    private void Sway()
    {
        Vector3 invertLook = lookInput * -step;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxStepDistance, maxStepDistance);

        swayPos = invertLook;
    }

    private void SwayRotation()
    {
        Vector2 invertLook = lookInput * -rotationStep;
        // Где " * 1f" поменять на умножение на "коэффициент прицеливания" - уменьшение тряски во время прицеливания и его увеличение без прицеливания
        invertLook.x = Mathf.Clamp(invertLook.x, -maxRotationStep, maxRotationStep) * 1f;
        invertLook.y = Mathf.Clamp(invertLook.y, -maxRotationStep, maxRotationStep) * 1f;
        swayEulerRot = new Vector3(invertLook.y, invertLook.x, invertLook.y);
    }

    private void CompositePositionRotation()
    {
        Vector3 targetPosition = isAiming ? aimPosition : defaultPosition;
        // Интерполяция позиции и поворота
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition + swayPos + bobPosition, Time.deltaTime * smooth);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(swayEulerRot) * Quaternion.Euler(bobEulerRotation), Time.deltaTime * smoothRot);
    }

    private void BobOffset()
    {
        speedCurve += Time.deltaTime * (playerController.isGrounded ? (walkInput.magnitude * bobExaggeration) : 1f) + 0.01f;

        bobPosition.x = (curveCos * bobLimit.x * (playerController.isGrounded ? 1 : 0)) - (walkInput.x * travelLimit.x);
        bobPosition.y = (curveSin * bobLimit.y) - (walkInput.y * travelLimit.y);
        bobPosition.z = -(walkInput.y * travelLimit.z);
    }

    private void BobRotation()
    {
        // Где " * 1f" поменять на умножение на "коэффициент прицеливания" - уменьшение тряски во время прицеливания и его увеличение без прицеливания
        bobEulerRotation.x = (walkInput != Vector2.zero ? multiplier.x * (Mathf.Sin(2 * speedCurve)) : multiplier.x * (Mathf.Sin(2 * speedCurve) / 2)) * 1f;
        bobEulerRotation.y = (walkInput != Vector2.zero ? multiplier.y * curveCos : 0) * 1f;
        bobEulerRotation.z = (walkInput != Vector2.zero ? multiplier.z * curveCos * walkInput.x : 0) * 1f;
    }

    private void ToggleAim(bool aimState)
    {
        // Изменяем состояние прицеливания
        isAiming = aimState;
    }
}
