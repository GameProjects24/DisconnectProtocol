using UnityEngine;

namespace DisconnectProtocol
{
    public class KeepObject : MonoBehaviour
    {
        private static KeepObject m_instance;
		private void Start() {
			if (m_instance != null) {
				Destroy(gameObject);
				return;
			}

			m_instance = this;
			DontDestroyOnLoad(gameObject);
		}
    }
}
