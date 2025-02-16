using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DisconnectProtocol
{
	public class ComplexDoor : IDoor
    {
		public event System.Action<IDoor.State> StateChanged;
        private List<IDoor> m_doors;
		private byte m_count = 0;

		private ComplexDoor() {}
		public ComplexDoor(IEnumerable<IDoor> doors) {
			m_doors = new List<IDoor>(doors.Count());
			foreach (var d in doors) {
				m_doors.Add(d);
				d.StateChanged += OnStateChanged;
			}
		}

		private void OnStateChanged(IDoor.State state) {
			if (++m_count == m_doors.Count) {
				m_count = 0;
				StateChanged?.Invoke(state);
			}
		}

		public void Open(bool isInterrupt = false) {
			m_doors.ForEach(d => d.Open(isInterrupt));
		}

		public void Close(bool isInterrupt = false) {
			m_doors.ForEach(d => d.Close(isInterrupt));
		}

		public void Toggle(bool isInterrupt = false) {
			m_doors.ForEach(d => d.Toggle(isInterrupt));
		}
	}
}
