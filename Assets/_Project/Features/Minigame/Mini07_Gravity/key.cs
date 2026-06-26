using UnityEngine;

public class KeyItem : MonoBehaviour
{
    [System.Obsolete]
    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<MonoBehaviourPlayerController>();
        if (player != null)
        {
            player.CollectKey();
            Debug.Log("Key collected!");

            // Display UI message
            UIMessageManager.Instance.ShowMessage("You collected a key!");

            Destroy(gameObject);
        }
    }
}

