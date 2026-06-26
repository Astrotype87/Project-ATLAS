using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class habControl : MonoBehaviour
{
    [Header("Float Settings")]
    public float maxUpwardSpeed = 2f;        // Max rise speed
    public float maxDownwardSpeed = -1f;     // Max fall speed
    public float fireChangeSpeed = 0.5f;     // Speed of manual fire change
    public float verticalSmooth = 0.25f;     // Vertical smoothness
    public float airDrag = 0.99f;            // Air drag for smooth flight

    [Header("Horizontal Movement")]
    public float horizontalSpeed = 2f;
    public float horizontalSmooth = 5f;

    [Header("UI Elements")]
    public Slider firePowerSlider;           // 🔥 Fire power display
    
    [Header("Fire Particle Effect")]
    public ParticleSystem fireParticle;
    
    [Range(0f, 1f)]
    public float firePower = 0f;

    private Rigidbody2D rb;
    private bool isIncreasingFire = false;
    private bool isDecreasingFire = false;
    private int horizontalInput = 0;
    private float targetVerticalVelocity = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        if (firePowerSlider != null)
        {
            firePowerSlider.minValue = 0f;
            firePowerSlider.maxValue = 1f;
            firePowerSlider.value = firePower;
        }
    }

    void Update()
    {
        // 🔥 Manual fire control only
        if (isIncreasingFire)
        {
            firePower = Mathf.MoveTowards(firePower, 1f, fireChangeSpeed * Time.deltaTime);
        }
        else if (isDecreasingFire)
        {
            firePower = Mathf.MoveTowards(firePower, 0f, fireChangeSpeed * Time.deltaTime);
        }

        // 🪂 Target velocity
        targetVerticalVelocity = Mathf.Lerp(maxDownwardSpeed, maxUpwardSpeed, firePower);

        // 🕹️ Smooth horizontal + vertical movement
        float targetHorizontal = horizontalInput * horizontalSpeed;
        rb.linearVelocity = new Vector2(
            Mathf.Lerp(rb.linearVelocity.x, targetHorizontal, Time.deltaTime * horizontalSmooth),
            Mathf.Lerp(rb.linearVelocity.y, targetVerticalVelocity, verticalSmooth)
        );

        // 🌬️ Drag for smooth float
        rb.linearVelocity *= airDrag;

        // 🔁 Update slider UI
        if (firePowerSlider != null)
        {
            firePowerSlider.value = firePower;
            
            var main = fireParticle.main;
            main.startLifetime = Mathf.Lerp(0, 0.5f, firePower);
        }
    }

    // 🔘 UI Button Functions
    public void StartIncreaseFire() => isIncreasingFire = true;
    public void StopIncreaseFire() => isIncreasingFire = false;
    public void StartDecreaseFire() => isDecreasingFire = true;
    public void StopDecreaseFire() => isDecreasingFire = false;

    public void MoveLeft() => horizontalInput = -1;
    public void MoveRight() => horizontalInput = 1;
    public void StopMove() => horizontalInput = 0;
}
