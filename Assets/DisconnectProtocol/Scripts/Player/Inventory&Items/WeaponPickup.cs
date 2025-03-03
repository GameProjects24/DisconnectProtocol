using UnityEngine;

public class WeaponPickup : MonoBehaviour, IPickupable
{
    public WeaponData weaponData;

    public void Pickup(Inventory inventory)
    {
        // if (!inventory.weaponDataList.Contains(weaponData))
        // {
        //     inventory.AddWeapon(weaponData);
        //     Destroy(gameObject);
        // }
    }

    private void OnTriggerEnter(Collider other)
    {
        // if (other.CompareTag("Player"))
        // {
        //     Inventory inventory = other.GetComponent<Inventory>();
        //     if (inventory != null)
        //     {
        //         Pickup(inventory);
        //     }
        // }
    }
}
