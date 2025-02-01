using UnityEngine;
using System.Collections.Generic;

namespace DisconnectProtocol
{
    public class ObjectsToObject : MonoBehaviour
    {
		public Transform target;
		public string[] tags;
		private List<Transform> objects = new List<Transform>();

		private void Start() {
			foreach (var t in tags) {
				foreach (var go in GameObject.FindGameObjectsWithTag(t)) {
					objects.Add(go.transform);
				}
			}
		}

        private void Update() {
			var cp = target.position;
			var cpl = cp;
			objects.RemoveAll(o => o == null);
			foreach (var obj in objects) {
				cpl.y = obj.position.y;
				obj.LookAt(cpl);
			}
        }
    }
}
