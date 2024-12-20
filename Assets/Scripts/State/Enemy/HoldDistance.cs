using UnityEngine;
using UnityEngine.AI;

namespace State.Enemy {
    public class HoldDistance : State {
        [SerializeField] private Transform m_target;
        [SerializeField] private float m_speed = 10f;
        [SerializeField] private float m_distance = 7f;
        private NavMeshAgent m_body;

        private void Awake() {
            m_body = GetComponentInParent<NavMeshAgent>();
        }

        public override void Enter() {
            m_body.speed = m_speed;
            m_body.isStopped = false;
        }

        public override void Act() {
            var dir = m_target.position - m_body.transform.position;
            m_body.SetDestination(m_target.position);
            if (dir.magnitude < m_distance) {
                m_body.isStopped = true;
                m_body.ResetPath();
            }
        }

        public override void Exit() {
            m_body.isStopped = true;
            m_body.ResetPath();
        }
    }
}
