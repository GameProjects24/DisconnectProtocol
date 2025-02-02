using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

namespace DisconnectProtocol
{
	[RequireComponent(typeof(Animator))]
    public class MechaAnimatorController : MonoBehaviour
    {
		[SerializeField] private BehaviorGraphAgent m_agent;
		private BlackboardVariable<AgentStateChanged> m_stateChanged;
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

			if (m_agent && m_stateChanged == null) {
				if (m_agent.BlackboardReference.GetVariable("AgentStateChanged", out m_stateChanged)) {
					if (m_stateChanged.Value == null) {
						m_stateChanged.Value = ScriptableObject.CreateInstance<AgentStateChanged>();
					}
				}
			}
			if (m_stateChanged != null) {
				m_stateChanged.Value.Event += OnAgentStateChanged;
			}
		}

		private void OnDisable() {
			if (m_stateChanged != null) {
				m_stateChanged.Value.Event -= OnAgentStateChanged;
			}
		}

		private void OnAgentStateChanged(AgentState state) {
			switch (state) {
			case AgentState.HipAim:
				m_animator.enabled = false;
				break;
			}
		}
    }
}