using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace DisconnectProtocol
{
	[RequireComponent(typeof(Collider))]
    public class NextLevelElevator : MonoBehaviour
    {
		private class Passenger {
			public Transform self;
			public Transform parent;

			public Passenger(Transform self) {
				this.self = self;
				parent = self.parent;
			}

			public void ToParent() {
				self.SetParent(parent, true);
			}

			public void Deconstruct(out Transform self, out Transform parent) {
				self = this.self;
				parent = this.parent;
			}
		}

		[SerializeField] private Interactable m_toggle;
		public string nextLevel;

		private ComplexDoor m_door;
		private Collider m_col;
		private List<Passenger> m_passengers = new List<Passenger>();
		private Transform m_tr;


		private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
			StartCoroutine(ChangePlace());
		}

		private IEnumerator ChangePlace() {
			yield return null;
			var place = GameObject.FindGameObjectWithTag("ElevatorNextLevel");
			if (place) {
				place.SetActive(false);
				var placet = place.transform;

				m_tr.SetPositionAndRotation(placet.position, placet.rotation);
			}

			m_door?.Open();
			foreach (var pas in m_passengers) {
				pas.ToParent();
			}
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
				foreach ((var pas, _) in m_passengers) {
					if (pas != null) {
						pas.SetParent(m_tr, true);
					}
				}
				GameController.instance.ChangeLevel(nextLevel);
			}
		}

		private void OnTriggerEnter(Collider other) {
			var pas = other.transform;
			var whole = other.GetComponentInParent<Whole>();
			if (whole != null) {
				pas = whole.transform;
				var tryp = m_passengers.Find(p => p.self == pas);
				if (tryp != null) {
					return;
				}
			}
			m_passengers.Add(new Passenger(pas));
		}

		private void OnTriggerExit(Collider other) {
			m_passengers.RemoveAll(p => p.self == other.transform);
		}
	}
}
