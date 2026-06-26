using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectATLAS.Gameplay.UI
{
    using Range = ProjectATLAS.Types.Range;
    
    public class SimParameter : MonoBehaviour
    {
        [Header("Details")]
        [SerializeField] private string label;
        [SerializeField] private string unit;
        
        [Header("Data")]
        [SerializeField] private float value;
        [SerializeField] private Range range;
        [SerializeField] private float snap = 0.5f;
        [SerializeField, Range(0, 4)] private int rounding = 1;
        
        [Header("Components")]
        [SerializeField] private TMP_Text labelText;
        [SerializeField] private TMP_Text unitText;
        [SerializeField] private TMP_InputField valueInput;
        [SerializeField] private Button decreaseButton;
        [SerializeField] private Button increaseButton;
        
        // PROPERTIES
        public string Label => label;
        public float Value => value;
        public event Action<SimParameter> OnValueChanged;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            if (decreaseButton != null)
                decreaseButton.onClick.AddListener(DecreaseButton_onClick);
            if (increaseButton != null)
                increaseButton.onClick.AddListener(IncreaseButton_onClick);
            
            if (valueInput != null)
                valueInput.onEndEdit.AddListener(ValueInput_OnEndEdit);
            
            RefreshUI();
        }
        
        private void OnValidate()
        {
            RefreshUI();
        }
        
        
        
        // PUBLIC METHODS
        public void SetLabel(string label)
        {
            this.label = label;
            if (labelText) labelText.text = label;
        }
        
        public void SetUnit(string unit)
        {
            this.unit = unit;
            if (unitText) unitText.text = unit;
        }
        
        public void SetValue(float newValue)
        {
            // Apply snapping
            if (snap > 0) newValue = Mathf.Round(newValue / snap) * snap;
            
            // Clamp to range
            newValue = Mathf.Clamp(newValue, range.min, range.max);
            
            // Apply rounding
            newValue = (float)Math.Round(newValue, rounding);
            
            // Update value if it changed
            if (!Mathf.Approximately(value, newValue))
            {
                value = newValue;
                OnValueChanged?.Invoke(this);
            }
            
            RefreshUI();
        }
        
        // PRIVATE METHODS
        private void RefreshUI()
        {
            if (labelText != null) labelText.text = label;
            if (unitText != null) unitText.text = unit;
            if (valueInput != null) valueInput.SetTextWithoutNotify(value.ToString($"F{rounding}"));
        }
        
        
        // EVENT LISTENER METHODS
        private void DecreaseButton_onClick()
        {
            SetValue(value - snap);
        }
        private void IncreaseButton_onClick()
        {
            SetValue(value + snap);
        }
        private void ValueInput_OnEndEdit(string input)
        {
            if (float.TryParse(input, out float parsed))
            {
                SetValue(parsed);
            }
            else
            {
                RefreshUI(); // reset to current value if parse fails
            }
        }
    }
}
