using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace DisconnectProtocol
{
	public class EnemySoldierDamageable : Damageable
	{
		public float hp = 40f;
		public Texture[] hpImages;
		public RawImage rawImage;
		public GlitchController glitchController;
		public DissolveController dissolveController;

		private float m_hpiRate;
		private int m_imageIdx;

		private void Start() {
			ChangeHpImage(hpImages.Length - 1);
			m_hpiRate = hpImages.Length / hp;
			glitchController.Prepare();
		}

		public override void TakeDamage(float damage) {
			if (hp <= 0) return;
			if ((hp -= damage) <= 0) {
				Die();
				return;
			}

			glitchController.Glitch(.3f, .5f);
			var nidx = (int)(hp * m_hpiRate);
			if (nidx != m_imageIdx) {
				ChangeHpImage(nidx);
			}
		}

		private void Die() {
			rawImage.gameObject.SetActive(false);
			dissolveController.Prepare();
			dissolveController.Dissolve(1f);
			Debug.Log("Died!");
		}

		private void ChangeHpImage(int idx) {
			if (idx >= hpImages.Length) {
				idx = hpImages.Length - 1;
			}
			m_imageIdx = idx;
			rawImage.texture = hpImages[idx];
		}
	}
}