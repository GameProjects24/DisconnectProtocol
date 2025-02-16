using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DisconnectProtocol
{
    public static class SceneLoader
    {
		public static void Load(MonoBehaviour ctrl, string scene) {
			ctrl.StartCoroutine(LoadAsync(scene));
		}

		private static IEnumerator LoadAsync(string sc) {
			var op = SceneManager.LoadSceneAsync(sc);
			GC.Collect();
			yield return new WaitUntil(() => op.isDone);
		}
    }
}
