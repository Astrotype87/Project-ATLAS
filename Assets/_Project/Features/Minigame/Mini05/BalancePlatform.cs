using UnityEngine;
using TMPro;
using System.Collections;

namespace ProjectATLAS.Input
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(AudioSource))]
    public class BalancePlatform : MonoBehaviour
    {
        [Header("Zones")]
        public Transform leftZone;
        public Transform rightZone;

        [Header("UI")]
        public TMP_Text balanceText;
        public TMP_Text leftWeightText;
        public TMP_Text rightWeightText;
        public TMP_Text timerText;

        [Header("Settings")]
        public float rotationSpeed = 2f;
        public float balanceTolerance = 0.2f;
        public float maxRotation = 15f;
        public float timeLimit = 10f;
        public float resetDelay = 2.5f;

        [Header("Sound Effects")]
        public AudioClip winSound;
        public AudioClip loseSound;

        private AudioSource audioSource;

        private float leftWeight = 0f;
        private float rightWeight = 0f;
        private float timer;
        private bool gameEnded = false;

        private Vector3 platformStartPos;
        private Quaternion platformStartRot;
        private Vector3[] boxStartPos;
        private Quaternion[] boxStartRot;
        private Box[] boxes;

        private void Start()
        {
            // Save starting positions
            platformStartPos = transform.position;
            platformStartRot = transform.rotation;

            boxes = FindObjectsOfType<Box>();
            boxStartPos = new Vector3[boxes.Length];
            boxStartRot = new Quaternion[boxes.Length];

            for (int i = 0; i < boxes.Length; i++)
            {
                boxStartPos[i] = boxes[i].transform.position;
                boxStartRot[i] = boxes[i].transform.rotation;
            }

            audioSource = GetComponent<AudioSource>();
            ResetGame();
        }

        private void Update()
        {
            if (gameEnded) return;

            // Timer countdown
            timer -= Time.deltaTime;
            if (timerText != null)
                timerText.text = $"Time: {Mathf.Ceil(timer)}s";

            if (timer <= 0)
            {
                timer = 0;
                EndGame(false);
            }

            // Tilt rotation
            float diff = rightWeight - leftWeight;
            float targetZ = Mathf.Clamp(diff * rotationSpeed, -maxRotation, maxRotation);
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.Euler(0, 0, -targetZ),
                Time.deltaTime * 2f
            );

            UpdateUI();

            // Check for balance
            if (Mathf.Abs(diff) < balanceTolerance && (leftWeight + rightWeight) > 0)
            {
                EndGame(true);
            }
        }

        private void UpdateUI()
        {
            if (leftWeightText != null)
                leftWeightText.text = $"Left: {leftWeight:0.0} kg";
            if (rightWeightText != null)
                rightWeightText.text = $"Right: {rightWeight:0.0} kg";
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (gameEnded) return;

            Box box = col.gameObject.GetComponent<Box>();
            if (box == null) return;

            if (col.transform.position.x < transform.position.x)
                leftWeight += box.weight;
            else
                rightWeight += box.weight;

            UpdateUI();
        }

        private void OnCollisionExit2D(Collision2D col)
        {
            if (gameEnded) return;

            Box box = col.gameObject.GetComponent<Box>();
            if (box == null) return;

            if (col.transform.position.x < transform.position.x)
                leftWeight -= box.weight;
            else
                rightWeight -= box.weight;

            leftWeight = Mathf.Max(0, leftWeight);
            rightWeight = Mathf.Max(0, rightWeight);
            UpdateUI();
        }

        private void EndGame(bool balanced)
        {
            if (gameEnded) return;
            gameEnded = true;

            if (balanceText != null)
            {
                balanceText.gameObject.SetActive(true);
                balanceText.text = balanced ? " BALANCE ACHIEVED!" : " TIME'S UP!";
            }

            // Play sounds 🎵
            if (audioSource != null)
            {
                if (balanced && winSound != null)
                    audioSource.PlayOneShot(winSound);
                else if (!balanced && loseSound != null)
                    audioSource.PlayOneShot(loseSound);
            }

            // Freeze boxes
            foreach (Box b in boxes)
            {
                Rigidbody2D rb = b.GetComponent<Rigidbody2D>();
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
            }

            StartCoroutine(AutoResetAfterDelay());
        }

        private IEnumerator AutoResetAfterDelay()
        {
            yield return new WaitForSeconds(resetDelay);
            ResetGame();
        }

        private void ResetGame()
        {
            leftWeight = 0f;
            rightWeight = 0f;
            timer = timeLimit;
            gameEnded = false;

            if (balanceText != null)
                balanceText.gameObject.SetActive(false);

            transform.position = platformStartPos;
            transform.rotation = platformStartRot;

            for (int i = 0; i < boxes.Length; i++)
            {
                Rigidbody2D rb = boxes[i].GetComponent<Rigidbody2D>();
                rb.constraints = RigidbodyConstraints2D.None;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                boxes[i].transform.position = boxStartPos[i];
                boxes[i].transform.rotation = boxStartRot[i];
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }

            UpdateUI();
            if (timerText != null)
                timerText.text = $"Time: {Mathf.Ceil(timer)}s";
        }
    }
}
