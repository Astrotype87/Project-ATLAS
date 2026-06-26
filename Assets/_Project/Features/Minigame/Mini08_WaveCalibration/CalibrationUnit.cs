using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectATLAS
{
    public class CalibrationUnit : MonoBehaviour
    {
        [Header("Number")]
        [SerializeField] private bool isEnabled;
        [SerializeField, Range(0, 1)] private float alphaDisabled;
        [SerializeField] private int number;
        [SerializeField] private bool isChecked;
        
        [Header("Targets")]
        [SerializeField] private float frequency;
        [SerializeField] private float amplitude;
        [SerializeField] private float phase;
        
        [Header("Components")]
        [SerializeField] private Image visibilityImage;
        [SerializeField] private TMP_Text numberText;
        [SerializeField] private Image checkImage;
        [SerializeField] private TMP_Text frequencyText;
        [SerializeField] private TMP_Text amplitudeText;
        [SerializeField] private TMP_Text phaseText;
        
        
        // MONOBEHAVIOUR METHODS
        private void OnValidate()
        {
            SetNumber(number);
            SetFrequency(frequency);
            SetAmplitude(amplitude);
            SetPhase(phase);
            
            SetEnable(isEnabled);
            SetCheck(isChecked);
        }
        
        
        // PUBLIC METHODS
        public void SetEnable(bool isEnabled)
        {
            this.isEnabled = isEnabled;
            if (visibilityImage)
            {
                Color color = visibilityImage.color;
                color.a = isEnabled ? 0.0f : 1.0f - alphaDisabled;
                visibilityImage.color = color;
            }
        }
        
        public void SetCheck(bool isChecked)
        {
            this.isChecked = isChecked;
            if (checkImage) checkImage.gameObject.SetActive(isChecked);
        }
        
        public void SetNumber(int number)
        {
            this.number = number;
            if (numberText) numberText.text = $"#{number:00}";
        }
        
        public void SetFrequency(float frequency)
        {
            this.frequency = frequency;
            if (frequencyText) frequencyText.text = $"{frequency} Hz";
        }
        
        public void SetAmplitude(float amplitude)
        {
            this.amplitude = amplitude;
            if (amplitudeText) amplitudeText.text = $"{amplitude}";
        }
        
        public void SetPhase(float phase)
        {
            this.phase = phase;
            if (phaseText) phaseText.text = $"{phase} °";
        }
        
        
        
        public bool IsFrequencyCorrect(float frequency)
        {
            return Mathf.Approximately(this.frequency, frequency); 
        }
        
        public bool IsAmplitudeCorrect(float amplitude)
        {
            return Mathf.Approximately(this.amplitude, amplitude); 
        }
        
        public bool IsPhaseCorrect(float phase)
        {
            return Mathf.Approximately(this.phase, phase); 
        }
        
    }
}
