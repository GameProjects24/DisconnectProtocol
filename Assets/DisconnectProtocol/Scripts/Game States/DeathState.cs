using UnityEngine;

namespace DisconnectProtocol
{
    public class DeathState : MonoBehaviour
    {
        public GameObject deathScreen;
        private void OnEnable()
        {
            deathScreen.SetActive(true);
        }

        private void OnDisable()
        {
            deathScreen.SetActive(false);
        }
    }
}
