using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace DisconnectProtocol
{
	public class HealthDisplay : MonoBehaviour
	{
		public Damageable body;
		public Texture[] hpImages;
		public RawImage rawImage;

		private float m_hpiRate;
		private int m_imageIdx;

		private void Start() {
			m_hpiRate = hpImages.Length / body.maxHp;
		}

		private void OnEnable() {
			rawImage.gameObject.SetActive(true);
			ChangeHpImage(hpImages.Length - 1);
			body.OnDamage += TakeDamage;
			body.OnDie += RemoveImage;
		}

		private void OnDisable() {
			body.OnDamage -= TakeDamage;
			body.OnDie -= RemoveImage;
		}

		private void RemoveImage() {
			rawImage.gameObject.SetActive(false);
		}

		public void TakeDamage() {
			var nidx = (int)(body.hp * m_hpiRate);
			if (nidx != m_imageIdx && nidx < hpImages.Length) {
				ChangeHpImage(nidx);
			}
		}

		private void ChangeHpImage(int idx) {
			m_imageIdx = idx;
			rawImage.texture = hpImages[idx];
		}
	}
}