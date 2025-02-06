using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class HUDManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI cageAmmo;
    public TextMeshProUGUI reserveAmmo;
    //public TextMeshProUGUI weaponNameText;
    public Image weaponIcon;
    public Image reloadIndicator;

    public WeaponController weaponController;

    private Weapon currWeapon;

    private void OnEnable()
    {
        if (weaponController != null)
        {
            weaponController.OnChangeWeapon += UpdateWeapon;
            weaponController.OnReloadComplete += UpdateAmmoUI;
            weaponController.OnShoot += UpdateAmmoUI;
            weaponController.OnReload += StartReloadIndicator;
        }
    }

    private void OnDestroy()
    {
        if (weaponController != null)
        {
            weaponController.OnChangeWeapon -= UpdateWeapon;
            weaponController.OnReloadComplete -= UpdateAmmoUI;
            weaponController.OnShoot -= UpdateAmmoUI;
            weaponController.OnReload -= StartReloadIndicator;
        }
    }

    private void UpdateWeapon(Weapon newWeapon = null)
    {
        if (newWeapon != null)
        {
            //weaponNameText.text = newWeapon.weaponData.weaponName;
            weaponIcon.sprite = newWeapon.weaponData.weaponIcon;
            reloadIndicator.gameObject.SetActive(false);
            UpdateAmmoUI();
            currWeapon = newWeapon;
        }
    }

    public void UpdateAmmoUI()
    {
        if (weaponController == null || cageAmmo == null || reserveAmmo == null) return;

        cageAmmo.text = $"{weaponController.GetCurrentAmmo()}";
        reserveAmmo.text = $"{weaponController.GetTotalAmmo()}";

        // if (!weaponController.IsCurWeaponReloading())
        // {
        //     HideReloadIndicator();
        // }
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

    // private void ShowReloadIndicator()
    // {
    //     if (reloadIndicator != null)
    //         reloadIndicator.gameObject.SetActive(true);
    // }

    // private void HideReloadIndicator()
    // {
    //     if (reloadIndicator != null)
    //         reloadIndicator.gameObject.SetActive(false);
    // }
}
