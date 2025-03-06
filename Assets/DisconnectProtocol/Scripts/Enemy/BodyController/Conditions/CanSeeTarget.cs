using UnityEngine;

namespace DisconnectProtocol
{
	public interface ICanSeeTarget {
		bool Eval(Transform target);
	}

    public class CanSeeTarget {
        public float visionAngle;
		private Transform m_eyes;

		private CanSeeTarget() {}
		public CanSeeTarget(Transform eyes, float visAngle) {
			m_eyes = eyes;
			visionAngle = visAngle;
		}

		public bool Eval(Transform target) {
			if (target == null) {
				return false;
			}
			var dir = target.position - m_eyes.position;
			bool inAngle = Vector3.Angle(m_eyes.forward, dir) <= visionAngle;
			if (!inAngle) {
				return false;
			}
			
			bool noObstacles = true;
			if (Physics.Raycast(m_eyes.position, dir.normalized, out var hit)) {
				var wh = hit.transform.GetComponentInParent<Whole>();
				int hitId = hit.transform.gameObject.GetInstanceID();
				if (wh != null) {
					hitId = wh.gameObject.GetInstanceID();
				}
				noObstacles = hitId == target.gameObject.GetInstanceID();
			}
			return noObstacles;
		}
    }
}
