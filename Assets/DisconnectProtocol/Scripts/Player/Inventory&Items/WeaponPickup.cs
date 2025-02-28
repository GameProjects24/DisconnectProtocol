using UnityEngine;

public class WeaponPickup : MonoBehaviour, IPickupable
{
    public Weapon weapon;

    public void Pickup(Inventory inventory)
    {
        inventory.AddWeapon(weapon);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Inventory inventory = other.GetComponent<Inventory>();
            if (inventory != null)
            {
                Pickup(inventory);
            }
        }
    }
}
