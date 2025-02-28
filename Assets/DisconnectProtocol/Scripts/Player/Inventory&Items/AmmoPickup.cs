using UnityEngine;

public class AmmoPickup : MonoBehaviour, IPickupable
{
    public Weapon weapon;

    public int amount;

    public void Pickup(Inventory inventory)
    {
        inventory.AddAmmo(weapon, amount);
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

