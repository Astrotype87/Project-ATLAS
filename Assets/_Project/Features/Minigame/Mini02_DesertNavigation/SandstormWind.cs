using System.Collections;
using ProjectATLAS.Gameplay;
using ProjectATLAS.Types;
using TMPro;
using UnityEngine;

namespace ProjectATLAS.Minigame.Mini02_DesertNavigation
{
    public class SandstormWind : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Vector2 windVelocity;
        [SerializeField] private float smoothTime = 0.5f;
        [SerializeField] private Range easyValue = new(1, 2);
        [SerializeField] private Range mediumValue = new(2, 4);
        [SerializeField] private Range hardValue = new(3, 8);
        [SerializeField] private float windTime = 10f;
        [SerializeField] private float restTime = 5f;
        
        [Header("Particle")]
        [SerializeField] private float particleSpeed;
        [SerializeField] private float randomParticleSpeedRange;
        [SerializeField, Range(0, 1)] private float maxAlpha;
        [SerializeField] private float maxAlphaAtSpeed;
        
        [Header("Components")]
        [SerializeField] private KinematicHovercraftPhysics kinematicHovercraftPhysics;
        [SerializeField] private ParticleSystem windParticle;
        
        [Header("Text")]
        [SerializeField] private TMP_Text windText;
        [SerializeField] private float blinkTime = 0.125f;
        [SerializeField] private float displayTime = 2f;
        
        private Vector2 currentWindVelocity;
        private Vector2 windVelocitySmoothDampVelocity;
        
        private Difficulty difficulty;
        
        
        // MONOBEHAVIOUR METHODS
        private void Start()
        {
            windText.gameObject.SetActive(false);
        }
        
        private void Update()
        {
            // Smoothly blend current velocity toward target windVelocity
            currentWindVelocity = Vector2.SmoothDamp(
                currentWindVelocity,
                windVelocity,
                ref windVelocitySmoothDampVelocity,
                smoothTime
            );
            
            // Apply smoothed wind to hovercraft physics
            if (kinematicHovercraftPhysics)
                kinematicHovercraftPhysics.SetWindVelocity(currentWindVelocity);
        }
        
        private void LateUpdate()
        {
            UpdateSandParticleEffects();
        }
        
        private void OnValidate()
        {
            UpdateSandParticleEffects();
        }
        
        
        
        // PUBLIC METHODS
        public void SetDifficulty(Difficulty difficulty)
        {
            this.difficulty = difficulty;
        }
        
        public void StartSandstormSequence()
        {
            StartCoroutine(SandstormSequence());
        }
        
        
        // PRIVATE METHODS
        /// <summary> Sandstorm sequence with random wind velocity. </summary>
        private IEnumerator SandstormSequence()
        {
            Range range = difficulty switch
            {
                Difficulty.Easy => easyValue,
                Difficulty.Medium => mediumValue,
                Difficulty.Hard => hardValue,
                _ => easyValue
            };
            
            while (true)
            {
                yield return windVelocity = Vector2.zero;
                yield return new WaitForSeconds(restTime);
                
                StartCoroutine(DisplayWindApproachingText());
                float xRand = range.GetRandom() * RandSign();
                float yRand = range.GetRandom() * RandSign();
                yield return windVelocity = new(xRand, yRand);
                yield return new WaitForSeconds(windTime);
            }
            
            static float RandSign() => Random.value < 0.5f ? -1 : 1;
        }
        
        /// <summary> Blinking text animation. </summary>
        private IEnumerator DisplayWindApproachingText()
        {
            windText.gameObject.SetActive(true);
            yield return new WaitForSeconds(blinkTime);
            
            windText.gameObject.SetActive(false);
            yield return new WaitForSeconds(blinkTime);
            
            windText.gameObject.SetActive(true);
            yield return new WaitForSeconds(blinkTime);
            
            windText.gameObject.SetActive(false);
            yield return new WaitForSeconds(blinkTime);
            
            windText.gameObject.SetActive(true);
            yield return new WaitForSeconds(displayTime);
            
            windText.gameObject.SetActive(false);
        }
        
        private void UpdateSandParticleEffects()
        {
            if (!windParticle) return;
            
            // Particle velocity
            Vector2 particleVelocity = currentWindVelocity * particleSpeed;
            
            var velocityOverLifetime = windParticle.velocityOverLifetime;
            velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(
                particleVelocity.x - randomParticleSpeedRange,
                particleVelocity.x + randomParticleSpeedRange
            );
            velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(
                particleVelocity.y - randomParticleSpeedRange,
                particleVelocity.y + randomParticleSpeedRange
            );
            velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(
                -randomParticleSpeedRange,
                randomParticleSpeedRange
            );
            
            // Particle transparency
            float appliedSpeed = Mathf.Clamp01(Mathf.Abs(windVelocity.magnitude / maxAlphaAtSpeed));
            float appliedAlpha = Mathf.Lerp(0, maxAlpha, appliedSpeed);
            
            var main = windParticle.main;
            var startColor = main.startColor;
            Color color = startColor.color;
            color.a = appliedAlpha;
            main.startColor = new ParticleSystem.MinMaxGradient(color);
        }
    }
}
