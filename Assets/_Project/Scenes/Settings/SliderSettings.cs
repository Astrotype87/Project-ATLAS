using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderSettings : MonoBehaviour
{
    [Header("Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider uiSlider;

    [Header("Percentage Texts")]
    public TMP_Text masterText;
    public TMP_Text musicText;
    public TMP_Text sfxText;
    public TMP_Text uiText;

    [Header("Toggle Buttons (Image Swap)")]
    public Button masterToggle;
    public Button musicToggle;
    public Button sfxToggle;
    public Button uiToggle;

    [Header("On/Off Sprites")]
    public Sprite onSprite;
    public Sprite offSprite;

    private bool masterOn = true;
    private bool musicOn = true;
    private bool sfxOn = true;
    private bool uiOn = true;

    private void Start()
    {
        // Load saved volume values or default to 1
        float savedMaster = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 1f);
        float savedUI = PlayerPrefs.GetFloat("UIVolume", 1f);

        masterSlider.value = savedMaster;
        musicSlider.value = savedMusic;
        sfxSlider.value = savedSFX;
        uiSlider.value = savedUI;

        // Apply to AudioManager
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterVolume(savedMaster);
            AudioManager.Instance.SetMusicVolume(savedMusic);
            AudioManager.Instance.SetSFXVolume(savedSFX);
            AudioManager.Instance.SetUIVolume(savedUI);
        }

        // Update UI labels
        UpdateMasterText(savedMaster);
        UpdateMusicText(savedMusic);
        UpdateSFXText(savedSFX);
        UpdateUIText(savedUI);

        // Add listeners
        masterSlider.onValueChanged.AddListener(UpdateMasterText);
        musicSlider.onValueChanged.AddListener(UpdateMusicText);
        sfxSlider.onValueChanged.AddListener(UpdateSFXText);
        uiSlider.onValueChanged.AddListener(UpdateUIText);

        // Add toggle buttons
        masterToggle.onClick.AddListener(ToggleMaster);
        musicToggle.onClick.AddListener(ToggleMusic);
        sfxToggle.onClick.AddListener(ToggleSFX);
        uiToggle.onClick.AddListener(ToggleUI);
    }

    private void UpdateMasterText(float value)
    {
        masterText.text = Mathf.RoundToInt(value * 100) + "%";
        masterOn = value > 0.001f;
        UpdateButtonImage(masterToggle, masterOn);

        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMasterVolume(value);

        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    private void UpdateMusicText(float value)
    {
        musicText.text = Mathf.RoundToInt(value * 100) + "%";
        musicOn = value > 0.001f;
        UpdateButtonImage(musicToggle, musicOn);

        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(value);

        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    private void UpdateSFXText(float value)
    {
        sfxText.text = Mathf.RoundToInt(value * 100) + "%";
        sfxOn = value > 0.001f;
        UpdateButtonImage(sfxToggle, sfxOn);

        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(value);

        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    private void UpdateUIText(float value)
    {
        uiText.text = Mathf.RoundToInt(value * 100) + "%";
        uiOn = value > 0.001f;
        UpdateButtonImage(uiToggle, uiOn);

        if (AudioManager.Instance != null)
            AudioManager.Instance.SetUIVolume(value);

        PlayerPrefs.SetFloat("UIVolume", value);
    }

    // --- Toggles ---
    private void ToggleMaster()
    {
        masterOn = !masterOn;
        masterSlider.value = masterOn ? 1f : 0f;
        UpdateMasterText(masterSlider.value);
    }

    private void ToggleMusic()
    {
        musicOn = !musicOn;
        musicSlider.value = musicOn ? 1f : 0f;
        UpdateMusicText(musicSlider.value);
    }

    private void ToggleSFX()
    {
        sfxOn = !sfxOn;
        sfxSlider.value = sfxOn ? 1f : 0f;
        UpdateSFXText(sfxSlider.value);
    }

    private void ToggleUI()
    {
        uiOn = !uiOn;
        uiSlider.value = uiOn ? 1f : 0f;
        UpdateUIText(uiSlider.value);
    }

    // --- Helper ---
    private void UpdateButtonImage(Button button, bool isOn)
    {
        if (onSprite != null && offSprite != null)
        {
            button.image.sprite = isOn ? onSprite : offSprite;
        }
    }
}
