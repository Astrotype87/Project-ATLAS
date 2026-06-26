using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class AirshipManager : MonoBehaviour
{
    [Header("Goal Settings")]
    public int totalGoals = 5; // Kailangan ng 5 goals
    private int currentGoals = 0;

    [Header("UI")]
    public TextMeshProUGUI progressText;

    [Header("Airship Settings")]
    public Transform airship;
    public float liftHeight = 0.5f;
    public float liftSpeed = 1f;

    [Header("Timer Settings")]
    public float countdownTime = 10f; // Countdown time in seconds
    private float currentTime;
    private bool startCountdown = false;


    private bool isLaunched = false;
    
    
    // PROPERTIES
    public event Action OnGameWin;
    
    
    
    
    void Start()
    {
        UpdateUI();

        // Subscribe to the OnGoalCollected event
        BoxGoalTrigger.OnGoalCollected += AddGoal;

       
    }

    void OnDestroy()
    {
        // Unsubscribe from the event when the script is destroyed
        BoxGoalTrigger.OnGoalCollected -= AddGoal;
    }

    void Update()
    {
        if (startCountdown)
        {
            currentTime -= Time.deltaTime;
            UpdateUI();

            if (currentTime <= 0)
            {
                currentTime = 0;
                startCountdown = false;
                LaunchAirship();
            }
        }
    }

    // Public method to be called by other scripts when a goal is achieved
    public void AddGoal()
    {
        currentGoals++;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (startCountdown)
        {
            UpdateTMProText($"Launching in: {currentTime:F1}s");
        }
        else
        {
            float percentage = (float)currentGoals / totalGoals * 100f;
            UpdateTMProText($"Completion: {percentage:F0}%");
        }

        if (currentGoals >= totalGoals && !startCountdown)
        {
            OnGoalsCompleted();
        }
    }

    void OnGoalsCompleted()
    {
        UpdateTMProText("AIRSHIP READY! Launching in...");
        currentTime = countdownTime;
        startCountdown = true;
    }

    void LaunchAirship()
    {
        if (isLaunched) return;

        isLaunched = true;
        StartCoroutine(LiftAirship());
    }

    IEnumerator LiftAirship()
    {
        Vector3 startPos = airship.position;
        Vector3 targetPos = startPos + Vector3.up * liftHeight;

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * liftSpeed;
            airship.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        // Stay floating
        Rigidbody2D rb = airship.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0;
        }
        
        
        // Wait for one second before finishing simulation
        yield return new WaitForSeconds(1f);
        OnGameWin?.Invoke();
    }

    void UpdateTMProText(string text)
    {
        progressText.text = text;
    }

}