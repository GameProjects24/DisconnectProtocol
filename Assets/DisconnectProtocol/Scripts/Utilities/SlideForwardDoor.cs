using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DisconnectProtocol
{
    public class SlideForwardDoor : MonoBehaviour
    {
		public enum State {
			Open = -1, Close = 1,
		}

		public event System.Action<State> StateChanged;

		[SerializeField] private State m_last = State.Open;
		[Range(0, 1)]
		[SerializeField] private float m_rate = .02f;
		private bool m_isRunning = false;

		private Transform m_tr;
		private Vector3 m_size;

		private const float EPS = .1f;

		private void Awake() {
			m_tr = transform;
			var rend = GetComponentInChildren<Renderer>();
			if (rend != null) {
				m_size = rend.bounds.size;
			}
		}

		public void Open(bool isInterrupt = false) {
			if (m_last == State.Open) {
				return;
			}
			Toggle(isInterrupt);
		}

		public void Close(bool isInterrupt = false) {
			if (m_last == State.Close) {
				return;
			}
			Toggle(isInterrupt);
		}

		public void Toggle(bool isInterrupt = false) {
			if (m_isRunning && !isInterrupt) {
				return;
			}
			m_isRunning = true;
			m_last = (State)(-(int)m_last);
			StartCoroutine(ChangeStateCor());
		}

		private IEnumerator ChangeStateCor() {
			var dest = m_tr.position + ElMul(m_tr.forward * (float)m_last, m_size);

			do {
				yield return null;
				var vel = Vector3.zero;
				m_tr.position = Vector3.Lerp(m_tr.position, dest, m_rate);
			} while (ElEpsGr(dest - m_tr.position, EPS));

			m_isRunning = false;
			StateChanged?.Invoke(m_last);
			yield break;
		}

		private Vector3 ElMul(Vector3 a, Vector3 b) {
			return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		private bool ElEpsGr(Vector3 v, float eps) {
			return Mathf.Abs(v.x) > eps || Mathf.Abs(v.y) > eps || Mathf.Abs(v.z) > eps;
		}
    }
}