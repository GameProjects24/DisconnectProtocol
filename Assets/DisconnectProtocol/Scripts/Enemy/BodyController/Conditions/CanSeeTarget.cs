using UnityEngine;

namespace DisconnectProtocol
{
    public class CanSeeTarget
    {
        public float visionAngle;
		private Transform m_eyes;

		private CanSeeTarget() {}
		public CanSeeTarget(Transform eyes, float visAngle) {
			m_eyes = eyes;
			visionAngle = visAngle;
		}

		public bool Eval(Transform target) {
			var dir = target.position - m_eyes.position;
			bool inAngle = Vector3.Angle(m_eyes.forward, dir) <= visionAngle;
			if (!inAngle) {
				return false;
			}
			
			bool noObstacles = true;
			if (Physics.Raycast(m_eyes.position, dir.normalized, out var hit)) {
				var wh = hit.transform.GetComponentInParent<Whole>();
				int hitId = hit.transform.GetInstanceID();
				if (wh != null) {
					hitId = wh.transform.GetInstanceID();
				}
				noObstacles = hitId == target.GetInstanceID();
			}
			return noObstacles;
		}
    }
}
