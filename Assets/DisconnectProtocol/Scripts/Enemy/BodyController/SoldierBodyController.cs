using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace DisconnectProtocol
{
    public class SoldierBodyController : MonoBehaviour, IHoldDistance {
		public enum BodyState {
			Idle, Walk,
		}

		public enum BodyAction {
			AimStart, AimStop,
		}

        [SerializeField] private NavMeshAgent m_agent;
		[SerializeField] private Transform m_eyes;
		[SerializeField] private float m_visionAngle = 30;
		[SerializeField] private float m_holdDistance = 10;
		[Range(0, 1)]
		[SerializeField] private float m_rotationRate = .5f;

		public event System.Action<BodyState> BodyStateChanged;
		private BodyState m_curState = BodyState.Idle;
		private IStoppable m_curStoppable;
		public event System.Action<BodyAction> BodyActionPerformed;

		private Aim m_aimAction;
		private HoldDistance m_holdDistanceAction;
		private CanSeeTarget m_canSee;

		private void Start() {
			if (m_agent == null) {
				m_agent = GetComponentInParent<NavMeshAgent>();
			}
			m_aimAction = new Aim(this, m_rotationRate);
			m_aimAction.Stopped += OnAimStop;

			m_holdDistanceAction = new HoldDistance(this, m_agent, m_holdDistance);
			m_holdDistanceAction.Stopped += OnHoldDistanceStop;
			m_holdDistanceAction.Paused += OnHoldDistancePause;
			m_holdDistanceAction.Resumed += OnHoldDistanceResume;

			m_canSee = new CanSeeTarget(m_eyes ? m_eyes : transform, m_visionAngle);
		}

		private void ChangeState(BodyState state) {
			if (m_curState == state) {
				return;
			}
			m_curState = state;
			BodyStateChanged?.Invoke(state);
		}

		private void ChangeStoppable(IStoppable stp) {
			if (m_curStoppable == stp) {
				return;
			}
			m_curStoppable?.Stop();
			m_curStoppable = stp;
		}


		public bool CanSeeTarget(Transform target) {
			return m_canSee.Eval(target);
		}


		void IHoldDistance.Start(Transform target, bool perpetual) {
			ChangeState(BodyState.Walk);
			ChangeStoppable(m_holdDistanceAction);
			m_holdDistanceAction.Start(target, perpetual);
		}

		void IHoldDistance.Stop() {
			m_holdDistanceAction.Stop();
		}

		private void OnHoldDistanceStop() {
			ChangeState(BodyState.Idle);
			m_curStoppable = null;
		}

		private void OnHoldDistancePause() {
			ChangeState(BodyState.Idle);
		}

		private void OnHoldDistanceResume() {
			ChangeState(BodyState.Walk);
		}


		public void AimStart(Transform target, bool perpetual) {
			BodyActionPerformed?.Invoke(BodyAction.AimStart);
			m_aimAction.Start(target, perpetual);
		}

		public void AimStop() {
			m_aimAction.Stop();
		}

		private void OnAimStop() {
			BodyActionPerformed?.Invoke(BodyAction.AimStop);
		}
    }
}
