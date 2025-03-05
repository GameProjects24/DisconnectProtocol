using UnityEngine;

namespace DisconnectProtocol {	
	public class CreditActivator : MonoBehaviour {
		public Interactable toggle;
		public BasicDoor door;

		private void Start() {
			door.StateChanged += StartCredentials;
			toggle.Interacted += () => door.Close();
		}

		private void StartCredentials(IDoor.State state) {
			if (state == IDoor.State.Close) {
				GameStateController.Instance.ChangeState<CreditsState>();
			}
		}
	}
}