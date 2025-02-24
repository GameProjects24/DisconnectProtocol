using System.IO;
using UnityEngine;

namespace DisconnectProtocol
{
    public class GameController : MonoBehaviour {
		private static GameController m_instance;
		public static GameController instance {
			get {
				if (m_instance == null) {
					m_instance = FindAnyObjectByType<GameController>();
				}
				return m_instance;
			}
		}
		public PlayerData pd { get; private set; } = new PlayerData();
		private string m_pdPath;

		private void Awake() {
			if (m_instance == null) {
				m_instance = this;
				m_pdPath = Path.Combine(Application.persistentDataPath, "playerData");
			}
			if (m_instance != this) {
				Destroy(gameObject);
				return;
			}

			DontDestroyOnLoad(gameObject);
			LoadPlayerData();
		}

		private void OnApplicationQuit() {
			SavePlayerData();
		}

		/// <summary>
		/// Tries to load last saved level, otherwise load and saves fallback
		/// </summary>
		/// <param name="fallbackLevel">Level to load if there's no saved</param>
		public void LoadLevel(string fallbackLevel) {
			if (pd.isValid) {
				SceneLoader.Load(this, pd.lastLevel);
			} else {
				ChangeLevel(fallbackLevel);
			}
		}

		/// <summary>
		/// Change scene and save as last visited level
		/// </summary>
		/// <param name="level"></param>
		public void ChangeLevel(string level) {
			pd.lastLevel = level;
			pd.isValid = true;
			SceneLoader.Load(this, level);
		}

		/// <summary>
		/// Just change scene
		/// </summary>
		/// <param name="scene"></param>
		public void ChangeScene(string scene) {
			SceneLoader.Load(this, scene);
		}

		public void SavePlayerData() {
			FileSaver.Save(m_pdPath, pd);
		}

		public void LoadPlayerData() {
			FileSaver.Load(m_pdPath, pd);
		}
    }
}