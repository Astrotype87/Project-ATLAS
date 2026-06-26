using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Water2D : MonoBehaviour
{
    [Header("Water Properties")]
    public float density = 1f;      // Water density
    public float buoyancyMultiplier = 10f; // Strength of buoyancy

    private void OnTriggerStay2D(Collider2D other)
    {
        Rigidbody2D rb = other.attachedRigidbody;
        if (rb == null) return;

        // Compute submerged fraction
        float waterTop = transform.position.y + transform.localScale.y / 2f;
        float waterBottom = transform.position.y - transform.localScale.y / 2f;
        float objectTop = other.bounds.max.y;
        float objectBottom = other.bounds.min.y;

        float submergedFraction = Mathf.Clamp01((waterTop - objectBottom) / other.bounds.size.y);

        // Apply upward buoyancy force proportional to submerged fraction
        float forceY = (density * buoyancyMultiplier) * submergedFraction;

        rb.AddForce(Vector2.up * forceY, ForceMode2D.Force);
    }
}

