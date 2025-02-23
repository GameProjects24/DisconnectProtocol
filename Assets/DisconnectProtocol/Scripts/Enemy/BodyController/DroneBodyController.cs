using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace DisconnectProtocol
{
    public class DroneBodyController : MonoBehaviour, IHoldDistance
    {
		public enum BodyState {
			Walk,
		}

		[SerializeField] private NavMeshAgent m_agent;
		[SerializeField] private Damageable m_dmg;
		[SerializeField] private BehaviorGraphAgent m_brain;
		[SerializeField] private float m_holdDistance = 10f;
        
		private Transform m_tr;
		private HoldDistance m_hda;

		private void Start() {
			if (m_agent == null) {
				m_agent = gameObject.GetComponentWherever<NavMeshAgent>();
			}
			if (m_brain == null) {
				m_brain = gameObject.GetComponentWherever<BehaviorGraphAgent>();
			}
			m_tr = transform;

			m_hda = new HoldDistance(this, m_agent, m_holdDistance);
			m_hda.Paused += OnHoldDistancePause;
			m_hda.Resumed += OnHoldDistanceResume;
			m_hda.Stopped += OnHoldDistanceStop;
		}

		private void OnEnable() {
			if (m_dmg == null) {
				m_dmg = gameObject.GetComponentWherever<Damageable>();
			}
			m_dmg.OnDie += OnDie;
		}

		private void OnDisable() {
			m_dmg.OnDie -= OnDie;
		}

		private void OnDie() {
			m_brain.enabled = false;
			m_agent.enabled = false;
			m_hda.Stop();
		}

		private void LateUpdate() {
			var ap = m_tr.position;
			ap.y = 3;
			m_tr.position = ap;
		}

		void IHoldDistance.Start(Transform target, bool perpetual) {
			// Debug.Log($"Drone");
			m_hda.Start(target, perpetual);
		}

		void IHoldDistance.Stop() {
			m_hda.Stop();
		}

		private void OnHoldDistanceStop() {
			Debug.Log("Drone: stopped");
		}

		private void OnHoldDistancePause() {
			Debug.Log("Drone: paused");
		}

		private void OnHoldDistanceResume() {
			Debug.Log("Drone: resumed");
		}
	}
}
