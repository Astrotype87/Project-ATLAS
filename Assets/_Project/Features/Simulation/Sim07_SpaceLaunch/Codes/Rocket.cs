using UnityEngine;
using TMPro;
using ProjectATLAS.Input;

public class RocketControllers : MonoBehaviour
{
    [Header("Rocket Settings")]
    public float thrustPower = 10000f;
    public float sideSpeed = 5f;
    public float rocketMass = 1000f;

    [Header("Physics")]
    public float gravity = 9.81f;
    public float airResistanceCoefficient = 0.02f;
    public float escapeVelocity = 11200f;
    public float groundLevel = 0f;  // your ground reference
    
    [Header("Input")]
    public InputSlider thrustSlider;
    
    [Header("Audio")]
    public AudioSource thrustAudioSource;
    
    [Header("TMP UI")]
    public TMP_Text escapeVelocityTMP;
    public TMP_Text gravityTMP;
    public TMP_Text weightForceTMP;
    public TMP_Text airResistanceTMP;
    public TMP_Text altitudeTMP;

    private Rigidbody2D rb;
    private float thrustInput;
    private bool thrusting, movingLeft, movingRight;
    private float currentAirResistance;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.mass = rocketMass;
        rb.gravityScale = 0; // because we apply custom gravity
    }
    
    private void FixedUpdate()
    {
        HandleMovement();
    }
    
    void LateUpdate()
    {
        UpdateUI();
    }
    
    private void HandleMovement()
    {
        thrustInput = thrustSlider.Value;
        Vector2 appliedThrust = this.thrustPower * thrustInput * Vector2.up;
        rb.AddForce(appliedThrust * Time.fixedDeltaTime);
        
        // Audio
        thrustAudioSource.volume = thrustInput;
        thrustAudioSource.pitch = 1f + (thrustInput * 2f);
        
        
        
        // Move left/right
        Vector2 vel = rb.linearVelocity;
        if (movingLeft) vel.x = -sideSpeed;
        else if (movingRight) vel.x = sideSpeed;
        else vel.x = 0;
        rb.linearVelocity = vel;

        // Gravity
        rb.AddForce(Vector2.down * gravity * rocketMass * Time.deltaTime);

        // Air resistance (simple)
        if (rb.linearVelocity.y > 0)
        {
            currentAirResistance = airResistanceCoefficient * rb.linearVelocity.y * rb.linearVelocity.y;
            rb.AddForce(Vector2.down * currentAirResistance * Time.deltaTime);
        }
        else
        {
            currentAirResistance = 0;
        }
    }

    private void UpdateUI()
    {
        float altitude = Mathf.Max(0, transform.position.y - groundLevel);

        if (escapeVelocityTMP) escapeVelocityTMP.text = $"Escape Velocity: {escapeVelocity:F0} m/s";
        if (gravityTMP) gravityTMP.text = $"Gravity: {gravity:F2} m/s²";
        if (weightForceTMP) weightForceTMP.text = $"Weight Force: {(gravity * rocketMass):F0} N";
        if (airResistanceTMP) airResistanceTMP.text = $"Air Resistance: {currentAirResistance:F2} N";
        if (altitudeTMP) altitudeTMP.text = $"Altitude: {altitude:F2} m";
    }

    private string GetAtmosphereLayer(float alt)
    {
        if (alt < 12000) return "Troposphere";
        else if (alt < 50000) return "Stratosphere";
        else if (alt < 80000) return "Mesosphere";
        else if (alt < 700000) return "Thermosphere";
        else return "Exosphere";
    }

    // UI Buttons (Event Trigger → Pointer Down / Up)
    public void StartThrust() => thrusting = true;
    public void StopThrust() => thrusting = false;

    public void StartLeft() => movingLeft = true;
    public void StopLeft() => movingLeft = false;

    public void StartRight() => movingRight = true;
    public void StopRight() => movingRight = false;
}
