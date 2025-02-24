using UnityEngine;

namespace DisconnectProtocol
{
    public static class ExtensionMethods {
        public static T GetComponentWherever<T>(this GameObject obj) {
			var res = obj.GetComponentInParent<T>();
			if (res != null) {
				return res;
			}
			return obj.GetComponentInChildren<T>();		
		}
    }
}
