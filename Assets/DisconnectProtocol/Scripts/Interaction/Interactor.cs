using UnityEngine;

namespace DisconnectProtocol
{
    public class Interactor : MonoBehaviour
    {
		private Interactable m_inter;

		public void Interact() {
			Debug.Log("Inter");
			if (m_inter) {
				m_inter.Interact();
			}
		}

		public void InteractableFound(Interactable obj) {
			m_inter = obj;
		}

		public void InteractableLost(Interactable obj) {
			if (m_inter == obj) {
				m_inter = null;
			}
		}
	}
}
