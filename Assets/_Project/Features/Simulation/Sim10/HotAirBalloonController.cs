using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProjectATLAS.Input;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class HotAirBalloonController_UI : MonoBehaviour
{
    [Header("Input Buttons")]
    public InputButton BurnerButton;
    public InputButton VentButton;

    [Header("Heat System")]
    public Slider HeatBar;
    public float maxTemperature = 150f;
    public float ambientTemperature = 20f;
    public float heatIncreaseRate = 15f;
    public float heatDecreaseRate = 5f;
    public float ventCoolRate = 25f;

    [Header("Flight Physics")]
    public float liftMultiplier = 0.05f;
    public float maxLiftForce = 10f;
    public float stableFallRate = 3f;

    [Header("UI Elements")]
    public TMP_Text InfoText;
    public GameObject NextButton;

    [Header("Sound Effects (Clips)")]
    public AudioClip BGMusic;
    public AudioClip BurnerSFX;
    public AudioClip VentSFX;
    public AudioClip CongratsSFX;

    private Rigidbody2D rb;
    private AudioSource audioSource;
    private float currentTemperature;
    private bool hasShownCongrats = false;
    private bool isBurnerPlaying = false;
    private bool isVentPlaying = false;
    private string congratsMessage = "";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        // Background music start
        if (BGMusic != null)
        {
            audioSource.loop = true;
            audioSource.clip = BGMusic;
            audioSource.Play();
        }

        currentTemperature = ambientTemperature;

        if (HeatBar != null)
        {
            HeatBar.minValue = 0f;
            HeatBar.maxValue = maxTemperature;
            HeatBar.value = currentTemperature;
        }

        if (NextButton != null)
            NextButton.SetActive(false);

        UpdateInfoText("Initializing balloon...");
    }

    void FixedUpdate()
    {
        UpdateTemperature();
        ApplyLift();
        CheckAltitudeGoal();
        UpdateInfoText(congratsMessage);
    }

    void UpdateTemperature()
    {
        bool isBurning = (BurnerButton != null && BurnerButton.Value > 0);
        bool isVenting = (VentButton != null && VentButton.Value > 0);

        // 🔥 Burner heating
        if (isBurning)
        {
            currentTemperature += heatIncreaseRate * Time.fixedDeltaTime;
            PlayBurnerSFX(true);
        }
        else
        {
            PlayBurnerSFX(false);
        }

        // 💨 Vent cooling
        if (isVenting)
        {
            currentTemperature -= ventCoolRate * Time.fixedDeltaTime;
            PlayVentSFX(true);
        }
        else
        {
            PlayVentSFX(false);
        }

        // 🔄 Passive cooling/heating
        if (!isBurning && !isVenting)
        {
            if (currentTemperature > ambientTemperature)
                currentTemperature -= heatDecreaseRate * Time.fixedDeltaTime;
            else if (currentTemperature < ambientTemperature)
                currentTemperature += (heatDecreaseRate * 0.5f) * Time.fixedDeltaTime;
        }

        currentTemperature = Mathf.Clamp(currentTemperature, 0f, maxTemperature);
        if (HeatBar != null) HeatBar.value = currentTemperature;
    }

    void ApplyLift()
    {
        float tempDiff = currentTemperature - ambientTemperature;
        float liftForce = 0f;

        if (tempDiff > 0f)
        {
            liftForce = Mathf.Clamp(tempDiff * liftMultiplier, 0f, maxLiftForce);
            rb.AddForce(Vector2.up * liftForce, ForceMode2D.Force);
        }
        else
        {
            rb.AddForce(Vector2.down * stableFallRate, ForceMode2D.Force);
        }
    }

    void CheckAltitudeGoal()
    {
        float altitude = transform.position.y;

        if (altitude >= 500f && !hasShownCongrats)
        {
            hasShownCongrats = true;
            congratsMessage = "Congratulations! You reached 500 altitude!";

            if (NextButton != null)
                NextButton.SetActive(true);

            if (CongratsSFX != null)
                audioSource.PlayOneShot(CongratsSFX);
        }
    }

    void UpdateInfoText(string extraMessage = "")
    {
        if (InfoText == null) return;

        float tempDiff = currentTemperature - ambientTemperature;
        float lift = Mathf.Clamp(Mathf.Max(0f, tempDiff) * liftMultiplier, 0f, maxLiftForce);
        float vertSpeed = rb.linearVelocity.y;
        float altitude = transform.position.y;

        string status;
        if (BurnerButton != null && BurnerButton.Value > 0) status = "Heating (Burner ON)";
        else if (VentButton != null && VentButton.Value > 0) status = "Venting (Cooling)";
        else if (vertSpeed > 0.2f) status = "⬆️ Rising";
        else if (vertSpeed < -0.2f) status = "⬇️ Descending";
        else status = "⚖️ Stable";

        InfoText.text =
            $"HOT AIR BALLOON STATUS\n" +
            $"---------------------------------\n" +
            $"Inside Temp: {currentTemperature:F1}°C\n" +
            $"Ambient Temp: {ambientTemperature:F1}°C\n" +
            $"ΔTemp: {tempDiff:F1}°C\n" +
            $"Heat Level: {(HeatBar != null ? HeatBar.value / HeatBar.maxValue * 100f : 0f):F0}%\n\n" +
            $"Lift Force: {lift:F2} N\n" +
            $"Vertical Speed: {vertSpeed:F2} m/s\n" +
            $"Altitude: {altitude:F1} m\n" +
            $"---------------------------------\n" +
            $"Status: {status}\n" +
            $"{(extraMessage != "" ? "\n" + extraMessage : "")}";
    }

    void PlayBurnerSFX(bool active)
    {
        if (BurnerSFX == null) return;
        if (active && !isBurnerPlaying)
        {
            audioSource.PlayOneShot(BurnerSFX);
            isBurnerPlaying = true;
        }
        else if (!active)
        {
            isBurnerPlaying = false;
        }
    }

    void PlayVentSFX(bool active)
    {
        if (VentSFX == null) return;
        if (active && !isVentPlaying)
        {
            audioSource.PlayOneShot(VentSFX);
            isVentPlaying = true;
        }
        else if (!active)
        {
            isVentPlaying = false;
        }
    }
}
