using UnityEngine;

namespace DisconnectProtocol
{
    public interface IStoppable {
		public void Stop();
		public event System.Action Stopped;
    }
}
