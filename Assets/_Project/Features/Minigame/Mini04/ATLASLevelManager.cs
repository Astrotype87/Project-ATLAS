using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectATLAS.Minigame.Mini04_CargoAirshipStartup
{
    public class CargoLevelManager : MonoBehaviour
    {
        [Header("Player Settings")]
        public Rigidbody2D playerRb;
        public Transform playerStartPos;
        public float moveSpeed = 5f;
        public float jumpForce = 6f;
        private bool canJump = true;

        [Header("Input Buttons")]
        public ProjectATLAS.Input.InputButton leftButton;
        public ProjectATLAS.Input.InputButton rightButton;
        public ProjectATLAS.Input.InputButton jumpButton;
        public Button resetButton;

        [Header("Boxes and Goals")]
        public GameObject[] boxes;
        public Transform[] boxStartPositions;
        public SpriteRenderer[] goalBoxes;
        public GoalTrigger[] goalTriggers;
        public Color goalActiveColor = Color.green;
        public int totalGoals = 3;

        [Header("Bridge Settings")]
        public GameObject bridge;
        public int goalsToActivateBridge = 2;

        [Header("UI")]
        public TextMeshProUGUI statusText;
        public float gameTime = 50f;

        private float timer;
        private int goalsCompleted = 0;
        private bool hasWon = false;
        private bool gameOver = false;
        
        
        // PROPERTIES
        public event Action OnGameWin;
        public event Action OnGameLoss;
        
        
        
        private void Start()
        {
            if (resetButton != null)
                resetButton.onClick.AddListener(ManualReset);

            ResetGame();

            if (bridge != null)
                bridge.SetActive(false);
        }

        private void Update()
        {
            if (hasWon || gameOver) return;

            HandleMovement();
            HandleTimer();
        }

        private void HandleMovement()
        {
            float move = 0f;
            if (leftButton != null && leftButton.Value == 1) move = -1f;
            else if (rightButton != null && rightButton.Value == 1) move = 1f;

            playerRb.linearVelocity = new Vector2(move * moveSpeed, playerRb.linearVelocity.y);

            // Jump + SFX
            if (jumpButton != null && jumpButton.Value == 1 && canJump)
            {
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, jumpForce);
                canJump = false;
            }

            playerRb.freezeRotation = true;
        }

        private void HandleTimer()
        {
            timer -= Time.deltaTime;
            if (!hasWon)
                statusText.text = "⏱ Time Left: " + Mathf.Ceil(timer) + "s";

            if (timer <= 0)
            {
                timer = 0;
                GameOver();
            }
        }

        public void AddGoal(SpriteRenderer goal)
        {
            if (goal.color != goalActiveColor)
            {
                goal.color = goalActiveColor;
                goalsCompleted++;



                if (goalsCompleted >= goalsToActivateBridge && bridge != null)
                {
                    bridge.SetActive(true);

                }

                if (goalsCompleted >= totalGoals)
                    WinGame();
            }
        }

        private void WinGame()
        {
            hasWon = true;
            statusText.text = "🏆 YOU WIN!";
            playerRb.linearVelocity = new Vector2(0f, jumpForce * 1.5f);
            
            OnGameWin?.Invoke();
        }

        private void GameOver()
        {
            if (gameOver) return;
            gameOver = true;
            statusText.text = "💀 GAME OVER!";
            
            OnGameLoss?.Invoke();
        }

        public void ManualReset()
        {
            hasWon = false;
            gameOver = false;
            ResetGame();
        }

        public void ResetGame()
        {
            timer = gameTime;
            goalsCompleted = 0;
            statusText.text = "Get ready!";
            canJump = true;

            // Reset player
            if (playerStartPos != null)
                playerRb.transform.position = playerStartPos.position;

            playerRb.linearVelocity = Vector2.zero;
            playerRb.freezeRotation = true;

            // Reset boxes
            for (int i = 0; i < boxes.Length && i < boxStartPositions.Length; i++)
            {
                GameObject box = boxes[i];
                Transform startPos = boxStartPositions[i];

                if (box != null && startPos != null)
                {
                    box.SetActive(true);
                    box.transform.position = startPos.position;
                    box.transform.rotation = Quaternion.identity;

                    Rigidbody2D rb = box.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        rb.linearVelocity = Vector2.zero;
                        rb.angularVelocity = 0f;
                    }
                }
            }

            // Reset goal colors
            foreach (var g in goalBoxes)
            {
                if (g != null)
                    g.color = Color.white;
            }

            // Reset bridge
            if (bridge != null)
                bridge.SetActive(false);

            // Reset triggers
            foreach (var gt in goalTriggers)
            {
                if (gt != null)
                    gt.ResetGoal();
            }
        }

        private void PlaySFX(AudioClip clip)
        {
            if (clip == null) return;
            AudioSource.PlayClipAtPoint(clip, playerRb.transform.position);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider != null)
                canJump = true;
        }
    }
    
}
