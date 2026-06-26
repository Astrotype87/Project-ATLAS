using System;
using System.Collections;
using UnityEngine;
using TMPro;

using ProjectATLAS.Input;
using ProjectATLAS.Gameplay;

namespace ProjectATLAS.Minigame.Mini06_MeteorStorm
{
    using Random = UnityEngine.Random;

    public class AirshipController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveForce = 5f;
        public float rotationTorque = 3f;
        public float maxSpeed = 8f;

        public InputButton leftButton;
        public InputButton rightButton;
        
        [Header("Damage")]
        public InputSlider thrustSlider;
        public AudioSource engineSound;
        
        
        [Header("Damage")]
        public float meteorEasySize = 0.25f;
        public float meteorMediumSize = 0.5f;
        public float meteorHardSize = 1f;
        public float meteorDamage = 0.2f;

        [Header("UI")]
        public TMP_Text statusText;
        public TMP_Text mainTimerText;   // 🕒 Main 60s game timer
        public TMP_Text meteorTimerText; // ☄️ Meteor spawn timer
        public TMP_Text damageText;

        [Header("TMP Stats")]
        public TMP_Text angularStatsText; // 🌀 Combined angular stats
        public TMP_Text linearPositionText;
        public TMP_Text linearVelocityText;
        public TMP_Text linearAccelerationText;

        [Header("Audio Effects")]
        public AudioClip backgroundMusic; // 🎵 Background music
        public AudioClip winSound;        // 🏆 Win sound
        public AudioClip warningSound;    // ⚠️ Timer warning sound
        public AudioClip meteorImpact;    // 💥 Meteor hit sound

        [Header("Visual Effects")]
        public GameObject fireEffectPrefab;
        public float fireOffset = -0.8f;

        [Header("Meteor Settings")]
        public Sprite meteorSprite;
        public float meteorSpawnHeight = 8f;
        public float meteorSpeed = 4f;
        public Vector2 spawnDelayRange = new Vector2(4f, 6f);
        public float meteorOffsetRange = 4f;
        public float meteorMinSize = 0.8f;
        public float meteorMaxSize = 2.0f;

        [Header("Game Timer")]
        public float gameDuration = 60f;

        private Rigidbody2D rb;
        private bool gameActive = false;
        private GameObject activeFireEffect;
        private float lastAngularVelocity;
        private float angularAcceleration;
        private Vector2 lastVelocity;
        private Vector2 linearAcceleration;

        private float timer = 0f;
        private float meteorCountdown = 0f;
        private bool gameWon = false;
        private bool warningPlayed = false; // para hindi ulit-ulitin ang warning sound

        private float currentDamage;
        private Difficulty currentDifficulty;

        private AudioSource audioSource; // dynamic audio source

        // PROPERTIES
        public event Action OnGameWin;
        public event Action OnGameLoss;

        public float CurrentDamage => currentDamage;

        public void SetDifficulty(Difficulty difficulty) => currentDifficulty = difficulty;

        public void StartGame()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
                Debug.LogError("Airship needs a Rigidbody2D.");

            rb.gravityScale = 0f;
            rb.linearDamping = 0.5f;
            rb.angularDamping = 1.5f;

            // Gumawa ng AudioSource dynamically
            audioSource = gameObject.AddComponent<AudioSource>();

            // 🎵 Background music setup
            if (backgroundMusic != null)
            {
                audioSource.loop = true;
                audioSource.clip = backgroundMusic;
                audioSource.volume = 0.6f;
                audioSource.Play();
            }

            damageText.text = $"{currentDamage * 100:F2}%";
            StartCoroutine(MeteorRoutine());

