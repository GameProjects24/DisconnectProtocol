using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    [Header("References")]
    public RectTransform crosshairRectTransform;
    public GameObject weaponHolder;

    [Header("Base Settings")]
    public float baseSize = 50f;   // базовый размер перекрестья
    public float maxSize = 150f;   // максимальный размер перекрестья

    [Header("Smoothing")]
    public float smoothSpeed = 10f;  // скорость изменения размеров
    public float decaySpeed = 5f;    // скорость затухания значений

    [Header("Effect Multipliers")]
    public float bobbingMultiplier = 1f;
    public float swayMultiplier = 1f;
    public float recoilMultiplier = 1f;

    private float bobbingSpread;
    private float swaySpread;
    private float recoilSpread;

    private WeaponBobbing weaponBobbing;
    private WeaponSway weaponSway;
    private WeaponRecoil weaponRecoil;

    private float targetSize;
    private float currentSize;
    private float totalSpread;

    private void Awake()
    {
        if (weaponHolder != null)
        {
            weaponBobbing = weaponHolder.GetComponent<WeaponBobbing>();
            weaponSway = weaponHolder.GetComponent<WeaponSway>();
            weaponRecoil = weaponHolder.GetComponent<WeaponRecoil>();
        }
    }

    private void Start()
    {
        currentSize = baseSize;
        targetSize = baseSize;
    }

    private void OnEnable()
    {
        if (weaponBobbing != null)
            weaponBobbing.OnBobbingSpread += HandleBobbingSpread;
        if (weaponSway != null)
            weaponSway.OnSwaySpread += HandleSwaySpread;
        if (weaponRecoil != null)
            weaponRecoil.OnRecoilSpread += HandleRecoilSpread;
    }

    private void OnDisable()
    {
        if (weaponBobbing != null)
            weaponBobbing.OnBobbingSpread -= HandleBobbingSpread;
        if (weaponSway != null)
            weaponSway.OnSwaySpread -= HandleSwaySpread;
        if (weaponRecoil != null)
            weaponRecoil.OnRecoilSpread -= HandleRecoilSpread;
    }

    private void HandleBobbingSpread(float spread)
    {
        bobbingSpread = spread;
        UpdateCrosshair();
    }

    private void HandleSwaySpread(float spread)
    {
        swaySpread = spread;
        UpdateCrosshair();
    }

    private void HandleRecoilSpread(float spread)
    {
        recoilSpread = spread;
        UpdateCrosshair();
    }

    private void UpdateCrosshair()
    {
        // Cуммируем максимальные значения с учетом множителей
        totalSpread = (bobbingSpread * bobbingMultiplier) +
            (swaySpread * swayMultiplier) +
            (recoilSpread * recoilMultiplier);

        targetSize = Mathf.Clamp(baseSize + totalSpread, baseSize, maxSize);
    }

    private void Update()
    {
        bobbingSpread = Mathf.Lerp(bobbingSpread, 0f, Time.deltaTime * decaySpeed);
        swaySpread = Mathf.Lerp(swaySpread, 0f, Time.deltaTime * decaySpeed);
        recoilSpread = Mathf.Lerp(recoilSpread, 0f, Time.deltaTime * decaySpeed);

        // Пересчёт размера после затухания
        UpdateCrosshair();

        currentSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * smoothSpeed);
        crosshairRectTransform.sizeDelta = new Vector2(currentSize, currentSize);
    }
}
