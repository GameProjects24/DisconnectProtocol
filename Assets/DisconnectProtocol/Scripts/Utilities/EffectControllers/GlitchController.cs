using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DisconnectProtocol
{
    public class GlitchController : MonoBehaviour
    {
        public Material glitch;

		private int amountId;
		private Material m_glitch;
		private List<Renderer> m_rends = new List<Renderer>();

		private void Start() {
			m_glitch = new Material(glitch);
			GetComponentsInChildren(m_rends);
			amountId = Shader.PropertyToID("_Glitch_Amount");
		}

		public void Prepare() {
			m_rends.ForEach(r => r.sharedMaterial = m_glitch);
		}

		public void Glitch(float amount, float time) {
			StartCoroutine(GlitchCoroutine(amount, time));
		}

		private IEnumerator GlitchCoroutine(float amount, float time) {
			m_rends.ForEach(r => r.sharedMaterial.SetFloat(amountId, amount));
			yield return new WaitForSeconds(time);
			m_rends.ForEach(r => r.sharedMaterial.SetFloat(amountId, 0));
		}
    }
}
