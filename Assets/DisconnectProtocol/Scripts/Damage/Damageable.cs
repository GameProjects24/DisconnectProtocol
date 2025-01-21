using System.Collections;
using UnityEngine;

namespace DisconnectProtocol
{	
	public class Damageable : MonoBehaviour
	{
		[SerializeField] private float m_maxHp = 100f;
		public float maxHp { get => m_maxHp; }
		public float hp { get; private set; }
		public event System.Action OnDamage;
		public event System.Action OnDie;
		public event System.Func<IEnumerator> OnDieIEnum;

		private void OnEnable() {
			hp = m_maxHp;
		}

		public void TakeDamage(float damage) {
			if (hp <= 0) return;
			if ((hp -= damage) <= 0) {
				StartCoroutine(Die());
				return;
			}
			OnDamage?.Invoke();
		}

		private IEnumerator Die() {
			OnDie?.Invoke();
			foreach (System.Func<IEnumerator> hand in OnDieIEnum.GetInvocationList()) {
				yield return hand.Invoke();
			}
			transform.root.gameObject.SetActive(false);
		}
    }
}
