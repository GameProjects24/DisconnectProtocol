using UnityEngine; 
using TMPro;
using UnityEngine.UI;
using System.Collections;
using DisconnectProtocol;

public class HUDManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI cageAmmo;
    public TextMeshProUGUI reserveAmmo;
    //public TextMeshProUGUI weaponNameText;
    public Image weaponIcon;
    public Image reloadIndicator;
    public Image hpIndicator;

    public WeaponController weaponController;
    public Damageable damageable;

    private Weapon currWeapon;

    private void Start()
    {
        if (weaponController == null || damageable == null) return;

        // Подписываемся на события
        weaponController.OnChangeWeapon += UpdateWeapon;
        weaponController.OnReloadComplete += UpdateAmmoUI;
        weaponController.OnShoot += UpdateAmmoUI;
        weaponController.OnReload += StartReloadIndicator;
        damageable.OnDamage += UpdateHPIndicator;
        damageable.OnHeal += UpdateHPIndicator;

        UpdateHPIndicator();
        UpdateWeapon(weaponController.GetCurrentWeapon());  // Обновляем UI при старте
    }

    private void OnDestroy()
    {
        if (weaponController == null || damageable == null) return;

        // Отписываемся от событий
        weaponController.OnChangeWeapon -= UpdateWeapon;
        weaponController.OnReloadComplete -= UpdateAmmoUI;
        weaponController.OnShoot -= UpdateAmmoUI;
        weaponController.OnReload -= StartReloadIndicator;
        damageable.OnDamage -= UpdateHPIndicator;
    }

    private void UpdateWeapon(Weapon newWeapon)
    {
		if (currWeapon == newWeapon)
		{
			return;
		}
		if (currWeapon != null)
		{
			currWeapon.OnAmmoChanged -= UpdateAmmoUI;
		}

        if (newWeapon != null)
        {
            //weaponNameText.text = newWeapon.weaponData.weaponName;
            weaponIcon.sprite = newWeapon.weaponData.weaponIcon;
            reloadIndicator.gameObject.SetActive(false);
            currWeapon = newWeapon;
			currWeapon.OnAmmoChanged += UpdateAmmoUI;
            UpdateAmmoUI();  // Обновить патроны UI при смене оружия
        }
    }

    public void UpdateAmmoUI()
    {
        if (weaponController == null || cageAmmo == null || reserveAmmo == null) return;

        // Обновляем количество патронов в UI с учётом инвентаря
        if (currWeapon != null)
        {
            // Получаем количество патронов в магазине
            int currentAmmo = currWeapon.cageAmmo;
            int totalAmmo = currWeapon.reserveAmmo;

            cageAmmo.text = $"{currentAmmo}";  // Патроны в магазине
            reserveAmmo.text = $"{totalAmmo}";  // Общий запас патронов из инвентаря
        }
    }

    private void StartReloadIndicator()
    {
        if (reloadIndicator == null || currWeapon == null) return;

        float reloadTime = currWeapon.weaponData.reloadTime;
        if (reloadTime > 0)
        {
            reloadIndicator.gameObject.SetActive(true);
            StartCoroutine(AnimateReloadIndicator(reloadTime));
        }
    }

    private void UpdateHPIndicator()
    {
        if (hpIndicator == null) return;
        hpIndicator.fillAmount = damageable.hp / damageable.maxHp;
    }

    private IEnumerator AnimateReloadIndicator(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            reloadIndicator.fillAmount = 1f - (elapsed / duration);
            yield return null;
        }
        reloadIndicator.fillAmount = 0f;
        reloadIndicator.gameObject.SetActive(false);
    }
}
