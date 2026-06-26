using UnityEngine;

namespace ProjectATLAS.Minigame.Mini09_UnderwaterMining
{
    public class Crystal : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")) // make sure submarine has tag "Player"
            {
                // Find the GameManager in the scene
                GameManager gm = FindObjectOfType<GameManager>();
                if (gm != null)
                    gm.CollectCrystal();

                Destroy(gameObject); // remove crystal after collecting
            }
        }
    }
}
