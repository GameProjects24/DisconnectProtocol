using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using SBC = DisconnectProtocol.SoldierBodyController;

namespace DisconnectProtocol
{
	[RequireComponent(typeof(Animator))]
    public class MechaAnimController : MonoBehaviour
    {
		[SerializeField] private SBC m_body;
        private Animator m_animator;
		private List<Rigidbody> m_rbs = new List<Rigidbody>();

		private int m_aimt = Animator.StringToHash("Aim");
		private int m_idlet = Animator.StringToHash("Idle");
		private int m_walkt = Animator.StringToHash("Walk");
		private int m_runt = Animator.StringToHash("Run");
		private int m_curt;

		private void OnEnable() {
			if (!m_animator) {
				m_animator = GetComponent<Animator>();
			}
			if (m_rbs.Count == 0) {
				GetComponentsInChildren(m_rbs);
			}
			RagdollController.ChangeMode(RCMode.Animator, m_animator, m_rbs);
			
			if (m_body == null) {
				if (TryGetComponent(out m_body)) {
					m_body.BodyStateChanged += OnStateChanged;
					m_body.BodyActionPerformed += OnActionPerformed;
				}
			} else {
				m_body.BodyStateChanged += OnStateChanged;
				m_body.BodyActionPerformed += OnActionPerformed;
			}
		}

		private void OnDisable() {
			if (m_body) {
				m_body.BodyStateChanged -= OnStateChanged;
				m_body.BodyActionPerformed -= OnActionPerformed;
			}
		}

		private void OnStateChanged(SBC.BodyState state) {
			m_animator.ResetTrigger(m_curt);
			switch (state) {
				case SBC.BodyState.Idle:
					m_curt = m_idlet;
					break;
				case SBC.BodyState.Walk:
					m_curt = m_walkt;
					break;
			}
			m_animator.SetTrigger(m_curt);
		}

		private void OnActionPerformed(SBC.BodyAction action) {
			switch (action) {
				case SBC.BodyAction.AimStart:
					m_animator.SetTrigger(m_aimt);
					break;
				case SBC.BodyAction.AimStop:
					m_animator.ResetTrigger(m_aimt);
					break;
			}
		}
    }
}
