using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProjectATLAS.Input;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class RocketController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    public Transform startPoint;
    public TextMeshProUGUI infoText;

    [Header("Fuel Bar (UI Image)")]
    public Image fuelBar; // 🔋 UI Image (fill type = horizontal)

    [Header("Control Buttons")]
    public InputButton leftButton;
    public InputButton rightButton;
    public InputButton burnerButton;
    public InputButton ventButton;

    [Header("Rocket Settings")]
    public float moveSpeed = 25f;
    public float thrustForce = 20f;
    public float ventForce = -12f;
    public float horizontalAcceleration = 10f;

    [Header("Temperature Settings")]
    public float temperature = 100f;
    public float minTemp = 80f;
    public float maxTemp = 150f;
    public float heatRate = 20f;
    public float coolRate = 10f;

    [Header("Fuel Settings")]
    public float fuel = 100f;
    public float maxFuel = 100f;
    public float fuelConsumptionRate = 5f;
    public float fuelRefillAmount = 50f;

    private bool isResetting = false;

    private void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        // auto create start point if missing
        if (startPoint == null)
        {
            GameObject sp = new GameObject("StartPoint");
            sp.transform.position = transform.position;
            startPoint = sp.transform;
        }

        UpdateDisplay();
        UpdateFuelBar();
    }

    private void Update()
    {
        if (isResetting) return;

        HandleMovement();
        UpdateDisplay();
        UpdateFuelBar();

        // ⚠️ Auto reset if fuel is empty
        if (fuel <= 0 && !isResetting)
        {
            StartCoroutine(ResetRocket());
        }
    }

    private void HandleMovement()
    {
        float moveInput = 0f;
        if (leftButton != null && leftButton.Value > 0) moveInput -= 1f;
        if (rightButton != null && rightButton.Value > 0) moveInput += 1f;

        float targetVelocityX = moveInput * moveSpeed;
        rb.linearVelocity = new Vector2(
            Mathf.Lerp(rb.linearVelocity.x, targetVelocityX, Time.deltaTime * horizontalAcceleration),
            rb.linearVelocity.y
        );

        // 🔥 Burner (faster upward thrust)
        if (burnerButton != null && burnerButton.Value > 0 && fuel > 0)
        {
            rb.AddForce(Vector2.up * thrustForce * 2.5f, ForceMode2D.Force);
            temperature += heatRate * Time.deltaTime;
            fuel -= fuelConsumptionRate * Time.deltaTime;
        }

        // 🧊 Vent (cool down and push downward)
        if (ventButton != null && ventButton.Value > 0)
        {
            rb.AddForce(Vector2.up * ventForce, ForceMode2D.Force);
            temperature -= coolRate * Time.deltaTime;
        }

        // passive cooling
        if (burnerButton == null || burnerButton.Value == 0)
            temperature -= coolRate * 0.3f * Time.deltaTime;

        // auto descend if too cold
        if (temperature <= 120f)
            rb.AddForce(Vector2.up * ventForce * 1.5f, ForceMode2D.Force);

        temperature = Mathf.Clamp(temperature, minTemp - 10f, maxTemp + 10f);
        fuel = Mathf.Clamp(fuel, 0f, maxFuel);
    }

    private void UpdateDisplay()
    {
        if (infoText == null) return;

        string display = $"Temperature: {temperature:F1}°C\nFuel: {fuel:F0}%\n";

        if (fuel <= 0)
            display += "Out of fuel!";
        else if (temperature >= 150f)
            display += "WARNING: Hot Air Balloon is overheating!";
        else if (temperature <= 120f)
            display += "Temperature is too low!";

        infoText.text = display;
    }

    private void UpdateFuelBar()
    {
        if (fuelBar == null) return;

        float fuelPercent = fuel / maxFuel;
        fuelBar.fillAmount = fuelPercent;

        // 🎨 Change color based on fuel level
        if (fuelPercent > 0.3f)
            fuelBar.color = Color.green;
        else if (fuelPercent > 0.1f)
            fuelBar.color = Color.yellow;
        else
            fuelBar.color = Color.red;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 🟢 Fuel refill pickup
        if (other.CompareTag("FuelPickup"))
        {
            fuel = Mathf.Clamp(fuel + fuelRefillAmount, 0f, maxFuel);
            Destroy(other.gameObject);
        }

        // ⚪ White goal (reset)
        if (other.CompareTag("WhiteGoal"))
        {
            StartCoroutine(ResetRocket());
        }
    }

    private System.Collections.IEnumerator ResetRocket()
    {
        isResetting = true;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(1f);

        transform.position = startPoint.position;
        rb.linearVelocity = Vector2.zero;
        temperature = 100f;
        fuel = 100f;

        isResetting = false;
        UpdateDisplay();
        UpdateFuelBar();
    }
}
