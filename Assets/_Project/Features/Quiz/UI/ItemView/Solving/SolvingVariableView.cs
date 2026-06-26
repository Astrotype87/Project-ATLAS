using UnityEngine;
using TMPro;
using System;

namespace ProjectATLAS.Quiz.UI
{
    public class SolvingVariableView : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private string identifier;
        [SerializeField] private new string name;
        [SerializeField] private double value;
        [SerializeField] private string unit;
        [SerializeField] [Range(0, 4)] private int rounding = 2;
        
        [Header("Components")]
        [SerializeField] private TMP_Text identifierText;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text valueText;
        [SerializeField] private CanvasGroup canvasGroup;
        
        // MONOBEHAVIOUR METHODS
        private void OnValidate()
        {
            value = Math.Round(value, rounding);
            
            if (identifierText) identifierText.text = identifier;
            if (nameText) nameText.text = name;
            if (valueText) valueText.text = $"{value.ToString($"F{rounding}")} {unit}";
        }
        
        // PUBLIC METHODS
        public void DisplayVariable(string identifier, string name, double value, string unit, int rounding)
        {
            this.identifier = identifier;
            this.name = name;
            this.value = Math.Round(value, rounding);
            this.unit = unit;
            this.rounding = rounding;
            
            if (identifierText) identifierText.text = identifier;
            if (nameText) nameText.text = name;
            if (valueText) valueText.text = $"{value.ToString($"F{rounding}")} {unit}";
        }
        
        public void SetVisible(bool isVisible) => canvasGroup.alpha = isVisible ? 1f : 0f;
    }
}
