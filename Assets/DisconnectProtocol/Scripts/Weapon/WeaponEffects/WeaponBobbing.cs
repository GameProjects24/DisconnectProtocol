using UnityEngine;

public class WeaponBobbing : MonoBehaviour
{
    private WeaponTransformManager positionManager;
    public PlayerController playerController;
    public PlayerInputs inputs;

    [Header("Bobbing Settings")]
    public float bobSpeed = 5f;
    public float bobSprintSpeed = 7f;
    public Vector3 travelLimit = Vector3.one * 0.025f;
    public Vector3 bobLimit = Vector3.one * 0.01f;
    public float aimBobMultiplier = 0.3f;

    [Header("Bob Rotation Settings")]
    public Vector3 multiplier;

    private Vector3 bobPosition;
    private Vector3 bobEulerRotation;
    private bool isSprinting;
    private WeaponAiming weaponAim;
    private float targetBobSpeed;
    private float time;
    private float bobMultiplier;

    private void Start()
    {
        weaponAim = GetComponent<WeaponAiming>();
        positionManager = GetComponent<WeaponTransformManager>();
        // сделать через свойство, публичное для чтения
        playerController.OnSprint += ToggleSprint;
    }

    private void OnDestroy()
    {
        playerController.OnSprint -= ToggleSprint;
    }

    private void ToggleSprint(bool sprintState)
    {
        isSprinting = sprintState;
    }

    private void Update()
    {
        ApplyBobbing(inputs.move.normalized);
    }

    private void ApplyBobbing(Vector2 walkInput)
    {
        if (walkInput == Vector2.zero || !playerController.isGrounded)
        {
            bobPosition = Vector3.Lerp(bobPosition, Vector3.zero, Time.deltaTime * 5f);
            bobEulerRotation = Vector3.Lerp(bobEulerRotation, Vector3.zero, Time.deltaTime * 5f);
        }
        else
        {
            targetBobSpeed = isSprinting ? bobSprintSpeed : bobSpeed;
            time = Time.time * targetBobSpeed;
            bobMultiplier = weaponAim != null && weaponAim.IsAiming ? aimBobMultiplier : 1f;

            // Перемещение оружия по дуге
            bobPosition.x = (Mathf.Cos(time) * bobLimit.x - walkInput.x * travelLimit.x) * bobMultiplier;
            bobPosition.y = (Mathf.Sin(time) * bobLimit.y - walkInput.y * travelLimit.y) * bobMultiplier;
            bobPosition.z = -walkInput.y * travelLimit.z * bobMultiplier;

            // Вращение оружия
            bobEulerRotation.x = (multiplier.x * Mathf.Sin(2 * time)) * bobMultiplier;
            bobEulerRotation.y = (multiplier.y * Mathf.Cos(time)) * bobMultiplier;
            bobEulerRotation.z = (multiplier.z * Mathf.Cos(time) * walkInput.x) * bobMultiplier;
        }

        positionManager.AddPositionOffset(bobPosition);
        positionManager.AddRotationOffset(bobEulerRotation);
    }
}
