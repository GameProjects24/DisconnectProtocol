using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;

namespace DisconnectProtocol
{
	public interface IHoldDistance {
		public void Start(Transform target, bool perpetual);
		public void Stop();
	}

    public class HoldDistance : IStoppable
    {
        public float holdDistance;
		private MonoBehaviour m_controller;
		private NavMeshAgent m_agent;

		private float m_hd2;
		private Coroutine m_cor;

		public event System.Action Stopped;
		public event System.Action Paused;
		public event System.Action Resumed;

		private bool m_isJustStarted;
		private bool m_isNotPaused = true;

		private HoldDistance() {}
		public HoldDistance(MonoBehaviour ctrl, NavMeshAgent agent, float distance) {
			m_controller = ctrl;
			m_agent = agent;
			holdDistance = distance;
			m_hd2 = distance * distance;
		}

		public void Start(Transform target, bool perpetual) {
			if (m_cor != null) {
				return;
			}
			m_isJustStarted = true;
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
			var self = m_agent.transform;

			do {
				if (target == null) break;
				var dist = Vector3.SqrMagnitude(target.transform.position - self.position);
				if (dist > m_hd2) {
					if (m_isJustStarted) {
						m_isJustStarted = false;
					} else {
						Resumed?.Invoke();
					}
					m_isNotPaused = true;
					m_agent.isStopped = false;

					do {
						if (target == null) break;
						m_agent.SetDestination(target.transform.position);
						dist = Vector3.SqrMagnitude(target.transform.position - self.position);
						yield return new WaitForFixedUpdate();
					} while (dist > m_hd2);
					
					m_agent.isStopped = true;
					m_agent.ResetPath();
				} else {
					if (m_isNotPaused) {
						m_isNotPaused = false;
						Paused?.Invoke();
					}
					yield return new WaitForFixedUpdate();
				}
			} while (perpetual);

			m_agent.isStopped = true;
			m_agent.ResetPath();
			m_cor = null;
			Stopped?.Invoke();
		}
    }
}