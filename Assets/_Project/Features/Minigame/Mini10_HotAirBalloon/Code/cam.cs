using UnityEngine;

public class cam : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;         // Assign your balloon here 🪂

    [Header("Camera Settings")]
    public float smoothSpeed = 5f;   // How smooth the camera follows
    public Vector3 offset;           // Offset distance from the balloon

    [Header("Clamp Settings")]
    public bool useClamp = false;    // Optional: limit camera movement
    public Vector2 minPosition;      // Min X,Y camera can go
    public Vector2 maxPosition;      // Max X,Y camera can go

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate desired position
        Vector3 desiredPosition = target.position + offset;

        // Smoothly move camera
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Optional clamp (if you don't want camera to go outside map)
        if (useClamp)
        {
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minPosition.x, maxPosition.x);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minPosition.y, maxPosition.y);
        }

        transform.position = smoothedPosition;
    }
}
