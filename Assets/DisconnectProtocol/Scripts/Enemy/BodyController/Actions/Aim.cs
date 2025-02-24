using System.Collections;
using UnityEngine;

namespace DisconnectProtocol
{
    public class Aim : IStoppable
    {
		[Range(0, 1)]
		public float rotationRate;

		private MonoBehaviour m_controller;
		private Coroutine m_cor;

		private const float EPS = .1f;

		public event System.Action Stopped;

		private Aim() {}
		public Aim(MonoBehaviour controller, float rotRate) {
			m_controller = controller;
			rotationRate = rotRate;
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
			var self = m_controller.transform;
			float dif;
			do {
				var dir = target.position - self.position;
				dir.y = 0;
				dif = self.rotation.eulerAngles.y;
				self.rotation = Quaternion.Slerp(self.rotation, Quaternion.LookRotation(dir), rotationRate);
				dif -= self.rotation.eulerAngles.y;

				yield return null;
			} while (perpetual || Mathf.Abs(dif) > EPS);
			m_cor = null;
			Stopped?.Invoke();
		}
    }
}
