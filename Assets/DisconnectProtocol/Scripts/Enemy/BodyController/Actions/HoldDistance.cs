using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;

namespace DisconnectProtocol
{
    public class HoldDistance
    {
        public float holdDistance;
		private MonoBehaviour m_controller;
		private NavMeshAgent m_agent;

		private float m_hd2;
		private bool m_isActive = false;
		private Coroutine m_cor;

		private HoldDistance() {}
		public HoldDistance(MonoBehaviour ctrl, NavMeshAgent agent, float hold) {
			m_controller = ctrl;
			m_agent = agent;
			holdDistance = hold;
			m_hd2 = hold * hold;
		}

		public void Start(Transform target, bool perpetual) {
			if (m_isActive) {
				return;
			}
			m_cor = m_controller.StartCoroutine(Cor(target, perpetual));
		}

		public void Stop() {
			m_isActive = false;
			if (m_cor != null) {
				m_controller.StopCoroutine(m_cor);
			} 
		}

		private IEnumerator Cor(Transform target, bool perpetual) {
			m_isActive = true;
			var self = m_controller.transform;

			do {
				var dist = Vector3.SqrMagnitude(target.transform.position - self.position);
				if (dist > m_hd2) {
					m_agent.isStopped = false;

					do {
						m_agent.SetDestination(target.transform.position);
						dist = Vector3.SqrMagnitude(target.transform.position - self.position);
						yield return null;
					} while (dist > m_hd2);
					
					m_agent.isStopped = true;
					m_agent.ResetPath();
				} else {
					yield return null;
				}
			} while (perpetual);

			m_agent.isStopped = true;
			m_agent.ResetPath();
			m_isActive = false;
		}
    }
}