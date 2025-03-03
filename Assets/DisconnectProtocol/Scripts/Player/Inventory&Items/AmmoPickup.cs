using UnityEngine;

public class AmmoPickup : MonoBehaviour, IPickupable
{
    public WeaponData weaponData;

    public int amount;

    public void Pickup(Inventory inventory)
    {
        // if (inventory.reserveAmmoInventory.ContainsKey(weaponData) && !inventory.ReserveAmmoFull(weaponData))
        // {
        //     inventory.AddAmmo(weaponData, amount);
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

