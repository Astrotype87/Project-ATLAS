using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using ProjectATLAS.Input;

namespace ProjectATLAS.Simulation.Sim06_AircraftTesting
{
    public class AircraftController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float thrustForce = 10f;
        public float torqueForce = 150f;
        public float maxSpeed = 8f;

        [Header("UI Elements")]
        public Button leftButton;
        public Button rightButton;
        public InputSlider thrustSlider;
        // public Button powerButton;

        [Header("TMP Stats")]
        public TMP_Text angularPositionText;
        public TMP_Text angularVelocityText;
        public TMP_Text angularAccelerationText;
        public TMP_Text linearPositionText;
        public TMP_Text linearVelocityText;
        public TMP_Text linearAccelerationText;
        public TMP_Text congratsText;

        [Header("Visual Effects")]
        public GameObject[] fireObjects;
        
        [Header("SFX")]
        public AudioSource engineSound;

        private Rigidbody2D rb;
        private bool pressingLeft;
        private bool pressingRight;
        // private bool pressingPower;
        private float thrustPower;
        private bool hasFinished = false;
        private bool canMove = true;

        private GameObject activeFireEffect;
        private AudioSource bgmSource;

        // 🧭 Angular Stats
        private float lastAngularVelocity;
        private float angularAcceleration;

        // 🧭 Linear Stats
        private Vector2 lastVelocity;
        private Vector2 linearAcceleration;
        
        // PROPERTIES
        public event Action OnGameWin;
        
        
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.linearDamping = 0.5f;
            rb.angularDamping = 1.5f;

            AddButtonEvents(leftButton, () => pressingLeft = true, () => pressingLeft = false);
            AddButtonEvents(rightButton, () => pressingRight = true, () => pressingRight = false);
            

            if (congratsText != null) congratsText.gameObject.SetActive(false);

           
        }

        void FixedUpdate()
        {
            if (!canMove || hasFinished) return;
            
            
            // 🔁 Rotation via torque
            if (pressingLeft)
                rb.AddTorque(torqueForce * Time.fixedDeltaTime);
            if (pressingRight)
                rb.AddTorque(-torqueForce * Time.fixedDeltaTime);

            // 🚀 Thrust forward
            thrustPower = thrustSlider.Value;
            Vector2 appliedThrust = thrustForce * thrustPower * transform.up;
            rb.AddForce(appliedThrust);
            
            if (rb.linearVelocity.magnitude > maxSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            
            // Sound effects volume
            engineSound.volume = thrustSlider.Value;
            engineSound.pitch = (thrustSlider.Value * 2f) + 1f;
            
            if (thrustPower >= 0.5f)
            {
                HandleFireEffect(true);
            }
            else
            {
                HandleFireEffect(false);
            }

            UpdateAngularStats();
            UpdateLinearStats();
        }

        void HandleFireEffect(bool active)
        {
            foreach (var fireObject in fireObjects)
            {
                if (fireObject) fireObject.SetActive(active);
            }
        }

        void UpdateAngularStats()
        {
            float angularPos = transform.eulerAngles.z;
            float angularVel = rb.angularVelocity;
            angularAcceleration = (angularVel - lastAngularVelocity) / Time.fixedDeltaTime;
            lastAngularVelocity = angularVel;

            if (angularPositionText != null)
                angularPositionText.text = $"Angular Position:\n{angularPos:F2}°";
            if (angularVelocityText != null)
                angularVelocityText.text = $"Angular Velocity:\n{angularVel:F2} °/s";
            if (angularAccelerationText != null)
                angularAccelerationText.text = $"Angular Acceleration:\n{angularAcceleration:F2} °/s²";
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

        // 🟢 Win condition: Green Circle trigger
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("GreenCircle"))
            {
                hasFinished = true;
                canMove = false;
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;

                if (congratsText != null)
                {
                    congratsText.gameObject.SetActive(true);
                    congratsText.text = " Test Completed Successfully! You Reached the Green Circle!";
                }
                
                OnGameWin?.Invoke();

                // Optional: destroy the circle for feedback
                Destroy(collision.gameObject);
            }
        }

        void AddButtonEvents(Button btn, Action onPress, Action onRelease)
        {
            if (btn == null) return;

            EventTrigger trigger = btn.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = btn.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry pressEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            pressEntry.callback.AddListener((data) => { onPress.Invoke(); });
            trigger.triggers.Add(pressEntry);

            EventTrigger.Entry releaseEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            releaseEntry.callback.AddListener((data) => { onRelease.Invoke(); });
            trigger.triggers.Add(releaseEntry);
        }
    }
    
}
