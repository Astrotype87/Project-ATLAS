using TMPro;
using UnityEngine;

namespace ProjectATLAS.Gameplay.UI
{
    public class VariableView : MonoBehaviour
    {
        [Header("Variable")]
        [SerializeField] private new string name;
        [SerializeField] private string variable;
        [SerializeField] private float units = 4;
        [SerializeField] private string suffix;
        [SerializeField, Range(0, 4)] private int rounding = 2;
        
        [Header("Visuals")]
        [SerializeField] private Color color;
        // [SerializeField] private float unitsPerLength = 1;
        
        [Header("Components")]
        [SerializeField] private ArrowView arrowView;
        [SerializeField] private TMP_Text labelText;
        [SerializeField] private TMP_Text valueText;
        
        
        // MONOBEHAVIOUR METHODS
        private void OnValidate()
        {
            UpdateVariable(units, suffix);
        }
        
        
        // PUBLIC METHODS
        public void UpdateVariable(float units, string suffix)
        {
            this.units = units;
            this.suffix = suffix;
            
            if (arrowView)
            {
                arrowView.UpdateArrow(units);
                arrowView.UpdateColor(color);
            }
            
            if (labelText)
            {
                valueText.color = color;
                valueText.text = $"{name} ({variable})";
            }
            
            if (valueText)
            {
                valueText.color = color;
                valueText.text = $"{units.ToString($"F{rounding}")} {suffix}";
            }
        }
    }
}
