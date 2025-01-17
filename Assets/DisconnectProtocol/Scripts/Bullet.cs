using DisconnectProtocol;
using UnityEngine;

namespace DisconnectProtocol {
	public class Bullet : MonoBehaviour, IDamager
	{
		[SerializeField] private GameObject _sparksPrefab;
		public float weaponDamage { private get; set; }
		private void OnCollisionEnter(Collision other) {
			//Vector3 carVelocity = GetComponent<Rigidbody>().;
			//Vector3 sparkDirection = carVelocity.normalized;
			Instantiate(_sparksPrefab, transform.position,  Quaternion.LookRotation(transform.forward));
			Destroy(gameObject);
		}

		public float Damage() {
			return weaponDamage;
		}
	}
}