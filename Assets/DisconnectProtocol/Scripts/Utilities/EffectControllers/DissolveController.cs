using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DisconnectProtocol
{
    public class DissolveController : MonoBehaviour
    {
        public Material dissolve;

		private int amountId;
		private Material m_dissolve;
		private List<Renderer> m_rends = new List<Renderer>();

		private void Start() {
			m_dissolve = new Material(dissolve);
			GetComponentsInChildren(m_rends);
			amountId = Shader.PropertyToID("_Dissolve_Amount");
		}

		public void Prepare() {
			m_rends.ForEach(r => r.sharedMaterial = m_dissolve);
		}

		public void Dissolve(float time) {
			if (time <= 0) {
				return;
			}
			StartCoroutine(DissolveCoroutine(time));
		}

		private IEnumerator DissolveCoroutine(float time) {
			float total = 1 / time;
			float elapsed = Time.deltaTime;
			while (elapsed < time) {
				m_rends.ForEach(r => r.sharedMaterial.SetFloat(amountId, elapsed * total));
				yield return null;
				elapsed += Time.deltaTime;
			}
		}
    }
}
