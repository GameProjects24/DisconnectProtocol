using UnityEngine;

namespace DisconnectProtocol
{
    public class DeathState : GameState
    {
        public GameObject deathScreen;   // Ссылка на экран смерти (с анимациями и кнопками)

        public override void OnEnter()
        {
            base.OnEnter();
            // Остановка времени для геймплея (но анимации на DeathScreen могут использовать unscaled time)
            Time.timeScale = 0f;

            if (deathScreen != null)
                deathScreen.SetActive(true);
        }

        public override void OnExit()
        {
            if (deathScreen != null)
                deathScreen.SetActive(false);

            // При выходе из состояния смерти можно восстановить время
            Time.timeScale = 1f;

            base.OnExit();
        }
    }
}
