using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace DisconnectProtocol
{
	public enum BodyState {
		Aim,
	}

    public class BodyController : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent m_agent;
		[SerializeField] private Transform m_eyes;
		[SerializeField] private Transform m_weapon;
		[SerializeField] private float m_visionAngle = 30;
		[SerializeField] private float m_holdDistance = 10;
		[Range(0, 1)]
		[SerializeField] private float m_rotationRate = .5f;

		public event System.Action<BodyState> BodyStateChanged;

		private float m_hd2;
		private bool m_wasSeen = false;
		private bool m_isAim = false;

		private const float ANGLE_EPS = 0.1f;

		private void Start() {
			m_hd2 = m_holdDistance * m_holdDistance;
			if (m_agent == null) {
				m_agent = GetComponentInParent<NavMeshAgent>();
			}
		}

		public bool CanSeeTarget(Transform target) {
			var self = m_eyes == null ? transform : m_eyes;
			var dir = target.position - self.position;
			bool inAngle = Vector3.Angle(self.forward, dir) <= m_visionAngle;
			if (inAngle && !m_wasSeen) {
				bool noObstacles;
				if (Physics.Linecast(self.position, target.position, out var hit)) {
					noObstacles = hit.transform.root.CompareTag("Player");
				} else {
					noObstacles = true;
				}
				return m_wasSeen = noObstacles;
			}

			return inAngle;
		}

		public void HoldDistance(GameObject target) {
			StartCoroutine(HoldDistanceCor(target));
		}

		private IEnumerator HoldDistanceCor(GameObject target) {
			m_agent.isStopped = false;
			m_agent.SetDestination(target.transform.position);
			yield return null;

			while (true) {
				if (Vector3.SqrMagnitude(target.transform.position - transform.position) > m_hd2) {
					m_agent.SetDestination(target.transform.position);
				} else {
					m_agent.isStopped = true;
					m_agent.ResetPath();
				}
				yield return null;
			}
		}

		public void Aim(Transform target) {
			if (m_isAim) {
				return;
			}
			m_isAim = true;
			BodyStateChanged?.Invoke(BodyState.Aim);
			StartCoroutine(AimCor(target));
		}

		private IEnumerator AimCor(Transform target) {
			float dif;
			do {
				var dir = target.position - transform.position;
				dir.y = 0;
				dif = transform.rotation.eulerAngles.y;
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), m_rotationRate);
				dif -= transform.rotation.eulerAngles.y;

				yield return null;
			} while (Mathf.Abs(dif) > ANGLE_EPS);
			m_isAim = false;
		}
    }
}
