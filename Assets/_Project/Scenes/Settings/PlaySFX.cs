using UnityEngine;
using UnityEngine.UI;

public class PlaySFX : MonoBehaviour
{
    [Header("SFX Clip to Play")]
    public AudioClip clip;

    private void Start()
    {
        // Optional: auto-bind if attached to a button
        Button btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(PlaySound);
    }

    public void PlaySound()
    {
        if (clip != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(clip);
    }
}
