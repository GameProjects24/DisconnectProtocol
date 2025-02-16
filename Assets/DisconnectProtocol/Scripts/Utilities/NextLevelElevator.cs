using UnityEngine;
using System.Collections.Generic;
using SFDoor = DisconnectProtocol.SlideForwardDoor;
using UnityEngine.SceneManagement;

namespace DisconnectProtocol
{
	[RequireComponent(typeof(Collider))]
    public class NextLevelElevator : MonoBehaviour
    {
		[SerializeField] private Interactable m_toggle;
		private SFDoor m_door;

		private Collider m_col;
		private List<Transform> m_passengers = new List<Transform>();

		public string nextScene;

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
			var place = GameObject.FindGameObjectWithTag("ElevatorNextLevel");
			if (place) {
				transform.position = place.transform.position;
				transform.rotation = place.transform.rotation;
				place.SetActive(false);
			}

			if (m_door) {
				m_door.Open();
				foreach (var tr in m_passengers) {
					tr.SetParent(null, true);
				}
			}
		}

		private void Awake() {
			m_col = GetComponent<Collider>();
			m_col.isTrigger = true;
		}

		private void Start() {
			if (m_toggle == null) {
				m_toggle = GetComponentInChildren<Interactable>();
			}
			m_door = GetComponentInChildren<SFDoor>();
			m_door.StateChanged += OnDoorStateChanged;
		}

		private void OnEnable() {
			if (m_toggle) {
				m_toggle.Interacted += OnToggled;
			}
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		private void OnDisable() {
			if (m_toggle) {
				m_toggle.Interacted -= OnToggled;
			}
			SceneManager.sceneLoaded -= OnSceneLoaded;
		}

		private void OnToggled() {
			m_door.Toggle();
		}

		private void OnDoorStateChanged(SFDoor.State state) {
			if (state == SFDoor.State.Close) {
				foreach (var tr in m_passengers) {
					tr.SetParent(transform, true);
				}
				SceneLoader.Load(this, nextScene);
			}
		}

		private void OnTriggerEnter(Collider other) {
			m_passengers.Add(other.transform);
		}

		private void OnTriggerExit(Collider other) {
			m_passengers.Remove(other.transform);
		}
	}
}
