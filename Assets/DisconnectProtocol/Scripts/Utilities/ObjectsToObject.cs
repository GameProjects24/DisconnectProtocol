using Unity.VisualScripting;
using UnityEngine;

namespace DisconnectProtocol
{
    public class ObjectsToObject : MonoBehaviour
    {
		public Transform target;
		public Transform[] objects;

        private void Update() {
			var cp = target.position;
			var cpl = cp;
			foreach (var obj in objects) {
				cpl.y = obj.position.y;
				obj.LookAt(cpl);
			}
        }
    }
}
