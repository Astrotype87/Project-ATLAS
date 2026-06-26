using UnityEngine;

public class deDoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public Animator animator;
    public bool isOpen = false;
    public bool destroyOnOpen = false;
    public GameObject padlock;

    [Header("Sound Settings")]
    public AudioClip unlockClip; // Sound kapag nag-open
    private AudioSource audioSource;

    private Collider2D doorCollider;

    void Awake()
    {
        // Ensure may collider
        doorCollider = GetComponent<Collider2D>();
        if (doorCollider == null)
            Debug.LogWarning("⚠️ DoorController needs a Collider2D!");
        else
            doorCollider.isTrigger = false;

        // Ensure may AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void OpenDoor()
    {
        if (isOpen) return;

        isOpen = true;
        Debug.Log("🚪 Door opened!");

        // Optional animation
        if (animator != null)
            animator.SetTrigger("Open");

        // Disable collider para makadaan si player
        if (doorCollider != null)
            doorCollider.enabled = false;

        // Play sound effect kung meron
        if (unlockClip != null)
            audioSource.PlayOneShot(unlockClip);

        // Optional destroy
        if (destroyOnOpen)
            Destroy(gameObject, 0.5f);
    }
}
