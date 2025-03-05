using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DisconnectProtocol
{
	public interface IAim {
		void Start(Transform target, bool perpetual);
		void Stop();
	}

    public class Aim {
		[Range(0, 1)]
		public float rotationRate;
		public float effectiveDist;

		private List<Transform> m_weaponParts;
		private MonoBehaviour m_controller;
		private Coroutine m_cor;
		private Transform m_tr;

		private const float EPS = .1f;
		private const float UNIT_TARGET_R = .125f;
		private float reverseRate;

		public event System.Action Stopped;

		private Aim() {}
		public Aim(MonoBehaviour controller, float rotRate, float efDist, IEnumerable<Transform> weaponParts) {
			m_weaponParts = new List<Transform>(weaponParts);
			m_controller = controller;
			m_tr = controller.transform;
			rotationRate = rotRate;
			effectiveDist = efDist;
			reverseRate = UNIT_TARGET_R / efDist;
		}

		public void Start(Transform target, bool perpetual) {
			if (m_cor != null) {
				return;
			}
			m_cor = m_controller.StartCoroutine(Cor(target, perpetual));
		}

		public void Stop() {
			if (m_cor != null) {
				m_controller.StopCoroutine(m_cor);
				m_cor = null;
				Stopped?.Invoke();
			}
		}

		private IEnumerator Cor(Transform target, bool perpetual) {
			float dif;
			do {
				if (target == null) break;
				var dir = target.position - m_tr.position;
				dir.y = 0;
				dif = m_tr.rotation.eulerAngles.y;
				m_tr.rotation = Quaternion.Slerp(m_tr.rotation, Quaternion.LookRotation(dir), rotationRate);
				dif -= m_tr.rotation.eulerAngles.y;

				yield return null;
			} while (perpetual || Mathf.Abs(dif) > EPS);
			m_cor = null;
			Stopped?.Invoke();
		}

		/// <summary>
		/// </summary>
		/// <param name="targetPos"></param>
		/// <returns>Final direction of aim</returns>
		private Vector3 Deviate(Vector3 targetPos) {
			var d = (targetPos - m_tr.position).magnitude;
			var r = d * reverseRate;
			var pos = Random.insideUnitSphere * r + targetPos;
			return pos - m_tr.position;
		}
    }
}
