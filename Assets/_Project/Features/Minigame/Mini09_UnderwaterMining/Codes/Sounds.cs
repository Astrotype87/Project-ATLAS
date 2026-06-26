using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusic : MonoBehaviour
{
    [Header("🎵 Background Music Settings")]
    public AudioClip musicClip;
    [Range(0f, 1f)] public float musicVolume = 0.5f;   // ✅ renamed to avoid conflict

    private AudioSource _audioSource;

    private void Awake()
    {
        // Keep this GameObject alive between scenes
        DontDestroyOnLoad(gameObject);

        // Get or add AudioSource
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configure the AudioSource
        _audioSource.loop = true;
        _audioSource.playOnAwake = false;
        _audioSource.volume = Mathf.Clamp01(musicVolume);
        _audioSource.spatialBlend = 0f; // 2D sound
    }

    private void Start()
    {
        if (_audioSource == null)
        {
            Debug.LogError("❌ AudioSource is missing.");
            return;
        }

        if (musicClip != null)
        {
            _audioSource.clip = musicClip;
            _audioSource.Play();
            Debug.Log("🎧 Background music started!");
        }
        else
        {
            Debug.LogWarning("⚠️ No music clip assigned in the Inspector!");
        }
    }

    // Optional controls (UI buttons can use these)
    public void PlayMusic()
    {
        if (_audioSource != null && !_audioSource.isPlaying)
            _audioSource.Play();
    }

    public void StopMusic()
    {
        if (_audioSource != null && _audioSource.isPlaying)
            _audioSource.Stop();
    }

    public void SetMusicVolume(float newVolume)
    {
        musicVolume = Mathf.Clamp01(newVolume);
        if (_audioSource != null)
            _audioSource.volume = musicVolume;
    }
}
