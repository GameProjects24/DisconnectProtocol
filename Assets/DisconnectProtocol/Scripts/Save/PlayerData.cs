using Newtonsoft.Json;
using UnityEngine;

namespace DisconnectProtocol
{
	[System.Serializable]
	public class PlayerData : IData {
		public string lastLevel;
		public InventoryData weaponInventory = new InventoryData();
		public LocationData location = new LocationData();

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

		public void SetInventoryData(InventoryData inv) {
			weaponInventory = inv;
		}

		public InventoryData TryGetInventoryData(string scene) {
			if (scene != lastLevel) {
				return null;
			}
			return weaponInventory;
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
	}
}