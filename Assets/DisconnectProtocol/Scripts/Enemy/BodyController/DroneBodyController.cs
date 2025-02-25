using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using DBC = DisconnectProtocol.DroneBodyController;

namespace DisconnectProtocol
{
    public class DroneBodyController : BodyController<DBC.State, DBC.Action>, IHoldDistance
    {
		public enum State {
			Idle, Following,
		}

		public enum Action {
			
		}

		[Header("Control Center")]
		[SerializeField] private BehaviorGraphAgent m_brain;
		[SerializeField] private NavMeshAgent m_agent;
		[SerializeField] private Damageable m_dmg;

		[Header("Parameters")]
		[SerializeField] private float m_holdDistance = 10f;
        
		private Transform m_tr;

		private HoldDistance m_hda;

		private void Start() {
			m_tr = transform;

			m_hda = new HoldDistance(this, m_agent, m_holdDistance);
			m_hda.Paused += OnStatePause;
			m_hda.Resumed += OnStateResume;
			m_hda.Stopped += OnStateStop;
			m_states.Add(m_hda, new FlowState(
				start: State.Following, stop: State.Idle,
				resume: State.Following, pause: State.Idle
			));
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
			ChangeStoppable(null);
			m_agent.enabled = false;
			m_brain.enabled = false;
		}

		private void OnDie() => enabled = false;

		private void LateUpdate() {
			var ap = m_tr.position;
			ap.y = 3;
			m_tr.position = ap;
		}

		void IHoldDistance.Start(Transform target, bool perpetual) {
			ChangeStoppable(m_hda);
			m_hda.Start(target, perpetual);
		}

		void IHoldDistance.Stop() {
			m_hda.Stop();
		}
	}
}