using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

namespace DisconnectProtocol
{
	[RequireComponent(typeof(Animator))]
    public class MechaAnimatorController : MonoBehaviour
    {
		[SerializeField] private SoldierBodyController m_body;
        private Animator m_animator;
		private List<Rigidbody> m_rbs = new List<Rigidbody>();

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
					m_body.BodyStateChanged += OnBodyStateChanged;
				}
			} else {
				m_body.BodyStateChanged += OnBodyStateChanged;
			}
		}

		private void OnDisable() {
			if (m_body) {
				m_body.BodyStateChanged -= OnBodyStateChanged;
			}
		}

		private void OnBodyStateChanged(BodyState state) {
			switch (state) {
			case BodyState.Aim:
				Debug.Log("Aim");
				break;
			}
		}
    }
}