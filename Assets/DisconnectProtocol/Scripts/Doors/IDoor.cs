using System.Collections;
using UnityEngine;

namespace DisconnectProtocol
{
	public interface IDoor {
		public enum State {
			Open = -1, Close = 1
		}

		public event System.Action<State> StateChanged;
		public event System.Action<State> StateChanging;

		public void Open(bool isInterrupt = false);
		public void Close(bool isInterrupt = false);
		public void Toggle(bool isInterrupt = false);
	}

	public abstract class BasicDoor : MonoBehaviour, IDoor {
		public event System.Action<IDoor.State> StateChanged;
		public event System.Action<IDoor.State> StateChanging;

		public IDoor.State currentState = IDoor.State.Open;
		protected bool m_isRunning = false;
		
		public void Open(bool isInterrupt = false) {
			if (currentState == IDoor.State.Open) {
				return;
			}
			Toggle(isInterrupt);
		}

		public void Close(bool isInterrupt = false) {
			if (currentState == IDoor.State.Close) {
				return;
			}
			Toggle(isInterrupt);
		}

		public void Toggle(bool isInterrupt = false) {
			if (m_isRunning && !isInterrupt) {
				return;
			}
			m_isRunning = true;
			currentState = (IDoor.State)(-(int)currentState);
			StateChanging?.Invoke(currentState);
			StartCoroutine(ChangeStateCor());
		}

		protected virtual void OnStateChanged(IDoor.State state) {
			StateChanged?.Invoke(state);
		}

		protected abstract IEnumerator ChangeStateCor();
	}
}
