using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform submarine;    // Drag your submarine here
    public float smoothSpeed = 0.125f;
    public Vector3 offset;         // Optional offset
    public float fixedY = 0f;      // Set this to the Y you want the camera to stay at

    void LateUpdate()
    {
        if (submarine == null) return;

        // Only follow X axis; keep Y fixed
        Vector3 desiredPosition = new Vector3(submarine.position.x + offset.x, fixedY + offset.y, -10f);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}

