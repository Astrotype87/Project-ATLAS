using UnityEngine;

public class DraggableBox : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isDragging = false;
    private Vector3 offset;
    private Camera cam;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1; // para hindi bumagsak
        cam = Camera.main;
    }

    void OnMouseDown()
    {
        isDragging = true;
        offset = transform.position - GetMouseWorldPosition();
    }

    [System.Obsolete]
    void OnMouseUp()
    {
        isDragging = false;
        rb.velocity = Vector2.zero;
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 mousePos = GetMouseWorldPosition() + offset;
            rb.MovePosition(Vector3.Lerp(transform.position, mousePos, Time.deltaTime * 15f));
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = 10f; // distance from camera
        return cam.ScreenToWorldPoint(mousePoint);
    }
}
