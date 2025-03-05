using System;
using UnityEngine;

public class WeaponPosition : MonoBehaviour
{
    public PlayerController playerController;
    private PlayerControls controls;

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
    public float bobSpeed = 5f; // Скорость покачивания
    public float bobSprintSpeed = 7f;
    public Vector3 travelLimit = Vector3.one * 0.025f;
    public Vector3 bobLimit = Vector3.one * 0.01f;
    private Vector3 bobPosition;

    public float bobExaggeration = 1f;

    [Header("Bob Rotation Settings")]
    public Vector3 multiplier;
    private Vector3 bobEulerRotation;

    [Header("Aim Settings")]
    public Vector3 aimPosition;
    public float aimSpeed = 8f;
    private Vector3 defaultPosition;
    private bool isAiming;
    private bool isSpinting;

    [Header("Aim Multiplier Settings")]
    public float aimSwayMultiplier = 0.2f;
    public float aimBobMultiplier = 0.3f;

    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        controls = PlayerControls.Instance;
        defaultPosition = transform.localPosition;

        playerController.OnAim += ToggleAim;
        playerController.OnSprint += ToggleSprint;
    }

    private void OnDestroy()
    {
        playerController.OnAim -= ToggleAim;
        playerController.OnSprint -= ToggleSprint;
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
        walkInput = controls.move.normalized;
        lookInput = controls.look.normalized;
    }

    private void Sway()
    {
        Vector3 invertLook = lookInput * -step;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxStepDistance, maxStepDistance);

        swayPos = Vector3.Lerp(swayPos, invertLook * (isAiming ? aimSwayMultiplier : 1f), Time.deltaTime * smooth);
    }

    private void SwayRotation()
    {
        Vector2 invertLook = lookInput * -rotationStep;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxRotationStep, maxRotationStep);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxRotationStep, maxRotationStep);
        swayEulerRot = Vector3.Lerp(swayEulerRot, new Vector3(invertLook.y, invertLook.x, invertLook.y) * (isAiming ? aimSwayMultiplier : 1f), Time.deltaTime * smoothRot);
    }

    private void CompositePositionRotation()
    {
        Vector3 targetPosition = isAiming ? aimPosition : defaultPosition;

        // Более плавный переход позиции
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPosition + swayPos + bobPosition, ref velocity, 0.1f);

        // Плавное вращение
        Quaternion targetRotation = Quaternion.Euler(swayEulerRot) * Quaternion.Euler(bobEulerRotation) * Quaternion.Euler(new Vector3 (0, 0, 0));
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * smoothRot);
    }

    private void BobOffset()
    {
        float targetBobSpeed = isSpinting ? bobSprintSpeed : bobSpeed;
        float time = Time.time * targetBobSpeed;
        bobPosition.x = (walkInput != Vector2.zero ? (Mathf.Cos(time) * bobLimit.x) - (walkInput.x * travelLimit.x) : 0) * (isAiming ? aimBobMultiplier : 1f);
        bobPosition.y = (walkInput != Vector2.zero ? (Mathf.Sin(time) * bobLimit.y) - (walkInput.y * travelLimit.y) : 0) * (isAiming ? aimBobMultiplier : 1f);
        bobPosition.z = -(walkInput.y * travelLimit.z) * (isAiming ? aimBobMultiplier : 1f);
    }

    private void BobRotation()
    {
        float targetBobSpeed = isSpinting ? bobSprintSpeed : bobSpeed;
        float time = Time.time * targetBobSpeed;
        bobEulerRotation.x = (walkInput != Vector2.zero ? multiplier.x * Mathf.Sin(2 * time) : 0) * (isAiming ? aimBobMultiplier : 1f) * (playerController.isGrounded ? 1 : 0);
        bobEulerRotation.y = (walkInput != Vector2.zero ? multiplier.y * Mathf.Cos(time) : 0) * (isAiming ? aimBobMultiplier : 1f) * (playerController.isGrounded ? 1 : 0);
        bobEulerRotation.z = (walkInput != Vector2.zero ? multiplier.z * Mathf.Cos(time) * walkInput.x : 0) * (isAiming ? aimBobMultiplier : 1f) * (playerController.isGrounded ? 1 : 0);
    }

    private void ToggleAim(bool aimState)
    {
        isAiming = aimState;
    }

    private void ToggleSprint(bool sprintState)
    {
        isSpinting = sprintState;
    }
}
