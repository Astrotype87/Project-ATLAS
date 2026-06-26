using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("hab"))
        {
            MiniGameManager gm = FindObjectOfType<MiniGameManager>();
            if (gm.hasKey)
            {
                gm.Win();
            }
            else
            {
                gm.GameOver();
            }
        }
    }
}
