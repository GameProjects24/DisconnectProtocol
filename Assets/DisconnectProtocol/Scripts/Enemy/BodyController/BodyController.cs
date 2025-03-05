using System.Collections.Generic;
using UnityEngine;

namespace DisconnectProtocol
{
	public class BodyController<TState, TAction> : MonoBehaviour
	where TState : System.Enum
	where TAction : System.Enum
	{
		protected struct FlowState {
			public TState start;
			public TState pause;
			public TState resume;
			public TState stop;
			public FlowState(TState start, TState pause, TState resume, TState stop) {
				this.start = start;
				this.pause = pause;
				this.resume = resume;
				this.stop = stop;
			}
			public FlowState(TState start, TState stop) {
				this.start = start;
				this.pause = default;
				this.resume = default;
				this.stop = stop;
			}
		}
		protected Dictionary<IStoppable, FlowState> m_states = new Dictionary<IStoppable, FlowState>();
		protected TState m_curState;
		protected IStoppable m_curStoppable;

		public event System.Action<TState> StateChanged;
		public event System.Action<TAction> ActionDone;

		protected void ChangeState(TState state) {
			if (EqualityComparer<TState>.Default.Equals(m_curState, state)) {
				return;
			}
			m_curState = state;
			StateChanged?.Invoke(state);
		}

		protected void ChangeStoppable(IStoppable stp) {
			if (m_curStoppable == stp) {
				return;
			}
			// may involve OnStateStop,
			m_curStoppable?.Stop();
			// so order matters
			if (stp != null && m_states.TryGetValue(stp, out var sts)) {
				ChangeState(sts.start);
			}
			m_curStoppable = stp;
		}

		protected void OnStateStop() {
			if (m_states.TryGetValue(m_curStoppable, out var sts)) {
				ChangeState(sts.stop);
			}
			m_curStoppable = null;
		}

		protected void OnStatePause() {
			if (m_states.TryGetValue(m_curStoppable, out var sts)) {
				ChangeState(sts.pause);
			}
		}

		protected void OnStateResume() {
			if (m_states.TryGetValue(m_curStoppable, out var sts)) {
				ChangeState(sts.resume);
			}
		}

		protected void OnActionDone(TAction action) {
			ActionDone?.Invoke(action);
		}
    }
}