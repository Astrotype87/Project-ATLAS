using UnityEngine;

public class SimpleDrag2D : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;

    [Header("Optional Endpoints")]
    public Transform[] endPoints;

    private void Update()
    {
        if (isDragging)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 targetPos = mousePos + offset;

            // Clamp Y using endpoints
            if (endPoints != null && endPoints.Length > 0)
            {
                float minY = float.MaxValue;
                float maxY = float.MinValue;

                foreach (Transform t in endPoints)
                {
                    if (t.position.y < minY) minY = t.position.y;
                    if (t.position.y > maxY) maxY = t.position.y;
                }

                targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);
            }

            targetPos.z = 0f; // keep on 2D plane
            transform.position = targetPos;
        }
    }

    private void OnMouseDown()
    {
        isDragging = true;
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }
}
