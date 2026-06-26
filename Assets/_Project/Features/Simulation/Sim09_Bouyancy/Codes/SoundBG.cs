using UnityEngine;

public class BackgroundMusic09 : MonoBehaviour
{
    // Optional singleton so only one music object exists across scenes
    public static BackgroundMusic09 Instance;

    [Header("Background Music")]
    public AudioClip backgroundClip;                 // assign in Inspector
    [Range(0f, 1f)] public float volume = 0.5f;     // default volume

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton: keep the first instance, destroy duplicates
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        // Get or add an AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Configure the audio source
        audioSource.clip = backgroundClip;
        audioSource.loop = true;
        audioSource.volume = Mathf.Clamp01(volume);
        audioSource.playOnAwake = false; // we will control playback in Start()
    }

    void Start()
    {
        // Play automatically if clip assigned
        if (audioSource.clip != null)
            audioSource.Play();
    }

    // Public controls (call from UI buttons or other scripts)
    public void PlayMusic()
    {
        if (audioSource != null && !audioSource.isPlaying)
            audioSource.Play();
    }

    public void PauseMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Pause();
    }

    public void StopMusic()
    {
        if (audioSource != null)
            audioSource.Stop();
    }

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (audioSource != null)
            audioSource.volume = volume;
    }
}
