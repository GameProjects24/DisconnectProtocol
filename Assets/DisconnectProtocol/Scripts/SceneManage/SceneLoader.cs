using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DisconnectProtocol
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private string m_scene;
		public GameObject elev;

		private void Update() {
			if (Input.GetKeyDown(KeyCode.P)) {
				Load();
				elev.transform.position += Vector3.up * 100f;
			}
		}

		public void Load() {
			Debug.Log($"Load scene -- {m_scene}");
			StartCoroutine(LoadAsync());
		}

		private IEnumerator LoadAsync() {
			SceneManager.LoadSceneAsync(m_scene);
			var scene = SceneManager.GetSceneByName(m_scene);
			yield return new WaitUntil(() => scene.isLoaded);
		}
    }
}
