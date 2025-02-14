using UnityEngine;
using UnityEngine.AI;

namespace DisconnectProtocol
{
    public class DroneBodyController : MonoBehaviour, IHoldDistance
    {
		public enum BodyState {
			Walk,
		}

		[SerializeField] private NavMeshAgent m_agent;
		[SerializeField] private float m_holdDistance = 10f;
        
		private HoldDistance m_hda;

		private void Start()
		{
			if (m_agent == null) {
				m_agent = GetComponentInChildren<NavMeshAgent>();
			}

			m_hda = new HoldDistance(this, m_agent, m_holdDistance);
		}

		void IHoldDistance.Start(Transform target, bool perpetual) {
			Debug.Log($"Drone");
			m_hda.Start(target, perpetual);
		}

		void IHoldDistance.Stop() {
			m_hda.Stop();
		}
	}
}
