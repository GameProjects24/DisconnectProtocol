using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace DisconnectProtocol
{
	public enum BodyState {
		Idle, Walk,
	}

	public enum BodyAction {
		AimStart, AimStop,
	}

    public class SoldierBodyController : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent m_agent;
		[SerializeField] private Transform m_eyes;
		[SerializeField] private float m_visionAngle = 30;
		[SerializeField] private float m_holdDistance = 10;
		[Range(0, 1)]
		[SerializeField] private float m_rotationRate = .5f;

		public event System.Action<BodyState> BodyStateChanged;
		private BodyState m_curState = BodyState.Idle;
		public event System.Action<BodyAction> BodyActionPerformed;

		private Aim m_aimAction;
		private HoldDistance m_holdDistanceAction;
		private CanSeeTarget m_canSee;

		private void Start() {
			if (m_agent == null) {
				m_agent = GetComponentInParent<NavMeshAgent>();
			}
			m_aimAction = new Aim(this, m_rotationRate);
			m_holdDistanceAction = new HoldDistance(this, m_agent, m_holdDistance);
			m_canSee = new CanSeeTarget(m_eyes ? m_eyes : transform, m_visionAngle);
		}

		private void ChangeState(BodyState state) {
			if (m_curState == state) {
				return;
			}
			m_curState = state;
			BodyStateChanged?.Invoke(state);
		}

		public bool CanSeeTarget(Transform target) {
			return m_canSee.Eval(target);
		}

		public void HoldDistanceStart(Transform target, bool perpetual) {
			ChangeState(BodyState.Walk);
			m_holdDistanceAction.Start(target, perpetual);
		}

		public void HoldDistanceStop() {
			ChangeState(BodyState.Idle);
			m_holdDistanceAction.Stop();
		}

		public void AimStart(Transform target, bool perpetual) {
			BodyActionPerformed?.Invoke(BodyAction.AimStart);
			m_aimAction.Start(target, perpetual);
		}

		public void AimStop() {
			BodyActionPerformed?.Invoke(BodyAction.AimStop);
			m_aimAction.Stop();
		}
    }
}
