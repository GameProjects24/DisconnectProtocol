using Newtonsoft.Json;
using UnityEngine;

namespace DisconnectProtocol
{
	[System.Serializable]
	public class PlayerData : IData {
		public string lastLevel;
		public Inventory.InventoryData inventory;
		public LocationData location;

		public void SetLocation(Transform obj) {
			location.pos = obj.position;
			location.rot = obj.rotation;
		}

		public void TryLoadLocation(string scene, ref Transform obj) {
			if (scene != lastLevel) {
				return;
			}
			obj.SetPositionAndRotation(location.pos, location.rot);
		}

		public string ToFormat() {
			return JsonUtility.ToJson(this);
		}

		public void FromFormat(string data) {
			JsonUtility.FromJsonOverwrite(data, this);
		}
	}

	[System.Serializable]
	public class LocationData {
		public Vector3 pos;
		public Quaternion rot;

		public static LocationData FromTransform(Transform tr) {
			var ld = new LocationData();
			ld.pos = tr.position;
			ld.rot = tr.rotation;
			return ld;
		}

		public void ToTransform(ref Transform tr) {
			tr.SetPositionAndRotation(pos, rot);
		}
	}
}