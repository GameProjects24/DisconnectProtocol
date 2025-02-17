using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DisconnectProtocol
{
    public class GlitchDamageEffect : MonoBehaviour
    {
		public Damageable body;
        public Material glitch;
		[Range(0, 1)]
		public float amount = .3f;
		public float time = .5f;

		private int m_amountId;
		private Material m_glitch;
		private List<Renderer> m_rends = new List<Renderer>();

		private void Start() {
			m_glitch = new Material(glitch);
			m_amountId = Shader.PropertyToID("_Glitch_Amount");
			foreach (var r in GetComponentsInChildren<Renderer>()) {
				m_rends.Add(r);
				r.sharedMaterial = m_glitch;
			}
		}

		private void OnEnable() {
			if (body == null) {
				body = gameObject.GetComponentInParent<Damageable>();
			}
			body.OnDamage += Glitch;
		}

		private void OnDisable() {
			body.OnDamage -= Glitch;
		}

		public void Glitch() {
			StartCoroutine(GlitchCoroutine());
		}

		private IEnumerator GlitchCoroutine() {
			m_rends.ForEach(r => r.sharedMaterial.SetFloat(m_amountId, amount));
			yield return new WaitForSeconds(time);
			m_rends.ForEach(r => r.sharedMaterial.SetFloat(m_amountId, 0));
		}
    }
}
