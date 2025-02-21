using UnityEngine;

namespace DisconnectProtocol
{
	[System.Serializable]
	public class PlayerData : IData {
		public string lastLevel;
		public bool isValid = false;

		public string ToFormat() {
			return JsonUtility.ToJson(this);
		}

		public void FromFormat(string data) {
			JsonUtility.FromJsonOverwrite(data, this);
		}
	}
}