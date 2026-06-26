using UnityEngine;

namespace ProjectATLAS.Minigame.Mini09_UnderwaterMining
{
    public class Obstacle : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                FindObjectOfType<GameMessageUI>().ShowGameOver();
                GameManager.Instance.DisablePlayer();
            }
        }
    }
}
