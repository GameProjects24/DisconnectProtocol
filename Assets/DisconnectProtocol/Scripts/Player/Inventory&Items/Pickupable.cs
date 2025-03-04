using DisconnectProtocol;
using UnityEngine;

public interface IPickuper
{
	void PickupableFound(Pickupable item);
	void PickupableLost(Pickupable item);
}

[RequireComponent(typeof(Collider))]
public class Pickupable : MonoBehaviour
{
	private void Start()
	{
		var col = GetComponent<Collider>();
		col.isTrigger = true;
	}

	private void OnTriggerEnter(Collider other)
    {
		var pu = other.gameObject.GetComponentWherever<IPickuper>();
        if (pu != null)
        {
            pu.PickupableFound(this);
        }
    }

	private void OnTriggerExit(Collider other)
    {
		var pu = other.gameObject.GetComponentWherever<IPickuper>();
        if (pu != null)
        {
            pu.PickupableLost(this);
        }
    }
}