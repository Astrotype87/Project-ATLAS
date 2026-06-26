using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using ProjectATLAS.Input; // ✅ for InputButton

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class BalloonControllerSS : MonoBehaviour
{
    [Header("Physics & Lift Settings")]
    public Rigidbody2D rb;
    public float ambientTemp = 20f;
    public float temperature = 130f;
    public float liftCoefficient = 0.25f; 
    public float coolingRate = 2f;

    [Header("Temperature Limits")]
    public float safeMinTemp = 120f;
    public float safeMaxTemp = 150f;
    public float burstTemp = 220f;

    [Header("Fuel Settings")]
    [Range(0, 100)] public float fuel = 100f;
    public float heatPerBurn = 40f;
    public float fuelPerBurn = 8f;
    public float ventCooling = 25f;

    [Header("UI References")]
    public TMP_Text tempText;
    public TMP_Text statusText;
    public Image fuelBarFill;

    [Header("Movement Controls")]
    public float horizontalForce = 15f;
    public float moveSmoothness = 8f;
    public InputButton leftButton;
    public InputButton rightButton;
    public InputButton burnButton;
    public InputButton ventButton;

    [Header("Landing")]
    public float safeLandingSpeed = 2.5f;

    private bool gameOver = false;
    private float targetMoveX = 0f;
    private float currentMoveX = 0f;
    private Vector3 startPosition;

    void Start()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 1f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        rb.freezeRotation = true;

        // Save starting point for reset
        startPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (gameOver) return;

        HandleControls();
        HandlePhysics();
        CheckWarningsAndReset();
        UpdateUI();
    }

    private void HandleControls()
    {
        bool moveLeft = leftButton && leftButton.Value == 1;
        bool moveRight = rightButton && rightButton.Value == 1;

        if (moveLeft) targetMoveX = -1f;
        else if (moveRight) targetMoveX = 1f;
        else targetMoveX = 0f;

        currentMoveX = Mathf.Lerp(currentMoveX, targetMoveX, moveSmoothness * Time.fixedDeltaTime);

        if (Mathf.Abs(currentMoveX) > 0.05f)
            rb.AddForce(Vector2.right * currentMoveX * horizontalForce, ForceMode2D.Force);

        // 🔥 BURNER
        if (burnButton && burnButton.Value == 1 && fuel > 0f)
        {
            temperature += heatPerBurn * Time.fixedDeltaTime;
            fuel -= fuelPerBurn * Time.fixedDeltaTime;
            rb.AddForce(Vector2.up * 20f * Time.fixedDeltaTime, ForceMode2D.Impulse);
        }

        // 🌬️ VENT
        if (ventButton && ventButton.Value == 1)
        {
            temperature -= ventCooling * Time.fixedDeltaTime;
            rb.AddForce(Vector2.down * 5f * Time.fixedDeltaTime, ForceMode2D.Force);
        }
    }

    private void HandlePhysics()
    {
        // Cooling
        temperature -= coolingRate * Time.fixedDeltaTime;
        if (temperature < ambientTemp)
            temperature = ambientTemp;

        // Passive lift
        float tempDiff = Mathf.Max(0f, temperature - ambientTemp);
        float liftForce = liftCoefficient * tempDiff;

        if (temperature >= safeMinTemp)
            rb.AddForce(Vector2.up * liftForce * Time.fixedDeltaTime, ForceMode2D.Force);
    }

    private void CheckWarningsAndReset()
    {
        // Overheating warning
        if (temperature >= safeMaxTemp)
        {
            statusText.text = " WARNING: Balloon is overheating";
            StartCoroutine(ResetAfterDelay());
        }
        // Too cold warning
        else if (temperature <= safeMinTemp)
        {
            statusText.text = "Your temperature is too low";
            StartCoroutine(ResetAfterDelay());
        }
        // Out of fuel
        else if (fuel <= 0)
        {
            statusText.text = " Out of fuel";
            StartCoroutine(ResetAfterDelay());
        }
    }

    private IEnumerator ResetAfterDelay()
    {
        gameOver = true;
        yield return new WaitForSeconds(1.5f);

        // Reset all values
        transform.position = startPosition;
        rb.linearVelocity = Vector2.zero;
        temperature = 130f;
        fuel = 100f;
        statusText.text = "";
        gameOver = false;
    }

    private void UpdateUI()
    {
        if (tempText)
            tempText.text = $"{Mathf.RoundToInt(temperature)} °C";

        if (fuelBarFill)
            fuelBarFill.fillAmount = Mathf.Clamp01(fuel / 100f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameOver) return;

        if (other.CompareTag("LandingPad"))
        {
            float verticalSpeed = Mathf.Abs(rb.linearVelocity.y);
            if (verticalSpeed <= safeLandingSpeed)
                statusText.text = "Landed safely!";
            else
                statusText.text = " Hard landing!";
        }
    }
}
