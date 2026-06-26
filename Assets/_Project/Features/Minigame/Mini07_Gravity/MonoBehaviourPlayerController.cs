using UnityEngine;
using ProjectATLAS.Input;
using System;
using TMPro; 

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
[RequireComponent(typeof(AudioSource))]
public class MonoBehaviourPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float jumpForce = 12f;

    [Header("Gravity Flip Settings")]
    public float flipCooldown = 0.25f;
    private float lastFlipTime = -10f;
    private float gravityTimer = 0f; 

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.12f;
    public LayerMask groundLayer;

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;

    [Header("Coyote Time")]
    public float coyoteTime = 0.08f;

    [Header("Respawn Settings")]
    public Transform respawnPoint;
    public string alienTag = "Alien";
    public string checkpointTag = "Checkpoint";
    public string keyTag = "Key";
    public string winTag = "Congrats";

    private Rigidbody2D rb;
    private float horizontal;
    private bool facingRight = true;
    private bool gravityIsNormal = true;
    private float lastGroundedTime = -10f;
    private Vector3 lastSafePosition;

    [HideInInspector] public bool hasKey = false;

    [Header("Mobile Input Buttons")]
    public InputButton leftButton;
    public InputButton rightButton;
    public InputButton jumpButton;
    public InputButton flipButton;


    [Header("UI Display")]
    public TextMeshProUGUI gravityTimerText; 


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();


        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (respawnPoint == null)
        {
            GameObject temp = new GameObject("RespawnPoint");
            temp.transform.position = transform.position;
            respawnPoint = temp.transform;
        }

        lastSafePosition = transform.position;

      
    }
    
    public void StartTimer()
    {
        lastFlipTime = Time.time;
    }
    
    private void Start()
    {
        lastFlipTime = Time.time;
    }

    [System.Obsolete]
    void Update()
    {
        HandleMovementInput();
        HandleFlipGravity();
        HandleJump();

        gravityTimer = Time.time - lastFlipTime;

        if (gravityTimerText != null)
        {
            string direction = gravityIsNormal ? "Normal" : "Flipped";
            gravityTimerText.text = $"Gravity: <b>{direction}</b>\nTime since flip: <b>{gravityTimer:F2}s</b>";
        }
    }

    [System.Obsolete]
    void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);

        if (IsGrounded())
        {
            lastGroundedTime = Time.time;
            lastSafePosition = transform.position;
        }
    }

    void HandleMovementInput()
    {
        horizontal = 0f;

        if (leftButton != null && leftButton.Value > 0)
            horizontal = -1f;
        else if (rightButton != null && rightButton.Value > 0)
            horizontal = 1f;

        if (horizontal > 0.1f && !facingRight) FlipSprite();
        else if (horizontal < -0.1f && facingRight) FlipSprite();
    }

    [System.Obsolete]
    void HandleJump()
    {
        if (jumpButton != null && jumpButton.Value > 0 &&
            (IsGrounded() || Time.time - lastGroundedTime <= coyoteTime))
        {
            Jump();
        }
    }

    void HandleFlipGravity()
    {
        if (flipButton != null && flipButton.Value > 0 && Time.time - lastFlipTime >= flipCooldown)
        {
            FlipGravity();
            lastFlipTime = Time.time;
        }
    }

    bool IsGrounded()
    {
        if (groundCheck == null) return false;
        Collider2D hit = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        return hit != null;
    }

    [System.Obsolete]
    void Jump()
    {
        float gravityDir = Mathf.Sign(rb.gravityScale);
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(-Vector2.up * gravityDir * jumpForce, ForceMode2D.Impulse);
    }

    public void FlipGravity()
    {
        rb.gravityScale *= -1f;
        gravityIsNormal = rb.gravityScale > 0f;

        if (spriteRenderer != null)
            spriteRenderer.flipY = !gravityIsNormal;

        if (groundCheck != null)
        {
            Vector3 g = groundCheck.localPosition;
            g.y *= -1f;
            groundCheck.localPosition = g;
        }



        lastFlipTime = Time.time;
    }

    void FlipSprite()
    {
        facingRight = !facingRight;
        if (spriteRenderer != null)
            spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    [System.Obsolete]
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(alienTag))
        {
            Invoke(nameof(RespawnPlayer), 0.4f);
        }
    }

    [System.Obsolete]
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(checkpointTag))
        {
            respawnPoint.position = other.transform.position;
            lastSafePosition = other.transform.position;
            Debug.Log("Checkpoint reached!");

            UIMessageManager.Instance.ShowMessage("Checkpoint reached!");
        }

        if (other.CompareTag(keyTag))
        {
            hasKey = true;
            Destroy(other.gameObject);
            Debug.Log("Key collected!");
        }

        if (other.CompareTag(winTag))
        {
            Debug.Log("You Win!");
            UIMessageManager.Instance.ShowMessage("Congratulations, You Win!");
        }
    }

    [System.Obsolete]
    void RespawnPlayer()
    {
        transform.position = respawnPoint.position;
        rb.velocity = Vector2.zero;
        Debug.Log("Respawned at last checkpoint!");
    }

    



    public void CollectKey()
    {
        hasKey = true;
        Debug.Log("Key collected!");
    }

    public void UseKey()
    {
        hasKey = false;
        Debug.Log("Key used!");
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (respawnPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(respawnPoint.position, 0.15f);
        }
    }
}
