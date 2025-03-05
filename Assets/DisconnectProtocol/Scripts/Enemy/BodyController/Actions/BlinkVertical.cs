using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace DisconnectProtocol
{
	public interface IBlinkVertical {
		public bool TryStart();
		public void Stop();
	}

	public class BlinkVertical : IStoppable {
		private MonoBehaviour m_ctrl;
		private Coroutine m_cor;

		public float altMin, altMax;
		public float timeToBlink;
		public float height;

		private Transform m_tr;

		private const float EPS = .05f;

		public event System.Action Stopped;

		private BlinkVertical() {}
		public BlinkVertical(MonoBehaviour ctrl, float height, float altMin, float altMax, float timeToBlink) {
			m_ctrl = ctrl;
			m_tr = ctrl.transform;
			this.altMin = altMin;
			this.altMax = altMax;
			this.timeToBlink = timeToBlink;
			this.height = height;
		}

		public bool TryStart() {
			if (m_cor != null) {
				return true;
			}
			return TryStartInner();
		}

		public void Stop() {
			if (m_cor != null) {
				m_ctrl.StopCoroutine(m_cor);
				m_cor = null;
				Stopped?.Invoke();
			}
		}

		private bool TryStartInner() {
			if (altMin > altMax) {
				(altMin, altMax) = (altMax, altMin);
			}

			var pos = FindPos();
			if (pos.HasValue) {
				m_cor = m_ctrl.StartCoroutine(Cor(pos.Value));
				return true;
			}
			return false;
		}
		
		private Vector3? FindPos() {
			float minAltd = height * 1.5f;
			float altdf = altMax - m_tr.position.y;
			float altds = m_tr.position.y - altMin;
			float dir = 1;
			if (Random.Range(0, 2) == 0) {
				(altdf, altds) = (altds, altdf);
				dir = -dir;
			}

			if (altdf > minAltd) {
				var res = m_tr.position;
				res.y += Random.Range(height, altdf) * dir;
				return res;
			}
			if (altds > minAltd) {
				var res = m_tr.position;
				res.y += Random.Range(height, altds) * -dir;
				return res;
			}
			return null;
		}

		private IEnumerator Cor(Vector3 pos) {
			var vel = Vector3.zero;
			float remTime = timeToBlink;
			while (remTime > 0) {
				m_tr.position = Vector3.SmoothDamp(m_tr.position, pos, ref vel, remTime);
				remTime -= Time.fixedDeltaTime;
				yield return new WaitForFixedUpdate();
			}
			m_cor = null;
			Stopped?.Invoke();
		}
    }
}
