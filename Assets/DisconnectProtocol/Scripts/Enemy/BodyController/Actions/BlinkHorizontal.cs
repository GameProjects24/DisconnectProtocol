using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace DisconnectProtocol
{
	public interface IBlinkHorizontal {
		bool TryStart(Transform target);
		void Stop();
	}

    public class BlinkHorizontal : IStoppable {
		private NavMeshAgent m_agent;
		private MonoBehaviour m_ctrl;
		private Coroutine m_cor;

		public float angleMin, angleMax;
		public float distMin, distMax;

		private Transform m_agentTr;

		private const float EPS = .1f;

		public event System.Action Stopped;

		private BlinkHorizontal() {}
		public BlinkHorizontal(MonoBehaviour ctrl, NavMeshAgent agent, float angleMin, float angleMax, float distMin, float distMax) {
			m_ctrl = ctrl;
			m_agent = agent;
			m_agentTr = agent.transform;
			this.angleMin = angleMin;
			this.angleMax = angleMax;
			this.distMin = distMin;
			this.distMax = distMax;
		}

		public bool TryStart(Transform target) {
			if (m_cor != null) {
				return true;
			}
			return TryStartInner(target);
		}

		public void Stop() {
			if (m_cor != null) {
				m_ctrl.StopCoroutine(m_cor);
				m_cor = null;
				Stopped?.Invoke();
			}
		}

		private bool TryStartInner(Transform target) {
			if (angleMin > angleMax) {
				(angleMin, angleMax) = (angleMax, angleMin);
			}
			if (distMin > distMax) {
				(distMin, distMax) = (distMax, distMin);
			}

			var pos = FindPos(target);
			if (pos.HasValue) {
				m_cor = m_ctrl.StartCoroutine(Cor(pos.Value));
				return true;
			}
			return false;
		}

		private Vector3? FindPos(Transform target) {
			float angleRand = Random.Range(angleMin, angleMax);
			System.Span<float> angles = stackalloc[] {
				angleRand, angleMin, angleMax,
				-angleRand, -angleMin, -angleMax,
			};
			System.Span<float> dists = stackalloc[] {
				Random.Range(distMin, distMax), distMin, distMax
			};

			float dir = Random.Range(0f, maxInclusive: 1f);
			if (dir >= 0.5) {
				for (int i = 0; i < 6; ++i) {
					angles[i] = -angles[i];
				}
			}

			NavMeshHit hit;
			var dif = (target.position - m_agentTr.position).normalized;
			foreach (var a in angles) {
				var newDif = Quaternion.AngleAxis(a, Vector3.up) * dif;
				foreach (var d in dists) {
					if (NavMesh.SamplePosition(target.position + newDif * d, out hit, m_agent.height * 2f, NavMesh.AllAreas)) {
						return hit.position;
					}
				}
			}
			return null;
		}

		private IEnumerator Cor(Vector3 pos) {
			m_agent.SetDestination(pos);
			yield return null;
			bool outDist, isPathAndVel;
			do {
				outDist = m_agent.remainingDistance > m_agent.stoppingDistance;
				isPathAndVel = m_agent.hasPath && m_agent.velocity.sqrMagnitude > 0f;
				yield return new WaitForFixedUpdate();
			} while (outDist || isPathAndVel);
			m_agent.ResetPath();
			m_cor = null;
			Stopped?.Invoke();
		}
    }
}