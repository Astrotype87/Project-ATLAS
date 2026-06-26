using UnityEngine;
using System.Collections;
using System;

public class BoxGoalTrigger : MonoBehaviour
{
    [Header("Goal Settings")]
    public Color goalActiveColor = Color.green;
    public float magnetStrength = 20f;
    public float lockDuration = 0.3f;

    [Header("Audio")]
    public AudioClip goalSound;
    private AudioSource audioSource;

    // Event to notify when a goal is collected
    public static event Action OnGoalCollected;

    private SpriteRenderer goalRenderer;
    private Color originalColor;
    private bool isActivated = false;

    void Start()
    {
        goalRenderer = GetComponent<SpriteRenderer>();
        if (goalRenderer != null)
            originalColor = goalRenderer.color;

        // Audio setup
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isActivated) return;

        if (other.CompareTag("Box"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
                StartCoroutine(LockAndActivateGoal(rb));
        }
    }

    IEnumerator LockAndActivateGoal(Rigidbody2D rb)
    {
        isActivated = true;

        Vector2 target = transform.position;
        float timer = 0f;

        // Pull box to center
        while (timer < lockDuration)
        {
            rb.MovePosition(Vector2.Lerp(rb.position, target, Time.deltaTime * magnetStrength));
            timer += Time.deltaTime;
            yield return null;
        }

        // Disable physics
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Change color
        if (goalRenderer != null)
            goalRenderer.color = goalActiveColor;

        // Notify that a goal is collected
        OnGoalCollected?.Invoke();

        PlaySound(goalSound); // Play goal sound

        // Disable the box so it can't be collected again
        GetComponent<Collider2D>().enabled = false; // Disable trigger
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}