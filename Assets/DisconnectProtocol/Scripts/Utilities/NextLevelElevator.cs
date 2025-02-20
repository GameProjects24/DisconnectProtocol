using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace DisconnectProtocol
{
	[RequireComponent(typeof(Collider))]
    public class NextLevelElevator : MonoBehaviour
    {
		[SerializeField] private Interactable m_toggle;
		public string nextScene;

		private ComplexDoor m_door;
		private Collider m_col;
		private List<Transform> m_passengers = new List<Transform>();
		private Transform m_tr;


		private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
			var place = GameObject.FindGameObjectWithTag("ElevatorNextLevel");
			if (place) {
				place.SetActive(false);
				var placet = place.transform;

				m_tr.SetPositionAndRotation(placet.position, placet.rotation);
			}

			m_door?.Open();
		}

		private void Awake() {
			if (m_tr == null) {
				m_tr = transform;
			}
			if (m_col == null) {
				m_col = GetComponent<Collider>();
				m_col.isTrigger = true;
			}
		}

		private void Start() {
			if (m_toggle == null) {
				m_toggle = GetComponentInChildren<Interactable>();
				m_toggle.Interacted += OnToggled;
			}
			if (m_door == null) {
				m_door = new ComplexDoor(GetComponentsInChildren<IDoor>());
				m_door.StateChanged += OnDoorStateChanged;
			}
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

		private void OnDoorStateChanged(IDoor.State state) {
			if (state == IDoor.State.Close) {
				SceneLoader.Load(this, nextScene);
			}
		}

		private void OnTriggerEnter(Collider other) {
			other.transform.SetParent(m_tr, true);
			m_passengers.Add(other.transform);
		}

		private void OnTriggerExit(Collider other) {
			other.transform.SetParent(null, true);
			m_passengers.Remove(other.transform);
		}
	}
}
