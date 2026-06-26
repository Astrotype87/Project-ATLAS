using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KBCore.Refs;

namespace ProjectATLAS.Gameplay.UI
{
    public class ArrowView : MonoBehaviour
    {
        [Header("Main")]
        [SerializeField] private float units = 4;
        [SerializeField] private string prefix;
        [SerializeField] private string suffix;
        [SerializeField, Range(0, 4)] private int rounding = 2;
        [SerializeField] private bool comma;
        [SerializeField] private Color color = Color.white;
        
        [Header("Settings")]
        [SerializeField] private float unitsPerLength = 1;
        [SerializeField] private float lineWidth = 0.25f;
        [SerializeField, Range(-180, 180)] private float rotation = 90;
        [SerializeField] private Vector2 headSize = Vector2.one;
        
        [Header("References")]
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private RectTransform childTransform;
        [SerializeField] private TMP_Text valueText;
        [SerializeField, Child] private Graphic[] graphics;
        
        private float lastUnitsSign;
        
        
        // MONOBEHAVIOUR METHODS
        private void OnValidate()
        {
            this.ValidateRefs();
            
            UpdateArrow(units);
            UpdateColor(color);
        }
        
        
        // PUBLIC METHODS
        public void UpdateArrow(float units)
        {
            this.units = units;
            
            if (rectTransform)
            {
                // Update arrow length
                float lineLength = Mathf.Abs(units) / unitsPerLength;
                rectTransform.sizeDelta = new(lineWidth, lineLength);
                
                // Rotate arrow 180 degrees if unit sign flips
                float currentSign = Mathf.Sign(units);
                if (currentSign != lastUnitsSign && (currentSign == -1 || lastUnitsSign == -1))
                    rotation = WrapAngleSigned180(rotation + 180);
                lastUnitsSign = currentSign;
                
                rectTransform.localEulerAngles = new (0, 0, -rotation);
            }
            
            // Update head size
            if (childTransform) childTransform.sizeDelta = headSize;
            
            // Update text
            if (valueText)
            {
                string prefixString = string.IsNullOrWhiteSpace(prefix) ? "" : prefix;
                string suffixString = string.IsNullOrWhiteSpace(suffix) ? "" : suffix;
                valueText.text = $"{prefixString}{units.ToString(comma ? $"N{rounding}" : $"F{rounding}")}{suffixString}";
            }
        }
        
        public void UpdateArrow(float units, float rotation)
        {
            this.rotation = rotation;
            
            UpdateArrow(units);
        }
        
        public void UpdateColor(Color color)
        {
            this.color = color;
            
            foreach (var graphic in graphics)
            {
                graphic.color = color;
            }
            
            if (valueText) valueText.color = color;
        }
        
        
        // PRIVATE METHODS
        private static float WrapAngleSigned180(float angle)
        {
            angle %= 360f; // keep between -360 and 360
            if (angle > 180f) angle -= 360f;
            else if (angle < -180f) angle += 360f;
            return angle;
        }
        
        private static float WrapAngle360(float angle)
        {
            angle %= 360f;
            if (angle < 0f) angle += 360f;
            return angle;
        }
        
    }
}
