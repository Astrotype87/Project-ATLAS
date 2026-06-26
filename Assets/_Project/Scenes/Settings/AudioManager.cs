using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer Reference")]
    public AudioMixer audioMixer;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource uiSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null); // Detach from parent
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        // Load saved volumes (default to 1)
        SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume", 1f));
        SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 1f));
        SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 1f));
        SetUIVolume(PlayerPrefs.GetFloat("UIVolume", 1f));
    }

    // --- MUSIC ---
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;

        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource.isPlaying)
            musicSource.Stop();
    }

    // --- SOUND EFFECTS ---
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    // --- UI SOUNDS ---
    public void PlayUI(AudioClip clip)
    {
        if (clip == null) return;
        uiSource.PlayOneShot(clip);
    }

    // --- VOLUME SETTINGS ---
    public void SetMasterVolume(float value)
    {
        SetVolume("Master", value);
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        SetVolume("Music", value);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        SetVolume("SFX", value);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void SetUIVolume(float value)
    {
        SetVolume("UI", value);
        PlayerPrefs.SetFloat("UIVolume", value);
    }

    private void SetVolume(string parameter, float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(parameter, dB);
    }
}
