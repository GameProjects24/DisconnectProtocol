using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace DisconnectProtocol
{
    public class DroneBodyController : MonoBehaviour, IHoldDistance
    {
		public enum State {
			Following,
		}

		[Header("Control Center")]
		[SerializeField] private BehaviorGraphAgent m_brain;
		[SerializeField] private NavMeshAgent m_agent;
		[SerializeField] private Damageable m_dmg;

		[Header("Parameters")]
		[SerializeField] private float m_holdDistance = 10f;
        
		private Transform m_tr;

		private State m_curState;
		private IStoppable m_curStoppable;

		private HoldDistance m_hda;

		private void Start() {
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

			if (m_agent == null) {
				m_agent = gameObject.GetComponentWherever<NavMeshAgent>();
			}
			m_agent.enabled = true;

			if (m_brain == null) {
				m_brain = gameObject.GetComponentWherever<BehaviorGraphAgent>();
			}
			m_brain.enabled = true;
		}

		private void OnDisable() {
			m_dmg.OnDie -= OnDie;
			m_curStoppable?.Stop();
			m_agent.enabled = false;
			m_brain.enabled = false;
		}

		private void OnDie() => enabled = false;

		private void LateUpdate() {
			var ap = m_tr.position;
			ap.y = 3;
			m_tr.position = ap;
		}

		private void ChangeStoppable(IStoppable stp) {
			if (m_curStoppable == stp) {
				return;
			}
			m_curStoppable?.Stop();
			m_curStoppable = stp;
		}

		void IHoldDistance.Start(Transform target, bool perpetual) {
			ChangeStoppable(m_hda);
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
