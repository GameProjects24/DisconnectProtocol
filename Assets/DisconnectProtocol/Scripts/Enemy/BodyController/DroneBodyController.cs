using System.Collections;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using DBC = DisconnectProtocol.DroneBodyController;

namespace DisconnectProtocol
{
    public class DroneBodyController : BodyController<DBC.State, DBC.Action>,
	IHoldDistance, IBlinkHorizontal, IBlinkVertical, ICanSeeTarget, IAim
    {
		public enum State {
			Idle, Follow, Blink
		}

		public enum Action {
			AimStart, AimStop
		}

		[Header("Control Center")]
		[SerializeField] private BehaviorGraphAgent m_brain;
		[SerializeField] private NavMeshAgent m_agent;
		[SerializeField] private Damageable m_dmg;

		[Header("Hold Distance")]
		[SerializeField] private float m_holdDistance = 10f;

		[Header("Blink Horizontal")]
		[SerializeField] private float m_angleMin = 10f;
		[SerializeField] private float m_angleMax = 90f;
		[SerializeField] private float m_targetDistMin = 2f;
		[SerializeField] private float m_targetDistMax = 5f;

		[Header("Blink Vertical")]
		[SerializeField] private float m_altitudeMin = 0f;
		[SerializeField] private float m_altitudeMax = 2f;
		[SerializeField] private float m_desiredTime = 0.2f;

		[Header("Can See")]
		[SerializeField] private Transform m_eyes;
		[SerializeField] private float m_visionAngle = 40f;

		[Header("Aim")]
		[SerializeField] private float m_rotationRate = .5f;
		[SerializeField] private float m_effectiveDistance = 20f;
		[SerializeField] private Transform[] m_weaponParts;
        
		private Transform m_tr;
		private Vector3 m_hoverPos;
		private Coroutine m_cor;

		private HoldDistance m_hda;
		private BlinkHorizontal m_blih;
		private BlinkVertical m_bliv;
		private CanSeeTarget m_canSee;
		private Aim m_aim;

		private void Start() {
			m_hda = new HoldDistance(this, m_agent, m_holdDistance);
			m_hda.Paused += OnStatePause;
			m_hda.Resumed += OnStateResume;
			m_hda.Stopped += OnStateStop;
			m_states.Add(m_hda, new FlowState(
				start: State.Follow, stop: State.Idle,
				resume: State.Follow, pause: State.Idle
			));

			m_blih = new BlinkHorizontal(
				this, m_agent, m_angleMin, m_angleMax, m_targetDistMin, m_targetDistMax);
			m_blih.Stopped += OnStateStop;
			m_states.Add(m_blih, new FlowState(
				start: State.Blink, stop: State.Idle
			));

			m_bliv = new BlinkVertical(
				this, m_agent.height, m_altitudeMin, m_altitudeMax, m_desiredTime);
			m_bliv.Stopped += OnStateStop;
			m_bliv.Stopped += StartHover;
			m_states.Add(m_bliv, new FlowState(
				start: State.Blink, stop: State.Idle
			));

			m_canSee = new CanSeeTarget(m_eyes == null ? m_tr : m_eyes, m_visionAngle);
			m_aim = new Aim(this, m_rotationRate, m_effectiveDistance, m_weaponParts);
			m_aim.Stopped += () => OnActionDone(Action.AimStop);
		}

		private void OnEnable() {
			if (m_tr == null) {
				m_tr = transform;
			}
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
			StartHover();
		}

		private void OnDisable() {
			m_dmg.OnDie -= OnDie;
			ChangeStoppable(null);
			m_agent.enabled = false;
			m_brain.enabled = false;
		}

		private void OnDie() => enabled = false;

		private void StartHover() {
			m_hoverPos = m_tr.position;
			m_cor = StartCoroutine(Hover());
		}

		private void StopHover() {
			if (m_cor != null) {
				StopCoroutine(m_cor);
			}
		}

		private IEnumerator Hover() {
			m_tr.position = m_hoverPos;
			yield return null;
		}

		bool ICanSeeTarget.Eval(Transform target) {
			return m_canSee.Eval(target);
		}

		void IAim.Start(Transform target, bool perpetual) {
			OnActionDone(Action.AimStart);
			m_aim.Start(target, perpetual);
		}

		void IAim.Stop() {
			m_aim.Stop();
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

		bool IBlinkVertical.TryStart() {
			ChangeStoppable(m_bliv);
			if (m_bliv.TryStart()) {
				StopHover();
				return true;
			}
			return false;
		}

		void IBlinkVertical.Stop() {
			m_bliv.Stop();
		}
	}
}