using UnityEngine;

using ProjectATLAS.Input;
using TMPro;
using System;

namespace ProjectATLAS.Minigame.Mini02_DesertNavigation
{
    public class KinematicHovercraftPhysics : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private new Rigidbody2D rigidbody2D;
        [SerializeField] private LayerMask trackLayer;
        
        [Header("Settings")]
        [SerializeField] private float minSpeed = 0f;
        [SerializeField] private float maxSpeed = 10f;
        [SerializeField] private float speedResponse = 1f;
        [SerializeField] private float rotationResponse = 5f;
        [SerializeField] private float airResponse = 1f;
        
        [Header("Input")]
        [SerializeField] private InputSlider speedSlider; // returns 0-1
        [SerializeField] private InputDirection inputDirection; // returns -180 - 180  // Direction, angle
        [SerializeField] private Vector2 windVelocity;
        
        [Header("Input")]
        [SerializeField] private AudioSource engineSound;
        
        
        [Header("Offtrack")]
        [SerializeField] private TMP_Text damageText;
        [SerializeField] private float damageAmount;
        [Tooltip("How much damage is received per second when off-track.")]
        [SerializeField] private float damageRate = 0.1f;
        [SerializeField] private float slowdownRate = 0.5f;
        
        private float smoothedSpeedInput;
        private float speedInputVelocity;
        private float rotationVelocity;
        private Vector2 currentWindVelocity;
        
        
        // PROPERTIES
        public float CurrentDamage => damageAmount;
        public bool IsOnTrack { get; private set; }
        public bool IsCrashed { get; private set; }
        
        public event Action OnCrashed;
        
        
        // MONOBEHAVIOUR METHODS
        private void FixedUpdate()
        {
            if (IsCrashed) return;
            
            float rotationInput = inputDirection.Angle;
            
            float speedInput = speedSlider.Value;
            if (!IsOnTrack) speedInput *= slowdownRate;
            
            smoothedSpeedInput = Mathf.SmoothDamp(
                smoothedSpeedInput,
                speedInput,
                ref speedInputVelocity,
                1f / speedResponse, // smooth time
                Mathf.Infinity,
                Time.fixedDeltaTime
            );
            
            
            DetectTrack();
            ApplyRotation(rotationInput);
            ApplySpeed(smoothedSpeedInput);
        }
        
        private void LateUpdate()
        {
            float speedInput = speedSlider.Value;
            engineSound.volume = speedInput;
            engineSound.pitch = (speedInput * 2f) + 1f;
        }
        
        // PUBLIC METHODS
        public void SetWindVelocity(Vector2 windVelocity)
        {
            this.windVelocity = windVelocity;
        }
        
        // PRIVATE METHODS
        private void ApplySpeed(float speedInput)
        {
            float targetSpeed = Mathf.Lerp(minSpeed, maxSpeed, speedInput);
            
            Vector2 forward = transform.up;
            Vector2 targetVelocity = forward * targetSpeed;
            
            currentWindVelocity.x = Mathf.SmoothDamp(currentWindVelocity.x, windVelocity.x, ref currentWindVelocity.x, 1f / airResponse, Mathf.Infinity, Time.fixedDeltaTime);
            currentWindVelocity.y = Mathf.SmoothDamp(currentWindVelocity.y, windVelocity.y, ref currentWindVelocity.y, 1f / airResponse, Mathf.Infinity, Time.fixedDeltaTime);
            
            Vector2 finalVelocity = targetVelocity + currentWindVelocity;
            
            rigidbody2D.linearVelocity = finalVelocity;
        }
        
        private void ApplyRotation(float targetAngle)
        {
            // Normalize to [-180, 180]
            targetAngle = Mathf.Repeat(targetAngle + 180f, 360f) - 180f;
            
            // Smoothly move toward target angle
            float newAngle = Mathf.SmoothDampAngle(
                rigidbody2D.rotation,
                targetAngle,
                ref rotationVelocity,
                1f / rotationResponse,    // smooth time (higher response = faster turn)
                Mathf.Infinity,
                Time.fixedDeltaTime
            );
            
            rigidbody2D.MoveRotation(newAngle);
        }
        
        private void DetectTrack()
        {
            IsOnTrack = Physics.Raycast(transform.position, transform.forward, float.PositiveInfinity, trackLayer);
            
            if (!IsOnTrack)
            {
                damageAmount += damageRate * Time.fixedDeltaTime;
                damageAmount = Mathf.Clamp01(damageAmount);
                damageText.text = $"{damageAmount * 100:F0}%";
            }
            
            IsCrashed = damageAmount >= 1;
            
            if (IsCrashed)
            {
                OnCrashed?.Invoke();
            }
        }
        
    }
}
