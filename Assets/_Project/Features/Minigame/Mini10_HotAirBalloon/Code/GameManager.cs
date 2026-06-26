using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class MiniGameManager : MonoBehaviour
{
    [Header("⏳ Game Settings")]
    public float timeLimit = 500;

    [Header("📊 UI Elements")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI keyText;        // 👈 same logic as timer
    public GameObject gameOverPanel;
    public GameObject winPanel;
    public GameObject keyIcon;

    [Header("🗝️ Key System")]
    public bool hasKey = false;

    private float timeRemaining;
    private bool isGameOver = false;
    
    // PROPERTIES
    public event Action OnGameWin;
    public event Action OnGameLose;
    
    
    
    

    void Start()
    {
        timeRemaining = timeLimit;

        // Panels
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);

        // Key UI — show from start just like timer
        if (keyText != null)
        {
            keyText.gameObject.SetActive(true);      // make sure it's visible
            keyText.text = "Key: 0 / 1";             // show default
        }

        if (keyIcon != null)
            keyIcon.SetActive(false);

        Time.timeScale = 1f;
    }

    void Update()
    {
        if (isGameOver) return;

        // 🕒 Timer countdown
        timeRemaining -= Time.deltaTime;

        if (timerText != null)
            timerText.text = Mathf.Ceil(timeRemaining).ToString() + "s";

        if (timeRemaining <= 0)
        {
            GameOver();
        }
    }

    // 🗝️ Called when key is collected
    public void CollectKey()
    {
        hasKey = true;

        if (keyIcon != null)
            keyIcon.SetActive(true);

        if (keyText != null)
            keyText.text = "Key: 1 / 1";   // 🔥 Just like the timer, update the text

        Debug.Log("✅ Key collected!");
    }

    // 💀 Game Over
    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
        
        OnGameLose?.Invoke();
    }

    // 🏆 Win
    public void Win()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (winPanel != null)
            winPanel.SetActive(true);
            

        Time.timeScale = 0f;
        
        OnGameWin?.Invoke();
    }

    // 🔁 Restart Button
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
