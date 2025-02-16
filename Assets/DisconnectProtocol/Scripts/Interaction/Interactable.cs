using UnityEngine;

namespace DisconnectProtocol
{
	[RequireComponent(typeof(Collider))]
    public class Interactable : MonoBehaviour
    {
		public event System.Action Interacted;
        private Collider m_interactionField;

		private void Start() {
			m_interactionField = GetComponent<Collider>();
			m_interactionField.isTrigger = true;
		}

		public void Interact() {
			Interacted?.Invoke();
		}

		private void OnTriggerEnter(Collider other) {
			var inter = other.GetComponentInChildren<Interactor>();
			if (inter) {
				inter.InteractableFound(this);
			}
		}

		private void OnTriggerExit(Collider other) {
			var inter = other.GetComponentInChildren<Interactor>();
			if (inter) {
				inter.InteractableLost(this);
			}
		}
	}
}
