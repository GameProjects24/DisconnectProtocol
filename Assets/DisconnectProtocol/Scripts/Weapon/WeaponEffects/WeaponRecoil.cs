using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    private WeaponTransformManager transformManager;
    public WeaponController weaponController;
    private WeaponAiming weaponAiming;

    [Header("Recoil Settings")]
    public Vector3 recoilKickback = new Vector3(0f, 0f, -0.1f);
    public Vector3 recoilRotation = new Vector3(-2f, 1f, 0.5f);
    public float recoilRecoverySpeed = 8f;

    [Header("Aim Multipliers")]
    public float aimRecoilMultiplierPosition = 0.5f;
    public float aimRecoilMultiplierRotation = 0.5f;

    private Vector3 currentRecoilPos;
    private Quaternion currentRecoilRot = Quaternion.identity;

    private void Start()
    {
        weaponAiming = GetComponent<WeaponAiming>();
        transformManager = GetComponent<WeaponTransformManager>();
        weaponController.OnShoot += ApplyRecoil;
    }

    private void OnDestroy()
    {
        weaponController.OnShoot -= ApplyRecoil;
    }

    private void Update()
    {
        RecoverRecoil();
    }

    public void ApplyRecoil()
    {
        float posMultiplier = weaponAiming.IsAiming ? aimRecoilMultiplierPosition : 1f;
        float rotMultiplier = weaponAiming.IsAiming ? aimRecoilMultiplierRotation : 1f;

        currentRecoilPos += recoilKickback * posMultiplier;

        Quaternion recoilRot = Quaternion.Euler(
            Random.Range(recoilRotation.x * 0.8f, recoilRotation.x * 1.2f),
            Random.Range(-recoilRotation.y, recoilRotation.y),
            Random.Range(-recoilRotation.z, recoilRotation.z)
        );

        currentRecoilRot *= Quaternion.Slerp(Quaternion.identity, recoilRot, rotMultiplier);

        transformManager.AddPositionOffset(currentRecoilPos);
        transformManager.AddRotationOffset(currentRecoilRot);
    }

    private void RecoverRecoil()
    {
        // Плавное восстановление позиций с помощью Lerp
        currentRecoilPos = Vector3.Lerp(currentRecoilPos, Vector3.zero, Time.deltaTime * recoilRecoverySpeed);
        currentRecoilRot = Quaternion.Slerp(currentRecoilRot, Quaternion.identity, Time.deltaTime * recoilRecoverySpeed);

        transformManager.AddPositionOffset(currentRecoilPos);
        transformManager.AddRotationOffset(currentRecoilRot);
    }
}
