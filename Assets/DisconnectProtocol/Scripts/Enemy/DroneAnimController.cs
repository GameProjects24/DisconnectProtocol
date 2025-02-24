using System.Collections.Generic;
using UnityEngine;

namespace DisconnectProtocol
{
	[RequireComponent(typeof(Animator))]
    public class DroneAnimController : MonoBehaviour {
        [SerializeField] private Damageable m_dmg;
		private Animator m_animator;
		private List<Rigidbody> m_rbs = new List<Rigidbody>();

		private void Start() {
			m_animator = GetComponent<Animator>();
			GetComponentsInChildren(m_rbs);
		}

		private void OnEnable() {
			if (m_dmg == null) {
				m_dmg = gameObject.GetComponentWherever<Damageable>();
			}
			m_dmg.OnDie += OnDie;
		}

		private void OnDisable() {
			m_dmg.OnDie -= OnDie;
		}

		private void OnDie() {
			RagdollController.ChangeMode(RCMode.GravityRagdoll, m_animator, m_rbs);
		}
	}
}
