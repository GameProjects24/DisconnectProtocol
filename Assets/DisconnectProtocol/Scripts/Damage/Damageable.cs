using UnityEngine;

namespace DisconnectProtocol
{	
	public abstract class Damageable : MonoBehaviour
	{
		public abstract void TakeDamage(float damage);
    }
}
