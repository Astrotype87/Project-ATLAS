using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;

public class PlayUI : MonoBehaviour
{
    [Header("UI Sound Clip")]
    [SerializeField] private AudioClip uiClip;
    [SerializeField, Self] private Button button;
    
    private void Start()
    {
        if (button)
        {
            button.onClick.AddListener(() => {
                Play();
            });
        }
    }
    
    private void OnValidate()
    {
        this.ValidateRefs();
    }
    
    public void Play()
    {
        if (uiClip != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUI(uiClip);
        }
    }
}
