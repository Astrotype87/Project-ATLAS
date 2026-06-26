using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectATLAS.Simulation.Sim01_PartsMeasurements
{
    /// <summary> Input field with label and unit display. </summary>
    public class UnitField : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField] private string labelText;
        [SerializeField] private float inputText;
        [SerializeField] private string unitText;
        [SerializeField] [Range(0, 4)] private int rounding = 1;
        [SerializeField] private bool isHighlighted;
        
        [Header("Visuals")]
        [SerializeReference] private Color normalColor;
        [SerializeReference] private Color highlightedColor;
        
        [Header("Components")]
        [SerializeField] private TMP_Text labelComponent;
        [SerializeField] private TMP_InputField inputComponent;
        [SerializeField] private TMP_Text unitComponent;
        [SerializeField] private Image background; // Add reference for highlight
        
        
        // PROPERTIES
        public string LabelText
        {
            get => labelComponent ? labelComponent.text : "";
            set
            {
                labelText = value;
                if (labelComponent) labelComponent.text = value;
            }
        }
        
        public float InputText
        {
            get
            {
                if (inputComponent && float.TryParse(inputComponent.text, out float parsed))
                    return parsed;
                return 0f;
            }
            set
            {
                // Round value to the specified decimal places
                float rounded = (float)Math.Round(value, rounding);
                inputText = rounded;

                if (inputComponent) 
                    inputComponent.text = rounded.ToString($"F{rounding}");
            }
        }
        
        public string UnitText
        {
            get => unitComponent ? unitComponent.text : "";
            set
            {
                unitText = value;
                if (unitComponent) unitComponent.text = value;
            }
        }
        
        // PUBLIC METHODS
        public void SetHighlight(bool isHighlighted)
        {
            this.isHighlighted = isHighlighted;
            if (background) background.color = isHighlighted ? highlightedColor : normalColor;
        }
        
        public void ResetHighlight()
        {
            SetHighlight(false);
        }
        
        
        // MONOBEHAVIOUR METHODS
        private void OnValidate()
        {
            if (labelComponent) labelComponent.text = labelText;
            if (inputComponent) inputComponent.text = inputText.ToString();
            if (unitComponent) unitComponent.text = unitText;
            if (background) background.color = isHighlighted ? highlightedColor : normalColor;
        }
    }
}
