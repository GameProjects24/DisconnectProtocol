using System.IO;
using UnityEngine;

namespace DisconnectProtocol
{
    public static class FileSaver {
		public static void Save(string path, IData data) {
			File.WriteAllText(path, data.ToFormat());
		}

		public static void Load(string path, IData data) {
			if (!File.Exists(path)) {
				return;
			}
			data.FromFormat(File.ReadAllText(path));
		}
    }
}