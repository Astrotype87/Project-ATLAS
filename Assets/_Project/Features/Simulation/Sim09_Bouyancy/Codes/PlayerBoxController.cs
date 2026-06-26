using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

using ProjectATLAS.Input;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerBoxController : MonoBehaviour
{
    [Header("Box Properties")]
    public float density = 1f;
    public float volume = 1f;

    [Header("Slider Ranges")]
    public float minDensity = 0.1f;
    public float maxDensity = 4.5f;
    public float minVolume = 0.5f;
    public float maxVolume = 1f;

    [Header("Water Settings")]
    public Transform waterTransform;
    public float smoothSpeed = 5f;

    [Header("UI Sliders")]
    public Slider densitySlider;
    public Slider volumeSlider;

    [Header("UI TMP Text")]
    public TMP_Text densityText;
    public TMP_Text volumeText;
    public TMP_Text massText;

    [Header("UI Panels")]
    public GameObject gameOverPanel;
    public GameObject winPanel;

    [Header("Timer Settings")]
    public float winHoldTime = 5f;      // fully submerged/floating for 5s = win
    public float noInteractionLimit = 10f; // 10s no adjustment = game over

    private Rigidbody2D rb;
    private bool isDragging = false;
    private Vector2 dragOffset;
    private Camera mainCamera;

    // Timers
    private float submergedFloatTimer = 0f;
    private float noInteractionTimer = 0f;
    private bool hasInteracted = false;

    // Previous state
    private float lastDensity;
    private float lastVolume;
    private Vector2 lastPosition;
    
    public event Action OnGameWin;
    public event Action OnGameLose;
    
    
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1f;
        rb.mass = density * volume;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.linearDamping = 3f;
        rb.angularDamping = 3f;

        mainCamera = Camera.main;
        transform.localScale = Vector3.one * volume;

        if (densitySlider != null)
        {
            densitySlider.minValue = minDensity;
            densitySlider.maxValue = maxDensity;
            densitySlider.value = density;
            densitySlider.onValueChanged.AddListener(SetDensity);
        }

        if (volumeSlider != null)
        {
            volumeSlider.minValue = minVolume;
            volumeSlider.maxValue = maxVolume;
            volumeSlider.value = volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        UpdateUIText();

        lastDensity = density;
        lastVolume = volume;
        lastPosition = transform.position;

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);

        Time.timeScale = 1f; // ensure game runs
    }

    private void Update()
    {
        // HandleDrag();
        CheckDragInteraction();
        CheckTimers();
    }

    private void FixedUpdate()
    {
        if (!InputInteractionState.IsDragging && IsInWater())
            ApplyBuoyancy();
    }

    // #region Drag Logic
    private void CheckDragInteraction()
    {
        if (InputInteractionState.IsDragging)
        {
            hasInteracted = true;
            noInteractionTimer = 0f;
        }
    }
    // private void HandleDrag()
    // {
    //     Vector2 worldPos = Vector2.zero;
    //     bool press = false;

    //     if (Mouse.current != null && Mouse.current.leftButton.isPressed)
    //     {
    //         Vector2 mousePos = Mouse.current.position.ReadValue();
    //         worldPos = mainCamera.ScreenToWorldPoint(mousePos);
    //         press = true;
    //     }

    //     if (press)
    //     {
    //         if (!isDragging)
    //         {
    //             Collider2D hit = Physics2D.OverlapPoint(worldPos);
    //             if (hit != null && hit.gameObject == gameObject)
    //             {
    //                 isDragging = true;
    //                 dragOffset = (Vector2)transform.position - worldPos;
    //                 hasInteracted = true;
    //                 noInteractionTimer = 0f; // reset no interaction
    //             }
    //         }

    //         if (isDragging)
    //         {
    //             Vector2 targetPos = worldPos + dragOffset;
    //             rb.MovePosition(targetPos);
    //         }
    //     }
    //     else
    //     {
    //         isDragging = false;
    //     }
    // }
    // #endregion

    #region Buoyancy
    private bool IsInWater()
    {
        if (waterTransform == null) return false;
        float boxY = rb.position.y;
        float waterTop = waterTransform.position.y + waterTransform.localScale.y / 2f;
        float waterBottom = waterTransform.position.y - waterTransform.localScale.y / 2f;
        return boxY <= waterTop && boxY >= waterBottom;
    }

    private bool IsFullySubmergedOrFloating()
    {
        if (waterTransform == null) return false;
        float boxTop = transform.position.y + transform.localScale.y / 2f;
        float boxBottom = transform.position.y - transform.localScale.y / 2f;
        float waterTop = waterTransform.position.y + waterTransform.localScale.y / 2f;
        float waterBottom = waterTransform.position.y - waterTransform.localScale.y / 2f;

        // Fully submerged
        if (boxTop <= waterTop && boxBottom >= waterBottom)
            return true;

        // Floating near surface
        if (Mathf.Abs(boxBottom - waterTop) <= 0.05f)
            return true;

        return false;
    }

    private void ApplyBuoyancy()
    {
        if (waterTransform == null) return;
        float waterTop = waterTransform.position.y + waterTransform.localScale.y / 2f - transform.localScale.y / 2f;
        float waterBottom = waterTransform.position.y - waterTransform.localScale.y / 2f + transform.localScale.y / 2f;
        float t = (density - minDensity) / (maxDensity - minDensity);
        t = Mathf.Clamp01(t);
        float targetY = Mathf.Lerp(waterTop, waterBottom, t);
        rb.MovePosition(new Vector2(rb.position.x, Mathf.Lerp(rb.position.y, targetY, smoothSpeed * Time.fixedDeltaTime)));
    }
    #endregion

    #region Sliders & UI
    private void SetDensity(float value)
    {
        density = Mathf.Clamp(value, minDensity, maxDensity);
        rb.mass = density * volume;
        hasInteracted = true;
        noInteractionTimer = 0f;
        UpdateUIText();
    }

    private void SetVolume(float value)
    {
        volume = Mathf.Clamp(value, minVolume, maxVolume);
        transform.localScale = Vector3.one * volume;
        rb.mass = density * volume;
        hasInteracted = true;
        noInteractionTimer = 0f;
        UpdateUIText();
    }

    private void UpdateUIText()
    {
        if (densityText != null) densityText.text = $"{density:F2}";
        if (volumeText != null) volumeText.text = $"{volume:F2}";
        if (massText != null) massText.text = $"{(density * volume):F2} kg";
    }
    #endregion

    #region Win / GameOver Logic
    private void CheckTimers()
    {
        // Win: fully submerged/floating after touching for 5 secs
        if (hasInteracted && IsFullySubmergedOrFloating())
        {
            submergedFloatTimer += Time.deltaTime;
            if (submergedFloatTimer >= winHoldTime)
                ShowWinPanel();
        }
        else
        {
            submergedFloatTimer = 0f;
        }

        // GameOver: 10 secs no interaction
        bool noChange =
            Mathf.Approximately(lastDensity, density) &&
            Mathf.Approximately(lastVolume, volume) &&
            Vector2.Distance(lastPosition, transform.position) < 0.001f;

        if (!hasInteracted || noChange)
        {
            noInteractionTimer += Time.deltaTime;
            if (noInteractionTimer >= noInteractionLimit)
                ShowGameOverPanel();
        }
        else
        {
            noInteractionTimer = 0f;
        }

        lastDensity = density;
        lastVolume = volume;
        lastPosition = transform.position;
    }

    private void ShowWinPanel()
    {
        // if (winPanel != null) winPanel.SetActive(true);
        // Time.timeScale = 0f;
        // OnGameWin?.Invoke();
    }

    private void ShowGameOverPanel()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
        OnGameLose?.Invoke();
    }
    
    
    
    
    #endregion
}
