using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class aAirshipBalance_Easy : MonoBehaviour
{
    [Header("⚖️ Balance Settings (EASY MODE)")]
    public float tiltMultiplier = 1.2f;
    public float smoothSpeed = 4f;
    public float perfectAngleThreshold = 15f;
    public float balanceHoldTime = 1f;

    [Header("⏱ Timer Settings")]
    public float startTime = 25f;
    private float currentTime;
    public TMP_Text timerText;

    [Header("🏁 Countdown Settings")]
    public TMP_Text countdownText;
    public float countdownDuration = 5f;

    [Header("🏆 Score Settings")]
    public TMP_Text scoreText;
    private int score = 0;

    [Header("⚙️ Weight Display")]
    public TMP_Text weightText;

    [Header("🛩 Airship Visual")]
    public SpriteRenderer airshipBody;

    [Header("💥 Explosion Effect")]
    public GameObject explosionEffect;


    [Header("💬 Message Display")]
    public TMP_Text messageText;
    public float messageDuration = 5f;

    [Header("🏅 Win/Lose UI")]
    public GameObject winPanel;
    public GameObject losePanel;

    private AudioSource sfxSource;
    private Rigidbody2D rb;
    private bool gameOver = false;
    private bool gameWin = false;
    private bool gameStarted = false;
    private float balanceTimer = 0f;
    private bool isGreen = false;
    private bool wasYellow = false;
    private float targetAngle = 0f;
    private int greenCounter = 0;

    private Color balancedColor = Color.green;
    private Color warningColor = Color.yellow;
    private Color unbalancedColor = Color.red;
    
    
    // PROPERTIES
    public event Action OnGameWin;
    public event Action OnGameLoss;
    
    
    
    [System.Obsolete]
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0.3f;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;

        currentTime = startTime;
        UpdateTimerUI();
        UpdateScoreUI();
        UpdateWeightUI(0f);

        if (airshipBody != null)
            airshipBody.color = unbalancedColor;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = true;
        sfxSource.Play();

        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);

        StartCoroutine(StartCountdown());
    }

    [System.Obsolete]
    private System.Collections.IEnumerator StartCountdown()
    {
        float counter = countdownDuration;

        while (counter > 0)
        {
            if (countdownText != null)
                countdownText.text = Mathf.CeilToInt(counter).ToString();

            yield return new WaitForSeconds(1f);
            counter--;
        }

        if (countdownText != null)
            countdownText.text = "GO!";

        // ✅ Randomize weights only (not positions to prevent disappearing)
        RandomizeBoxes();

        yield return new WaitForSeconds(1f);
        if (countdownText != null)
            countdownText.text = "";

        gameStarted = true;
    }

    // ✅ FIXED: Boxes won't fly away
    [System.Obsolete]
    private void RandomizeBoxes()
    {
        DraggableBox[] boxes = FindObjectsOfType<DraggableBox>();
        if (boxes.Length == 0) return;

        foreach (var box in boxes)
        {
            if (box == null) continue;
            Rigidbody2D boxRb = box.GetComponent<Rigidbody2D>();
            if (boxRb == null) continue;

            // Random weight only
            boxRb.mass = Random.Range(0.8f, 2.5f);

            // Small random nudge (not strong force)
            Vector2 randomNudge = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0f, 0.3f));
            boxRb.AddForce(randomNudge, ForceMode2D.Impulse);
        }

        ShowMessage("Boxes ready!");
    }

    [System.Obsolete]
    void Update()
    {
        if (!gameStarted || gameOver || gameWin) return;

        currentTime -= Time.deltaTime;
        if (currentTime <= 0f)
        {
            currentTime = 0f;
            GameOver();
        }
        UpdateTimerUI();

        float currentZ = transform.eulerAngles.z;
        if (currentZ > 180f) currentZ -= 360f;
        float newZ = Mathf.Lerp(currentZ, targetAngle, Time.deltaTime * smoothSpeed);
        transform.rotation = Quaternion.Euler(0f, 0f, newZ);
    }

    [System.Obsolete]
    void FixedUpdate()
    {
        if (!gameStarted || gameOver || gameWin) return;

        DraggableBox[] boxes = FindObjectsOfType<DraggableBox>();
        if (boxes.Length == 0)
        {
            UpdateWeightUI(0f);
            airshipBody.color = unbalancedColor;
            balanceTimer = 0f;
            return;
        }

        float totalWeight = 0f;
        float weightedX = 0f;

        foreach (var box in boxes)
        {
            Rigidbody2D boxRb = box.GetComponent<Rigidbody2D>();
            if (boxRb == null) continue;

            float weight = Mathf.Max(0.1f, boxRb.mass);
            float localX = box.transform.position.x - transform.position.x;

            weightedX += localX * weight;
            totalWeight += weight;
        }

        UpdateWeightUI(totalWeight);
        float balancePoint = weightedX / totalWeight;
        targetAngle = Mathf.Clamp(-balancePoint * tiltMultiplier, -10f, 10f);

        if (Mathf.Abs(targetAngle) <= perfectAngleThreshold)
        {
            balanceTimer += Time.fixedDeltaTime;

            if (balanceTimer >= balanceHoldTime && !isGreen)
            {
                isGreen = true;
                AddScore(1);
                airshipBody.color = balancedColor;
                ShowMessage("Perfect Balance!");

                wasYellow = false;
                balanceTimer = 0f;
                greenCounter++;

                // Small randomize instead of full scatter
                RandomizeBoxes();

                if (greenCounter >= 5)
                {
                    GameWin();
                    return;
                }
            }
            else if (balanceTimer > 0f && !isGreen)
            {
                airshipBody.color = warningColor;
                if (!wasYellow)
                {
                    wasYellow = true;
                }
            }
        }
        else
        {
            balanceTimer = 0f;
            isGreen = false;
            airshipBody.color = unbalancedColor;
        }
    }

    void GameWin()
    {
        gameWin = true;
        ShowMessage("🎉 YOU WIN!");
        airshipBody.color = Color.cyan;
        sfxSource.Stop();

        if (winPanel != null) winPanel.SetActive(true);
        Invoke(nameof(RestartScene), 4f);
        
        OnGameWin?.Invoke();
    }

    [System.Obsolete]
    void GameOver()
    {
        gameOver = true;
        ShowMessage("💥 GAME OVER!");
        ExplodeAll();

        if (losePanel != null) losePanel.SetActive(true);
        Invoke(nameof(RestartScene), 4f);
        
        OnGameLoss?.Invoke();
    }

    [System.Obsolete]
    void ExplodeAll()
    {
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        foreach (var box in FindObjectsOfType<DraggableBox>())
        {
            if (explosionEffect != null)
                Instantiate(explosionEffect, box.transform.position, Quaternion.identity);

            Rigidbody2D boxRb = box.GetComponent<Rigidbody2D>();
            if (boxRb != null)
            {
                Vector2 randomDir = Random.insideUnitCircle.normalized * Random.Range(2f, 4f);
                boxRb.AddForce(randomDir, ForceMode2D.Impulse);
            }

            Destroy(box.gameObject, 0.8f);
        }

        if (airshipBody != null)
            Destroy(airshipBody.gameObject, 0.5f);
    }

    void ShowMessage(string message)
    {
        if (messageText == null) return;
        messageText.text = message;
        CancelInvoke(nameof(ClearMessage));
        Invoke(nameof(ClearMessage), messageDuration);
    }

    void ClearMessage() => messageText.text = "";
    void PlaySFX(AudioClip clip) { if (clip != null) sfxSource.PlayOneShot(clip); }
    void RestartScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    void UpdateTimerUI() { if (timerText != null) timerText.text = $"Time: {currentTime:F1}"; }
    void UpdateWeightUI(float totalWeight) { if (weightText != null) weightText.text = $"Weight: {totalWeight:F1} kg"; }
    void UpdateScoreUI() { if (scoreText != null) scoreText.text = $"Score: {score}"; }
    void AddScore(int points) { score += points; UpdateScoreUI(); }
}
