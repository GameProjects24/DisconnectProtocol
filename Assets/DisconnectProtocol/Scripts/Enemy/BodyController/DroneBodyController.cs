using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using DBC = DisconnectProtocol.DroneBodyController;

namespace DisconnectProtocol
{
    public class DroneBodyController : BodyController<DBC.State, DBC.Action>,
	IHoldDistance, IBlinkHorizontal
    {
		public enum State {
			Idle, Follow, Blink
		}

		public enum Action {
			
		}

		[Header("Control Center")]
		[SerializeField] private BehaviorGraphAgent m_brain;
		[SerializeField] private NavMeshAgent m_agent;
		[SerializeField] private Damageable m_dmg;

		[Header("Hold Distance")]
		[SerializeField] private float m_holdDistance = 10f;

		[Header("Blink")]
		[SerializeField] private float m_angleMin = 10f;
		[SerializeField] private float m_angleMax = 90f;
		[SerializeField] private float m_targetDistMin = 2f;
		[SerializeField] private float m_targetDistMax = 5f;
        
		private Transform m_tr;

		private HoldDistance m_hda;
		private BlinkHorizontal m_blih;

		private void Start() {
			m_tr = transform;

			m_hda = new HoldDistance(this, m_agent, m_holdDistance);
			m_hda.Paused += OnStatePause;
			m_hda.Resumed += OnStateResume;
			m_hda.Stopped += OnStateStop;
			m_states.Add(m_hda, new FlowState(
				start: State.Follow, stop: State.Idle,
				resume: State.Follow, pause: State.Idle
			));

			m_blih = new BlinkHorizontal(this, m_agent);
			m_blih.angleMin = m_angleMin;
			m_blih.angleMax = m_angleMax;
			m_blih.distMin = m_targetDistMin;
			m_blih.distMax = m_targetDistMax;
			m_blih.Stopped += OnStateStop;
			m_states.Add(m_blih, new FlowState(
				start: State.Blink, stop: State.Idle
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

		bool IBlinkHorizontal.TryStart(Transform target) {
			ChangeStoppable(m_blih);
			return m_blih.TryStart(target);
		}

		void IBlinkHorizontal.Stop() {
			m_blih.Stop();
		}
	}
}