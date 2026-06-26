using TMPro;  // ✅ TMP namespace
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Obstacles : MonoBehaviour
{
    [Header("Player Tag")]
    public string playerTag = "Player";

    [Header("UI Elements")]
    public GameObject gameOverPanel;   // 📝 Drag your background panel here
    public TMP_Text gameOverText;      // 📝 Drag TMP text here
    public Button restartButton;       // 📝 Drag Restart button here
    public Button exitButton;          // 📝 Drag Exit button here

    [Header("Sound (Optional)")]
    public AudioClip gameOverSound;
    private AudioSource audioSource;

    private void Start()
    {
        // Hide UI at start
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);
        if (restartButton != null)
            restartButton.gameObject.SetActive(false);
        if (exitButton != null)
            exitButton.gameObject.SetActive(false);

        // Audio setup
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        // Button listeners
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(playerTag))
        {
            Debug.Log("💥 Player hit obstacle — Game Over!");

            // Show UI
            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);
            if (gameOverText != null)
            {
                gameOverText.text = "💀 Game Over!\nYou Crashed!";
                gameOverText.gameObject.SetActive(true);
            }
            if (restartButton != null)
                restartButton.gameObject.SetActive(true);
            if (exitButton != null)
                exitButton.gameObject.SetActive(true);

            // Play sound if assigned
            if (gameOverSound != null)
                audioSource.PlayOneShot(gameOverSound);

            // Stop player
            Rigidbody2D rb = collision.collider.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            // Freeze game
            Time.timeScale = 0f;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Debug.Log("🚪 Exit Game pressed!");
        Application.Quit();

        // (Optional for testing in Editor)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}