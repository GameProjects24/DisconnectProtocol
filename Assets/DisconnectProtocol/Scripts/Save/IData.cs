using UnityEngine;

namespace DisconnectProtocol
{
    public interface IData {
        string ToFormat();
		void FromFormat(string data);
    }
}