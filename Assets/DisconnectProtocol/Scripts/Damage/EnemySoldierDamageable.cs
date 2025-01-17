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
		private float m_hpiRate;
		private int m_imageIdx;

		private void Start() {
			ChangeHpImage(hpImages.Length - 1);
			m_hpiRate = hpImages.Length / hp;
		}

		public override void TakeDamage(float damage) {
			if (hp <= 0) return;
			if ((hp -= damage) <= 0) {
				Die();
				return;
			}

			var nidx = (int)(hp * m_hpiRate);
			if (nidx != m_imageIdx) {
				ChangeHpImage(nidx);
			}
		}

		private void Die() {
			rawImage.gameObject.SetActive(false);
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