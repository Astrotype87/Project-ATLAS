using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ProjectATLAS
{
    public class GoalPanelController : MonoBehaviour
    {
        [Header("References")]
        public Transform rocket;
        public GameObject goalPanelUI;  // ✅ Renamed to avoid name conflict
        public Camera mainCamera;

        [Header("Atmosphere Settings")]
        public float backgroundHeight = 10f;
        public int backgroundsPerLayer = 6;
        public int totalLayers = 5;

        [Header("Goal Transition")]
        public Color goalFadeColor = Color.black;   // Color to fade the background to
        public float fadeDuration = 2f;             // How fast background fades

        private bool goalReached = false;
        private Rigidbody2D rb;
        private Color originalColor;
        
        // PROPERTIES
        public Action OnGameWin;
        
        
        void Start()
        {
            if (goalPanelUI != null)
                goalPanelUI.SetActive(false);

            if (rocket != null)
                rb = rocket.GetComponent<Rigidbody2D>();

            if (mainCamera == null)
                mainCamera = Camera.main;

            if (mainCamera != null)
                originalColor = mainCamera.backgroundColor;
        }

        void Update()
        {
            if (goalReached || rocket == null) return;

            float layerHeight = backgroundHeight * backgroundsPerLayer;
            float altitude = Mathf.Max(0, rocket.position.y);
            int currentLayer = Mathf.FloorToInt(altitude / layerHeight);

            if (currentLayer >= totalLayers - 1)
            {
                goalReached = true;
                StartCoroutine(ShowGoalPanelSmooth());
            }
        }

        private IEnumerator ShowGoalPanelSmooth()
        {
            // 🛑 Stop rocket movement
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero; // ✅ Correct property name
                rb.isKinematic = true;
            }

            // 🌑 Fade background color smoothly
            if (mainCamera != null)
            {
                Color startColor = mainCamera.backgroundColor;
                float t = 0f;

                while (t < 1f)
                {
                    t += Time.deltaTime / fadeDuration;
                    mainCamera.backgroundColor = Color.Lerp(startColor, goalFadeColor, t);
                    yield return null;
                }

                mainCamera.backgroundColor = goalFadeColor;
            }

            // 🪐 Show goal panel
            if (goalPanelUI != null)
                goalPanelUI.SetActive(true);
            
            OnGameWin?.Invoke();
        }

        // 🔁 Called by Restart Button
        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
