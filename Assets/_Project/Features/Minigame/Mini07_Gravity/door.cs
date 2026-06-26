using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public Animator animator;
    public bool isOpen = false;
    public bool destroyOnOpen = false;
    public GameObject padlock;

    private Collider2D doorCollider;

    void Awake()
    {
        doorCollider = GetComponent<Collider2D>();
        if (doorCollider == null)
            Debug.LogWarning("⚠️ DoorController needs a Collider2D!");

        if (doorCollider != null)
            doorCollider.isTrigger = false;
    }

    [System.Obsolete]
    void OnCollisionEnter2D(Collision2D collision)
    {
        var player = collision.collider.GetComponent<MonoBehaviourPlayerController>();
        if (player == null) return;


        // 🚫 Wala pang key — harangin at ipakita message
        if (!player.hasKey)
        {
            Debug.Log("The door is locked. Find the key first!");
            UIMessageManager.Instance.ShowMessage("The door is locked. Find the key first!");
        }
        else if (!isOpen)
        {
            // ✅ May key — open door
            player.UseKey();
            UnlockPadlock();
            OpenDoor();

            UIMessageManager.Instance.ShowMessage("Door unlocked!");
        }
    }

    void UnlockPadlock()
    {
        if (padlock != null)
        {
            Debug.Log("Padlock unlocked!");
            Destroy(padlock);
        }
    }

    void OpenDoor()
    {
        isOpen = true;
        Debug.Log("Door opened!");

        if (animator != null)
            animator.SetTrigger("Open");

        if (doorCollider != null)
            doorCollider.enabled = false;

        if (destroyOnOpen)
            Destroy(gameObject, 0.5f);
    }
}
