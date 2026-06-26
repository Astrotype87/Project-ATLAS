using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectATLAS.Lesson
{
    using Range = ProjectATLAS.Types.Range;
    
    public class VariableSlider : MonoBehaviour
    {
        [Header("Details")]
        [SerializeField] private string display;
        [SerializeField] private string variable;
        [SerializeField] private string unit;
        [SerializeField] private Color color = Color.white;
        
        [SerializeField] private double value = 0.0;
        [SerializeField] private Range range = new(0, 10);
        [SerializeField] private float snap = 0.5f;
        [SerializeField, Range(0, 4)] private int rounding = 2;
        
        [Header("Components")]
        [SerializeField] private TMP_Text variableText;
        [SerializeField] private TMP_Text rangeText;
        [SerializeField] private Slider slider;
        
        public string Display => display;
        public string Variable => variable;
        public double Value => value;
        public Color Color => color;
        public Range Range => range;
        public float Snap => snap;
        
        public event Action<string, double> OnSliderUpdated;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            UpdateSlider();
            UpdateText();
            
            slider.wholeNumbers = false;
            slider.onValueChanged.AddListener(Slider_OnSliderChanged);
        }
        
        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                // Clamp and round while editing in the Inspector
                value = Math.Round(Math.Clamp(value, range.min, range.max), rounding);
                
                if (slider != null)
                {
                    slider.minValue = range.min;
                    slider.maxValue = range.max;
                    
                    float snapped = Mathf.Round((float)value / snap) * snap;
                    snapped = Mathf.Clamp(snapped, range.min, range.max);
                    slider.SetValueWithoutNotify(snapped);
                }
                
                UpdateText();
            }
            
            if (snap <= 0f) snap = 0.1f;
        }
        
        
        // PUBLIC METHODS
        public void SetValue(double newValue)
        {
            newValue = Math.Clamp(newValue, range.min, range.max);
            value = Math.Round(newValue, rounding);
            
            if (slider)
                slider.SetValueWithoutNotify((float)value);
            
            UpdateVariableText();
        }
        
        
        // PRIVATE METHODS
        private void UpdateSlider()
        {
            if (slider)
            {
                slider.minValue = range.min;
                slider.maxValue = range.max;
                slider.SetValueWithoutNotify(Mathf.Clamp((float)value, range.min, range.max));
            }
        }
        
        private void UpdateText()
        {
            UpdateVariableText();
            
            if (rangeText != null)
            {
                rangeText.text = $"({FormatValue(range.min)} to {FormatValue(range.max)})";
                rangeText.color = color;
            }
        }
        
        private void UpdateVariableText()
        {
            if (variableText != null)
            {
                variableText.text = $"{display} = {value} {unit}";
                variableText.color = color;
            }
        }
        
        private string FormatValue(double v)
        {
            return Math.Round(v, rounding).ToString($"F{rounding}");
        }
        
        
        // EVENT LISTENER METHODS
        private void Slider_OnSliderChanged(float newValue)
        {
            // Snap and clamp value
            float snappedFloat = Mathf.Round(newValue / snap) * snap;
            snappedFloat = Mathf.Clamp(snappedFloat, range.min, range.max);
            
            // Ensure slider visually snaps
            slider.SetValueWithoutNotify(snappedFloat);
            
            value = Math.Round(snappedFloat, rounding);
            
            OnSliderUpdated?.Invoke(variable, value);
            
            UpdateVariableText();
        }
    }
}
