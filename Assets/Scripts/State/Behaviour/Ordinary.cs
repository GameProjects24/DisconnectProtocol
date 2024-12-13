using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace DisconnectProtocol
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Ordinary : MonoBehaviour
    {
        public Transform target;
        public float speed = 10f;
        public float distanceVision = 15f;
        public float angleVision = 45f;
        private NavMeshAgent m_self;
        private Idle m_idle;
        private HoldDistance m_hd;
        private EnemyState m_curState;

        private void Start() {
            m_self = GetComponent<NavMeshAgent>();
            m_idle = new Idle(m_self);
            m_hd = new HoldDistance(m_self, target, angleVision, distanceVision, speed);
            m_hd.OnExit += () => { m_curState = m_idle; };
            m_curState = m_idle;
        }

        private void Update() {
            if (m_curState == m_idle && m_hd.CanEnter()) {
                m_curState = m_hd;
            }
            m_curState.Update();
        }
    }
}