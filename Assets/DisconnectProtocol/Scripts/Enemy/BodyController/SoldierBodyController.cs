using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using SBC = DisconnectProtocol.SoldierBodyController;

namespace DisconnectProtocol
{
    public class SoldierBodyController : BodyController<SBC.State, SBC.Action>,
	IHoldDistance, IAim, ICanSeeTarget
	{
		public enum State {
			Idle, Walk,
		}

		public enum Action {
			AimStart, AimStop,
		}

        [SerializeField] private NavMeshAgent m_agent;
		[SerializeField] private Transform m_eyes;
		[SerializeField] private float m_visionAngle = 30;
		[SerializeField] private float m_holdDistance = 10;
		[Range(0, 1)]
		[SerializeField] private float m_rotationRate = .5f;

		private Aim m_aimAction;
		private HoldDistance m_hda;
		private CanSeeTarget m_canSee;

		private void Start() {
			if (m_agent == null) {
				m_agent = GetComponentInParent<NavMeshAgent>();
			}

			m_aimAction = new Aim(this, m_rotationRate);
			m_aimAction.Stopped += () => OnActionDone(Action.AimStop);

			m_hda = new HoldDistance(this, m_agent, m_holdDistance);
			m_hda.Stopped += OnStateStop;
			m_hda.Paused += OnStatePause;
			m_hda.Resumed += OnStateResume;
			m_states.Add(m_hda, new FlowState(
				stop: State.Idle, start: State.Walk,
				pause: State.Idle, resume: State.Walk
			));

			m_canSee = new CanSeeTarget(m_eyes ? m_eyes : transform, m_visionAngle);
		}

		bool ICanSeeTarget.Eval(Transform target) {
			return m_canSee.Eval(target);
		}

		void IHoldDistance.Start(Transform target, bool perpetual) {
			ChangeStoppable(m_hda);
			m_hda.Start(target, perpetual);
		}

		void IHoldDistance.Stop() {
			m_hda.Stop();
		}

		void IAim.Start(Transform target, bool perpetual) {
			OnActionDone(Action.AimStart);
			m_aimAction.Start(target, perpetual);
		}

		void IAim.Stop() {
			m_aimAction.Stop();
		}
	}
}