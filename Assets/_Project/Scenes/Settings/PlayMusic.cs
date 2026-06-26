using UnityEngine;

public class PlayMusic : MonoBehaviour
{
    [Header("Music Clip")]
    public AudioClip musicClip;

    private void Start()
    {
        Play();
    }

    public void Play()
    {
        if (musicClip != null && AudioManager.Instance != null)
        {
            // This will always override existing music
            AudioManager.Instance.PlayMusic(musicClip);
        }
    }

    public void Stop()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopMusic();
    }
}
