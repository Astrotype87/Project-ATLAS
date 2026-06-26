using UnityEngine;
using ProjectATLAS.Input;

public class PlayerPushBox : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 6f;

    [Header("Input Buttons")]
    public InputButton leftButton;
    public InputButton rightButton;
    public InputButton jumpButton;

    [Header("Box Interaction")]
    public float pushForce = 5f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1f;
        rb.freezeRotation = true; // Prevent rotation
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer
     
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
    }

    void HandleMovement()
    {
        float moveInput = rightButton.Value - leftButton.Value;
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Flip the sprite based on movement direction
        if (moveInput > 0)
        {
            spriteRenderer.flipX = false; // Face right
        }
        else if (moveInput < 0)
        {
            spriteRenderer.flipX = true; // Face left
        }
    }

    void HandleJump()
    {
        if (jumpButton.Value > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
          
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Handle box pushing
        if (collision.gameObject.CompareTag("Box"))
        {
            Rigidbody2D boxRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (boxRb != null && !boxRb.isKinematic) // Only push if not "locked"
            {
                float direction = Mathf.Sign(collision.transform.position.x - transform.position.x);
                boxRb.AddForce(Vector2.right * -direction * pushForce, ForceMode2D.Impulse);
               
            }
        }
    }
}