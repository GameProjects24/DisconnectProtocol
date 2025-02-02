using UnityEngine;

namespace DisconnectProtocol
{
	[RequireComponent(typeof(Collider))]
    public class DamageablePart : MonoBehaviour
    {
		public Damageable body;
        public float damageRate = 1f;

		private void OnCollisionEnter(Collision other) {
			if (other.gameObject.TryGetComponent<IDamager>(out var d)) {
				body.TakeDamage(d.Damage());
			}
		}

		public void TakeDamage(float damage)
		{
			body.TakeDamage(damage);
		}
    }
}
