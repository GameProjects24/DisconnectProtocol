using UnityEngine;

namespace DisconnectProtocol
{
    public class DeathState : GameState
    {
        public GameObject deathScreen;

        public override void OnEnter()
        {
            base.OnEnter();
            Time.timeScale = 0f;

            MusicManager.Instance.FadeOutAndPause(0f);

            if (deathScreen != null)
                deathScreen.SetActive(true);
        }

        public override void OnExit()
        {
            if (deathScreen != null)
                deathScreen.SetActive(false);

            MusicManager.Instance.ResumeAndFadeIn(0.5f);

            Time.timeScale = 1f;

            base.OnExit();
        }
    }
}
