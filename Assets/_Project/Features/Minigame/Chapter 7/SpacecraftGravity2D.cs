using System;
using UnityEngine;
using UnityEngine.InputSystem; // For Keyboard.current

[RequireComponent(typeof(Rigidbody2D))]
public class SpacecraftController2D : MonoBehaviour
{
    [Header("Movement")]
    public float mainThrust = 15f;
    public float lateralThrust = 5f;
    public float rotationTorque = 15f;
    public bool useRelativeThrust = true;

    [Header("Fuel")]
    public float maxFuel = 100f;
    public float fuel;
    public float fuelConsumptionPerSecond = 10f;
    public float fuelConsumptionRotation = 2f;

    [Header("Gravity")]
    public bool sampleAllGravitySources = true;

    [Header("Reset")]
    public Transform spawnPoint;   // assign sa inspector para alam saan mag-reset
    public string hazardTag = "Planet"; // lagyan ng "Planet" tag ang circle sprite

    Rigidbody2D rb;

    // Input state
    float thrustInput;
    float strafeInput;
    float rotateInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        fuel = maxFuel;

        if (spawnPoint == null)
        {
            // default starting position = initial transform
            GameObject spawn = new GameObject("SpawnPoint");
            spawn.transform.position = transform.position;
            spawnPoint = spawn.transform;
        }
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return; // safety check

        // 🚀 New Input System (Keyboard.current)
        thrustInput = kb.wKey.isPressed || kb.upArrowKey.isPressed ? 1f : 0f;

        strafeInput = 0f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) strafeInput -= 1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) strafeInput += 1f;

        rotateInput = 0f;
        if (kb.qKey.isPressed) rotateInput += 1f;   // rotate left
        if (kb.eKey.isPressed) rotateInput -= 1f;   // rotate right

        // Fuel consumption
        float consumption = 0f;
        if (thrustInput > 0f) consumption += fuelConsumptionPerSecond * Time.deltaTime;
        if (Mathf.Abs(rotateInput) > 0f) consumption += fuelConsumptionRotation * Time.deltaTime;

        if (consumption > 0f)
        {
            fuel -= consumption;
            fuel = Mathf.Max(0f, fuel);

            if (fuel <= 0f)
            {
                thrustInput = 0f;
                strafeInput = 0f;
                rotateInput = 0f;
            }
        }
    }

    [Obsolete]
    void FixedUpdate()
    {
        // 1) Gravity from all sources
        if (sampleAllGravitySources)
        {
            GravitySource2D[] sources = FindObjectsOfType<GravitySource2D>();
            Vector2 totalGravity = Vector2.zero;
            foreach (var s in sources)
                totalGravity += s.GetGravity(rb.position);

            rb.AddForce(totalGravity * rb.mass, ForceMode2D.Force);
        }

        // 2) Apply thrust
        if (thrustInput > 0f && fuel > 0f)
        {
            Vector2 dir = useRelativeThrust ? (Vector2)transform.up : Vector2.up;
            rb.AddForce(dir * mainThrust, ForceMode2D.Force);
        }

        // 3) Apply strafe
        if (Mathf.Abs(strafeInput) > 0f && fuel > 0f)
        {
            rb.AddForce(transform.right * strafeInput * lateralThrust, ForceMode2D.Force);
        }

        // 4) Apply rotation
        if (Mathf.Abs(rotateInput) > 0f && fuel > 0f)
        {
            rb.AddTorque(rotateInput * rotationTorque, ForceMode2D.Force);
        }
    }

    // ✅ Collision reset
    [Obsolete]
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(hazardTag))
        {
            ResetShip();
        }
    }

    [Obsolete]
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(hazardTag))
        {
            ResetShip();
        }
    }

    [Obsolete]
    void ResetShip()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
        fuel = maxFuel; // refill fuel on reset
    }
}

/// <summary>
/// Gravity Source that pulls spacecraft toward it (like a planet).
/// Attach this to a GameObject with a Transform.
/// </summary>
public class GravitySource2D : MonoBehaviour
{
    public float gravityStrength = 9.81f;   // like Earth's gravity
    public float radius = 10f;              // effective range

    public Vector2 GetGravity(Vector2 position)
    {
        Vector2 direction = (Vector2)transform.position - position;
        float distance = direction.magnitude;

        if (distance > radius) return Vector2.zero; // no pull if outside range

        direction.Normalize();
        // Inverse-square law gravity
        float force = gravityStrength / Mathf.Max(distance * distance, 0.1f);

        return direction * force;
    }
}
