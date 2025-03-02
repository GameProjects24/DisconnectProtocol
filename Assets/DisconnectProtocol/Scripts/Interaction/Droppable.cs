using Unity.Behavior;
using UnityEngine;

namespace DisconnectProtocol {
	// should consider then to be combined with Pickable into Item
	[RequireComponent(typeof(Rigidbody))]
	public class Droppable : MonoBehaviour {
		[Range(0, 1)]
        public float dropChance = .5f;
		public Damageable body;

		private Rigidbody m_rb;
		private Transform m_tr;

		private void Start() {
			m_tr = transform;
			m_rb = GetComponent<Rigidbody>();
			m_rb.isKinematic = true;
			m_rb.useGravity = false;
			if (body == null) {
				body = GetComponentInParent<Damageable>();
			}
			body.OnDie += TryDrop;
			gameObject.SetActive(false);
		}

		private void TryDrop() {
			if (dropChance >= Random.Range(0f, 1f)) {
				Drop();
			}
		}

		public void Drop() {
			body.OnDie -= TryDrop;
			m_tr.SetParent(null, true);
			m_rb.isKinematic = false;
			m_rb.useGravity = true;
			gameObject.SetActive(true);
		}
	}
}