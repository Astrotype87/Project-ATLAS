using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class SubmarineAirSystem : MonoBehaviour
{
    [Header("Air Tank Settings")]
    public float maxAir = 50f;
    public float currentAir = 50f; // start full = top of water

    [Header("UI")]
    public Image airBar;
    public TextMeshProUGUI depthText;

    [Header("Submarine Settings")]
    public Transform submarine;
    public float buoyancyForce = 5f;
    public float submergedOffset = 0.1f;

    [Header("Water Settings")]
    public float waterSurfaceY = 2.155f;
    public float waterBottomY = -2.155f;

    [Header("UI Messages")]
    public TextMeshProUGUI messageText;
    public float messageDuration = 2f;
    private float messageTimer = 0f;

    [Header("Automatic Movement")]
    public float autoMoveSpeed = 2f;

    private Rigidbody2D rb;
    private float displayedAir;

    void Start()
    {
        rb = submarine.GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.linearDamping = 2f;

        displayedAir = currentAir / maxAir;

        // Start at top of water
        float startY = Mathf.Lerp(waterBottomY, waterSurfaceY, currentAir / maxAir) - submergedOffset;
        Vector3 startPos = submarine.position;
        startPos.y = startY;
        submarine.position = startPos;
    }

    void Update()
    {
        UpdateAirBar();
        UpdateDepthText();
        ApplyBuoyancy();
        ClampDepth();
        SwimEndlessly();

        // Debug keys (converted to new Input System)
        if (Keyboard.current != null)
        {
            if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                AddAir(10f);
            }
            else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                ReleaseAir(10f);
            }
        }

        // Countdown message timer
        if (messageTimer > 0f)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0f)
                messageText.text = "";
        }
    }

    void SwimEndlessly()
    {
        Vector3 pos = submarine.position;
        pos.x += autoMoveSpeed * Time.deltaTime;
        submarine.position = pos;
    }

    void UpdateAirBar()
    {
        if (airBar == null) return;
        displayedAir = Mathf.Lerp(displayedAir, currentAir / maxAir, Time.deltaTime * 5f);
        airBar.fillAmount = displayedAir;

        if (displayedAir > 0.6f)
            airBar.color = Color.green;
        else if (displayedAir > 0.3f)
            airBar.color = Color.yellow;
        else
            airBar.color = Color.red;
    }

    void UpdateDepthText()
    {
        if (depthText != null)
            depthText.text = "Depth: " + Mathf.RoundToInt(submarine.position.y * -1f) + " m";
    }

    void ApplyBuoyancy()
    {
        if (rb == null) return;
        float targetDepth = Mathf.Lerp(waterBottomY, waterSurfaceY, currentAir / maxAir) - submergedOffset;
        float depthDiff = targetDepth - submarine.position.y;
        rb.AddForce(Vector2.up * depthDiff * buoyancyForce);
    }

    void ClampDepth()
    {
        Vector3 pos = submarine.position;
        if (pos.y > waterSurfaceY - submergedOffset)
        {
            pos.y = waterSurfaceY - submergedOffset;
            submarine.position = pos;
            rb.linearVelocity = Vector2.zero;
        }
        if (pos.y < waterBottomY)
        {
            pos.y = waterBottomY;
            submarine.position = pos;
            rb.linearVelocity = Vector2.zero;
        }
    }

    void ShowMessage(string msg)
    {
        if (messageText == null) return;
        messageText.text = msg;
        messageTimer = messageDuration;
    }

    public void AddAir(float amount)
    {
        if (currentAir >= maxAir)
        {
            ShowMessage("Air is full!");
            return;
        }
        currentAir = Mathf.Clamp(currentAir + amount, 0, maxAir);
        if (currentAir >= maxAir) ShowMessage("Air is full!");
    }

    public void ReleaseAir(float amount)
    {
        if (currentAir <= 0)
        {
            ShowMessage("No more air to release!");
            return;
        }
        currentAir = Mathf.Clamp(currentAir - amount, 0, maxAir);
        if (currentAir <= 0) ShowMessage("Air is empty!");
    }
}