            gameActive = true;
        }

        void FixedUpdate()
        {
            if (!gameActive) return;

            float rotateInput = (rightButton?.Value ?? 0) - (leftButton?.Value ?? 0);
            rb.AddTorque(-rotateInput * rotationTorque);
            
            
            // Apply thrust force
            float thrustInput = thrustSlider.Value;
            Vector2 appliedThrust = thrustInput * moveForce * transform.up;
            rb.AddForce(appliedThrust);
            
            // Update audio
            engineSound.volume = thrustSlider.Value;
            engineSound.pitch = (thrustSlider.Value * 2f) + 1f;
            
            
            
            // Max speed
            if (rb.linearVelocity.magnitude > maxSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;

            HandleFireEffect(true);
            UpdateAngularStats();
            UpdateLinearStats();

            // 🕒 Main game timer
            timer += Time.deltaTime;
            float remaining = Mathf.Max(0f, gameDuration - timer);

            // ⚠️ Play warning sound if <10s left (once only)
            if (remaining <= 10f && !warningPlayed)
            {
                warningPlayed = true;
                if (warningSound != null)
                    audioSource.PlayOneShot(warningSound);
            }

            if (mainTimerText != null)
                mainTimerText.text = $" Time Left: {remaining:F1}s";

            if (remaining <= 0f && gameActive)
                WinGame();

            // ☄️ Meteor timer display
            if (meteorTimerText != null)
            {
                if (meteorCountdown > 0f)
                    meteorTimerText.text = $" Next Meteor in {meteorCountdown:F1}s";
                else
                    meteorTimerText.text = " Meteor Incoming!";
            }
        }

        IEnumerator MeteorRoutine()
        {
            yield return new WaitForSeconds(2f);

            while (gameActive)
            {
                float delay = Random.Range(spawnDelayRange.x, spawnDelayRange.y);
                meteorCountdown = delay;

                while (meteorCountdown > 0f && gameActive)
                {
                    meteorCountdown -= Time.deltaTime;
                    yield return null;
                }

                if (!gameActive) yield break;

                SpawnMeteor();
                yield return new WaitForSeconds(0.1f);
            }
        }

        float GetMeteorSize(Difficulty difficulty)
        {
            return difficulty switch
            {
                Difficulty.Easy => meteorEasySize,
                Difficulty.Medium => meteorMediumSize,
                Difficulty.Hard => meteorHardSize,
                _ => meteorEasySize
            };
        }

        void SpawnMeteor()
        {
            GameObject meteor = new GameObject("Meteor");
            meteor.tag = "Meteor";

            float size = Random.Range(meteorMinSize, meteorMaxSize);
            meteor.transform.localScale = new Vector3(size, size, 1f) * GetMeteorSize(currentDifficulty);

            SpriteRenderer sr = meteor.AddComponent<SpriteRenderer>();
            sr.sprite = meteorSprite;
            // sr.sprite = CreateCircleSprite(48, Color.gray);
            sr.sortingOrder = 2;

            CircleCollider2D col = meteor.AddComponent<CircleCollider2D>();
            col.isTrigger = false;

            Rigidbody2D meteorRb = meteor.AddComponent<Rigidbody2D>();
            meteorRb.gravityScale = 0f;
            meteorRb.angularDamping = 0.2f;

            float randomOffset = Random.Range(-meteorOffsetRange, meteorOffsetRange);
            Vector3 spawnPos = new Vector3(transform.position.x + randomOffset, transform.position.y + meteorSpawnHeight, 0f);
            meteor.transform.position = spawnPos;

            Vector2 dir = (transform.position - meteor.transform.position).normalized;
            meteorRb.linearVelocity = dir * meteorSpeed;

            Destroy(meteor, 10f);
        }

        // Sprite CreateCircleSprite(int resolution, Color color)
        // {
        //     Texture2D tex = new Texture2D(resolution, resolution, TextureFormat.ARGB32, false);
        //     Color[] pixels = new Color[resolution * resolution];
        //     float radius = resolution / 2f;

        //     for (int y = 0; y < resolution; y++)
        //     {
        //         for (int x = 0; x < resolution; x++)
        //         {
        //             float dx = x - radius + 0.5f;
        //             float dy = y - radius + 0.5f;
        //             float dist = Mathf.Sqrt(dx * dx + dy * dy);
        //             pixels[y * resolution + x] = dist <= radius ? color : Color.clear;
        //         }
        //     }

        //     tex.SetPixels(pixels);
        //     tex.Apply();
        //     tex.wrapMode = TextureWrapMode.Clamp;

        //     return Sprite.Create(tex, new Rect(0, 0, resolution, resolution), new Vector2(0.5f, 0.5f), 100f);
        // }

        void HandleFireEffect(bool active)
        {
            if (active)
            {
                if (activeFireEffect == null && fireEffectPrefab != null)
                    activeFireEffect = Instantiate(fireEffectPrefab, transform);

                if (activeFireEffect != null)
                {
                    Vector3 offset = transform.up * fireOffset;
                    activeFireEffect.transform.localPosition = offset;
                    activeFireEffect.transform.localRotation = Quaternion.identity;
                }
            }
            else
            {
                if (activeFireEffect != null)
                {
                    Destroy(activeFireEffect);
                    activeFireEffect = null;
                }
            }
        }

        void UpdateAngularStats()
        {
            float angularPos = transform.eulerAngles.z;
            float angularVel = rb.angularVelocity;
            angularAcceleration = (angularVel - lastAngularVelocity) / Time.fixedDeltaTime;
            lastAngularVelocity = angularVel;

            if (angularStatsText != null)
            {
                angularStatsText.text =
                    $"🌀 Angular Stats\n" +
                    $"Position: {angularPos:F2}°\n" +
                    $"Velocity: {angularVel:F2} °/s\n" +
                    $"Acceleration: {angularAcceleration:F2} °/s²";
            }
        }

        void UpdateLinearStats()
        {
            Vector2 velocity = rb.linearVelocity;
            linearAcceleration = (velocity - lastVelocity) / Time.fixedDeltaTime;
            lastVelocity = velocity;

            if (linearPositionText != null)
                linearPositionText.text = $"Position: ({transform.position.x:F2}, {transform.position.y:F2})";
            if (linearVelocityText != null)
                linearVelocityText.text = $"Velocity: ({velocity.x:F2}, {velocity.y:F2})";
            if (linearAccelerationText != null)
                linearAccelerationText.text = $"Acceleration: ({linearAcceleration.x:F2}, {linearAcceleration.y:F2})";
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!gameActive) return;

            if (collision.gameObject.CompareTag("Meteor"))
            {
                // Sound effects
                if (meteorImpact != null)
                    audioSource.PlayOneShot(meteorImpact);

                // Reduce damage
                currentDamage += meteorDamage;

                // Update damage text
                damageText.text = $"{currentDamage * 100:F2}%";


                // Game loss when 100% damage
                if (currentDamage >= 1f)
                {
                    gameActive = false;


                    if (statusText != null)
                        statusText.text = "Aircraft destroyed!";

                    Destroy(collision.gameObject);
                    StopAllCoroutines();

                    StartCoroutine(BroadcastGameLoss());
                }
            }
        }

        IEnumerator BroadcastGameLoss()
        {
            yield return new WaitForSeconds(2f);
            OnGameLoss?.Invoke();
        }

        IEnumerator BroadcastGameWin()
        {
            yield return new WaitForSeconds(2f);
            OnGameWin?.Invoke();
        }


        void WinGame()
        {
            if (!gameActive) return;
            gameActive = false;
            gameWon = true;

            if (winSound != null)
                audioSource.PlayOneShot(winSound);

            if (statusText != null)
                statusText.text = "You survived 1 minute! Victory!";

            StartCoroutine(BroadcastGameWin());
        }
    }
}
