using UnityEngine;

public class ChangePlayer : MonoBehaviour
{
    public PlayerController playerController;
    public Player player1;
    public Player player2;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            if (player == player1)
            {
                playerController.SetTarget(player2);
            }
            else
            {
                playerController.SetTarget(player1);
            }
        }
    }
}
