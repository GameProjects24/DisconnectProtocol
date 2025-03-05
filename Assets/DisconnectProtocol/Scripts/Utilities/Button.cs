using UnityEngine;

namespace DisconnectProtocol
{
	public class Button : MonoBehaviour {
		public BasicDoor door;
		public Material material;
		[ColorUsage(true, true)] public Color activeColor;
		[ColorUsage(true, true)] public Color nonActiveColor;

		private Renderer m_renderer;
		private int m_emiColorId;

		private void Start() {
			m_renderer = GetComponentInChildren<Renderer>();
			m_renderer.sharedMaterial = new Material(material);
			m_emiColorId = Shader.PropertyToID("_EmissionColor");
			if (door != null) {
				door.StateChanging += OnStateChanging;
				door.StateChanged += OnStateChanged;
			}
			switch (door.currentState) {
				case IDoor.State.Open:
				m_renderer.sharedMaterial.SetColor(m_emiColorId, activeColor);
				break;

				case IDoor.State.Close:
				m_renderer.sharedMaterial.SetColor(m_emiColorId, nonActiveColor);
				break;
			};
		}

		private void OnStateChanging(IDoor.State state) {
			if (state == IDoor.State.Close) {
				m_renderer.sharedMaterial.SetColor(m_emiColorId, nonActiveColor);
			}
		}

		private void OnStateChanged(IDoor.State state) {
			if (state == IDoor.State.Open) {
				m_renderer.sharedMaterial.SetColor(m_emiColorId, activeColor);
			}
		}
	}
}
