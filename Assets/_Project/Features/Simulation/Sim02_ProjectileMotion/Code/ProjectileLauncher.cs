using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProjectATLAS.Gameplay.UI;
using System;

namespace ProjectATLAS.Simulation.Sim02_ProjectileMotion
{
    public class ProjectileLauncher : MonoBehaviour
    {
        [Header("UI Elements (TMP)")]
        public SimParameter velocityParameter;
        public SimParameter angleParameter;
        public SimParameter gravityParameter;
        // public TMP_InputField velocityInput;
        // public TMP_InputField angleInput;
        // public TMP_InputField gravityInput;
        public Button launchButton;
        public TMP_Text messageText;

        [Header("References")]
        public Rigidbody2D hoverCarRb;
        public SpriteRenderer hoverCarSprite;
        public Transform startPoint;
        public Transform goalTransform;
        public LineRenderer lineRenderer;

        [Header("Boundaries (Invisible)")]
        public Transform leftBoundary;
        public Transform rightBoundary;
        public Transform topBoundary;
        public Transform bottomBoundary;

        [Header("Fade Settings")]
        public float fadeDuration = 0.5f;

        private List<Vector2> trajectoryPoints = new List<Vector2>();
        private Coroutine moveCoroutine;
        private float lastV0, lastAngle, lastGravity;


        // PROPERTIES
        public event Action OnGoalReached;
        public event Action OnGoalReset;
        


        void Start()
        {
            if (launchButton != null) launchButton.onClick.AddListener(LaunchFromUI);
            // ensure line starts hidden and UI cleared
            if (lineRenderer != null) { lineRenderer.enabled = false; lineRenderer.positionCount = 0; }
            if (messageText != null) messageText.text = "";
            ResetToStart(); // default keeps line hidden
        }

        void LaunchFromUI()
        {
            float v0, angleDeg, gravity;
            v0 = velocityParameter.Value;
            angleDeg = angleParameter.Value;
            gravity = gravityParameter.Value;

            lastV0 = v0;
            lastAngle = angleDeg;
            lastGravity = gravity;

            // Compute and show trajectory line immediately
            ComputeTrajectory(v0, angleDeg, gravity);
            if (lineRenderer != null) lineRenderer.enabled = true;
            if (messageText != null) messageText.text = "";

            // Start movement
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(MoveAlongTrajectory());
        }

        void ComputeTrajectory(float v0, float angleDeg, float g)
        {
            trajectoryPoints.Clear();

            if (startPoint == null || lineRenderer == null) return;

            float angleRad = angleDeg * Mathf.Deg2Rad;
            float vx = v0 * Mathf.Cos(angleRad);
            float vy = v0 * Mathf.Sin(angleRad);
            Vector2 start = startPoint.position;

            for (int i = 0; i < 500; i++)
            {
                float t = i * 0.05f;
                Vector2 pos2D = start + new Vector2(vx * t, vy * t - 0.5f * g * t * t);
                trajectoryPoints.Add(pos2D);

                if (i > 0)
                {
                    if (leftBoundary != null && pos2D.x <= leftBoundary.position.x) break;
                    if (rightBoundary != null && pos2D.x >= rightBoundary.position.x) break;
                    if (topBoundary != null && pos2D.y >= topBoundary.position.y) break;
                    if (bottomBoundary != null && pos2D.y <= bottomBoundary.position.y) break;
                }
            }

            // Update line renderer immediately
            lineRenderer.positionCount = trajectoryPoints.Count;
            for (int i = 0; i < trajectoryPoints.Count; i++)
                lineRenderer.SetPosition(i, trajectoryPoints[i]);
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;

            // ensure line is fully opaque when shown
            lineRenderer.startColor = new Color(1f, 1f, 1f, 1f);
            lineRenderer.endColor = new Color(1f, 1f, 1f, 1f);
        }

