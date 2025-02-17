using UnityEngine;

public class WeaponAiming : MonoBehaviour
{
    private WeaponTransformManager positionManager;
    public PlayerController playerController;

    public Vector3 aimPosition;
    private Vector3 defaultPosition;
    public float aimSpeed = 8f;
    public bool IsAiming { get; private set; }

    private void Start()
    {
        positionManager = GetComponent<WeaponTransformManager>();
        defaultPosition = positionManager.DefaultPosition;
        playerController.OnAim += ToggleAim;
    }

    private void OnDestroy()
    {
        playerController.OnAim -= ToggleAim;
    }

    private void ToggleAim(bool aimState)
    {
        IsAiming = aimState;
    }

    private void Update()
    {
        Vector3 targetPos = IsAiming ? aimPosition : defaultPosition;
        positionManager.SetTargetPosition(targetPos);
    }
}
