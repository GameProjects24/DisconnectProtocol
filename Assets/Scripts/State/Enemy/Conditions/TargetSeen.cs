using UnityEngine;

namespace State.Enemy {
    public class TargetSeen : Condition {
        [SerializeField] private Transform m_target;
        [SerializeField] float m_visionAngle = 30f;
        [SerializeField] float m_visionDist = 10f;

        public override bool Eval() {
            var dir = m_target.position - transform.position;
            return dir.sqrMagnitude < m_visionDist * m_visionDist
                && Vector3.Angle(dir, transform.forward) < m_visionAngle;
        }
    }
}