        IEnumerator MoveAlongTrajectory()
        {
            if (hoverCarRb == null || startPoint == null)
            {
                // nothing to move
                if (messageText != null) messageText.text = "Missed!";
                yield break;
            }

            // prepare car
            hoverCarRb.position = startPoint.position;
            if (hoverCarSprite != null) hoverCarSprite.color = Color.white;
            hoverCarRb.bodyType = RigidbodyType2D.Kinematic;

            CircleCollider2D goalCollider = (goalTransform != null) ? goalTransform.GetComponent<CircleCollider2D>() : null;
            float goalRadius = (goalCollider != null) ? goalCollider.radius * goalTransform.localScale.x : -1f;

            for (int i = 0; i < trajectoryPoints.Count; i++)
            {
                Vector2 target = trajectoryPoints[i];
                hoverCarRb.MovePosition(target);

                // Check hit goal
                if (goalCollider != null)
                {
                    float dist = Vector2.Distance(hoverCarRb.position, goalTransform.position);
                    if (dist <= goalRadius)
                    {
                        if (messageText != null) messageText.text = "Reached!";
                        
                        OnGoalReached?.Invoke();
                        
                        yield return StartCoroutine(FadeOutCarAndLine());
                        
                        OnGoalReset?.Invoke();
                        ResetToStart(false); // after fade, clear line
                        
                        yield break;
                    }
                }

                // Check boundaries
                if ((leftBoundary != null && target.x <= leftBoundary.position.x) ||
                    (rightBoundary != null && target.x >= rightBoundary.position.x) ||
                    (topBoundary != null && target.y >= topBoundary.position.y) ||
                    (bottomBoundary != null && target.y <= bottomBoundary.position.y))
                {
                    if (messageText != null) messageText.text = "Missed!";
                    yield return StartCoroutine(FadeOutCarAndLine());
                    ResetToStart(false);
                    yield break;
                }

                yield return new WaitForFixedUpdate();
            }

            // finished path without hitting
            if (messageText != null) messageText.text = "Missed!";
            yield return StartCoroutine(FadeOutCarAndLine());
            ResetToStart(false);
        }

        IEnumerator FadeOutCarAndLine()
        {
            float elapsed = 0f;
            Color carStart = (hoverCarSprite != null) ? hoverCarSprite.color : Color.white;
            Color carEnd = carStart; carEnd.a = 0f;

            // initial line colors (if null or disabled, guard)
            Color lineStart = (lineRenderer != null) ? lineRenderer.startColor : new Color(1, 1, 1, 1);
            Color lineEnd = (lineRenderer != null) ? lineRenderer.endColor : new Color(1, 1, 1, 1);

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / fadeDuration);

                if (hoverCarSprite != null)
                    hoverCarSprite.color = Color.Lerp(carStart, carEnd, t);

                if (lineRenderer != null)
                {
                    Color ls = Color.Lerp(lineStart, new Color(lineStart.r, lineStart.g, lineStart.b, 0f), t);
                    Color le = Color.Lerp(lineEnd, new Color(lineEnd.r, lineEnd.g, lineEnd.b, 0f), t);
                    lineRenderer.startColor = ls;
                    lineRenderer.endColor = le;
                }

                yield return null;
            }

            if (hoverCarSprite != null) hoverCarSprite.color = carEnd;
            if (lineRenderer != null) { lineRenderer.enabled = false; lineRenderer.positionCount = 0; }
        }

        // default param so Start() can call ResetToStart() safely
        void ResetToStart(bool keepLine = false)
        {
            // stop movement coroutine if still running
            if (moveCoroutine != null) { StopCoroutine(moveCoroutine); moveCoroutine = null; }

            if (hoverCarRb != null && startPoint != null)
            {
                hoverCarRb.position = startPoint.position;
                hoverCarRb.linearVelocity = Vector2.zero;
                hoverCarRb.bodyType = RigidbodyType2D.Kinematic;
            }

            if (hoverCarSprite != null) hoverCarSprite.color = Color.white;

            // clear message
            if (messageText != null) messageText.text = "";

            if (!keepLine && lineRenderer != null)
            {
                lineRenderer.enabled = false;
                lineRenderer.positionCount = 0;
            }
        }
    }
    
}
