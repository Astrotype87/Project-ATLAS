using UnityEngine;
using UnityEngine.InputSystem; // Required for the new Input System
using ProjectATLAS.Input;

[RequireComponent(typeof(Rigidbody2D))]
public class DataWingControllerNewInput : MonoBehaviour
{
    [Header("Movement Settings")]
    public float thrustForce = 5f;
    public float turnSpeed = 200f;
    public float wallBoostMultiplier = 1.5f;

    [Header("Trail Effect")]
    public TrailRenderer trail;

    [Header("Input")]
    public InputButton thrustButton; // float Input
    public InputButton leftButton;   // float Input
    public InputButton rightButton;  // float Input

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (trail != null)
        {
            trail.emitting = true;
        }
    }

    void Update()
    {
        // --- Rotation ---
        float turn = 0f;

        // left is positive, right is negative (consistent with your old version)
        if (leftButton.Value > 0f)
            turn += 1f;
        if (rightButton.Value > 0f)
            turn -= 1f;

        transform.Rotate(0, 0, turn * turnSpeed * Time.deltaTime);

        // --- Thrust ---
        if (thrustButton.Value > 0f)
        {
            // Optionally multiply thrustForce by thrustButton.Value
            rb.AddForce(thrustButton.Value * thrustForce * transform.up);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // Wall sliding boost
        Vector2 normal = collision.contacts[0].normal;
        float angle = Vector2.Angle(rb.linearVelocity, -normal);

        if (angle < 45f) // sliding along the wall
        {
            rb.linearVelocity *= wallBoostMultiplier;
        }
    }
}
