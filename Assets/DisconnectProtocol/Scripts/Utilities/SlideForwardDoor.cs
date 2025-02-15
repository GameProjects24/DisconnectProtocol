using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace DisconnectProtocol
{
    public class SlideForwardDoor : MonoBehaviour
    {
		public enum Op {
			Open = -1, Close = 1,
		}

		[SerializeField] private Op m_current = Op.Open;
		[Range(0, 1)]
		[SerializeField] private float m_rate = .1f;

		private Stack<Op> m_ops = new Stack<Op>();
		private Coroutine m_cor;
		private Transform m_tr;
		private Renderer m_rend;

		private const float EPS = .1f;

		private void Start() {
			m_tr = transform;
			m_rend = GetComponentInChildren<Renderer>();
		}

		private void Update() {
			if (Input.GetKeyDown(KeyCode.C)) {
				AddOp(Op.Close);
			} else if (Input.GetKeyDown(KeyCode.O)) {
				AddOp(Op.Open);
			}
		}

		public void AddOp(Op op) {
			if (!(m_ops.TryPeek(out var o) && o == op) && op != m_current) {
				m_ops.Push(op);
			}

			if (m_cor == null) {
				m_cor = StartCoroutine(PerformOp());
			}
		}

		private IEnumerator PerformOp() {
			while (m_ops.TryPop(out var op)) {
				m_current = op;
				var dest = m_tr.position + ElMul(m_tr.forward * (float)op, m_rend.bounds.size);
				do {
					yield return null;
					var vel = Vector3.zero;
					m_tr.position = Vector3.Lerp(m_tr.position, dest, m_rate);
				} while (ElEpsGr(dest - m_tr.position, EPS));
			}
			m_cor = null;
			yield break;
		}

		private Vector3 ElMul(Vector3 a, Vector3 b) {
			return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		private bool ElEpsGr(Vector3 v, float eps) {
			return Mathf.Abs(v.x) > eps || Mathf.Abs(v.y) > eps || Mathf.Abs(v.z) > eps;
		}
    }
}
