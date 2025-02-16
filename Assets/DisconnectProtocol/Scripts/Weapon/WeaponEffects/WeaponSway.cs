using System;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public event Action<float> OnSwaySpread;
    private WeaponTransformManager positionManager;
    public PlayerInputs inputs;

    [Header("Sway Settings")]
    public float step = 0.01f;
    public float maxStepDistance = 0.06f;
    public float smooth = 10f;

    [Header("Sway Rotation Settings")]
    public float rotationStep = 4f;
    public float maxRotationStep = 5f;
    private float smoothRot = 12f;

    [Header("Aim Multiplier")]
    public float aimSwayMultiplier = 0.2f;

    private Vector3 swayPos;
    private Vector3 swayEulerRot;
    private float swayMultiplier;
    private WeaponAiming weaponAim;
    private float maxSwaySpread;

    private void Start()
    {
        weaponAim = GetComponent<WeaponAiming>();
        positionManager = GetComponent<WeaponTransformManager>();
    }

    private void Update()
    {
        ApplySway(inputs.look.normalized);
    }

    private void ApplySway(Vector2 lookInput)
    {
        swayMultiplier = weaponAim.IsAiming ? aimSwayMultiplier : 1f;

        // Горизонтальное смещение (позиция)
        Vector3 invertLook = lookInput * -step;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxStepDistance, maxStepDistance);

        swayPos = Vector3.Lerp(swayPos, invertLook * swayMultiplier, Time.deltaTime * smooth);

        // Вращение оружия
        Vector2 invertLookRot = lookInput * -rotationStep;
        invertLookRot.x = Mathf.Clamp(invertLookRot.x, -maxRotationStep, maxRotationStep);
        invertLookRot.y = Mathf.Clamp(invertLookRot.y, -maxRotationStep, maxRotationStep);

        swayEulerRot = Vector3.Lerp(swayEulerRot, new Vector3(invertLookRot.y, invertLookRot.x, invertLookRot.y) * swayMultiplier, Time.deltaTime * smoothRot);

        // Передаём смещения в WeaponPositionManager
        positionManager.AddPositionOffset(swayPos);
        positionManager.AddRotationOffset(swayEulerRot);

        maxSwaySpread = new Vector3(maxStepDistance, maxStepDistance, maxStepDistance).magnitude * swayMultiplier;
        OnSwaySpread?.Invoke(maxSwaySpread);
    }
}
