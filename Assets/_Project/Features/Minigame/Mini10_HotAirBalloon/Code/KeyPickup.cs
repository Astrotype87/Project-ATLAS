using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("hab"))
        {
            FindObjectOfType<MiniGameManager>().CollectKey();
            Destroy(gameObject);
        }
    }
}
