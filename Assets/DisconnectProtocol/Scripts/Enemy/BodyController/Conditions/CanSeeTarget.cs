using UnityEngine;

namespace DisconnectProtocol
{
    public class CanSeeTarget
    {
        public float visionAngle;
		private Transform m_eyes;

		private bool m_wasSeen = false;

		private CanSeeTarget() {}
		public CanSeeTarget(Transform eyes, float visAngle) {
			m_eyes = eyes;
			visionAngle = visAngle;
		}

		public bool Eval(Transform target) {
			var dir = target.position - m_eyes.position;
			bool inAngle = Vector3.Angle(m_eyes.forward, dir) <= visionAngle;
			if (inAngle && !m_wasSeen) {
				bool noObstacles = true;
				if (Physics.Linecast(m_eyes.position, target.position, out var hit)) {
					noObstacles = hit.transform.root.GetInstanceID() == target.GetInstanceID();
				}
				return m_wasSeen = noObstacles;
			}

			return inAngle;
		}
    }
}
