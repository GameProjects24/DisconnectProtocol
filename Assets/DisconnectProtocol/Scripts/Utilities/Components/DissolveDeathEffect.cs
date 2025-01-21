using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DisconnectProtocol
{
    public class DissolveDeathEffect : MonoBehaviour
    {
		public Damageable body;
        public Material dissolve;
		public float time = 0.5f;

		private float m_time = 0;
		private int m_amountId;
		private Material m_dissolve;
		private List<Renderer> m_rends = new List<Renderer>();

		private void Start() {
			if (time > 0) {
				m_time = 1 / time;
			}
			m_dissolve = new Material(dissolve);
			m_amountId = Shader.PropertyToID("_Dissolve_Amount");
			GetComponentsInChildren(m_rends);
		}

		private void OnEnable() {
			body.OnDieIEnum += Dissolve;
		}

		private void OnDisable() {
			body.OnDieIEnum -= Dissolve;
		}

		private IEnumerator Dissolve() {
			float elapsed = Time.deltaTime;
			m_rends.ForEach(r => r.sharedMaterial = m_dissolve);
			while (elapsed < time) {
				m_rends.ForEach(r => r.sharedMaterial.SetFloat(m_amountId, elapsed * m_time));
				yield return null;
				elapsed += Time.deltaTime;
			}
		}
    }
}
