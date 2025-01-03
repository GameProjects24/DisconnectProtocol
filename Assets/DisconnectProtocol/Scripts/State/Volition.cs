using UnityEngine;
using UnityEngine.AI;

namespace State {
    public class Volition : MonoBehaviour {
        public Machine machine;

        private void Update() {
            machine.Execute();
        }
    }
}