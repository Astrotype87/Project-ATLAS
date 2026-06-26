using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KBCore.Refs;
using CustomInspector;

namespace ProjectATLAS.Calculator
{
    [RequireComponent(typeof(Button))]
    public class CalculatorKey : MonoBehaviour
    {
        [Header("Modes")]
        [SerializeField] private bool isShiftable;
        [SerializeField] private bool isAltable;
        
        [Header("Keys")]
        [SerializeField] private KeyInfo mainKey;
        [SerializeField, ShowIf(nameof(isShiftable))] private KeyInfo shiftKey;
        [SerializeField, ShowIf(nameof(isAltable))] private KeyInfo altKey;
        [SerializeField, ShowIf(nameof(IsShiftAltable))] private KeyInfo shiftAltKey;
        
        [Header("Display")]
        [SerializeField] private float fontSize;
        
        [Header("Components")]
        [SerializeField, Child] private TMP_Text labelComponent;
        [SerializeField, Self] private Button button;
        
        
        // PROPERTIES
        public KeyInfo MainKey => mainKey;
        public KeyInfo ShiftKey => shiftKey;
        public KeyInfo AltKey => altKey;
        public KeyInfo ShiftAltKey => shiftAltKey;
        
        public bool IsShiftable => isShiftable;
        public bool IsAltable => isAltable;
        public bool IsShiftAltable() => isShiftable && isAltable;
        
        private bool isShifted;
        private bool isAlted;
        private KeyInfo currentKey;
        
        public event Action<KeyInfo> OnPressed;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            UpdateCurrentKey();
            
            button.onClick.AddListener(Button_onClick);
        }
        
        private void OnValidate()
        {
            gameObject.name = $"Key {mainKey.label}".Trim();
            
            if (labelComponent)
            {
                labelComponent.text = mainKey.label;
                labelComponent.fontSize = fontSize;
            }
        }
        
        
        // PUBLIC METHODS
        public void SetShift(bool isShifted)
        {
            if (this.isShifted == isShifted) return;
            this.isShifted = isShifted;
            
            UpdateCurrentKey();
        }
        
        public void SetAlt(bool isAlted)
        {
            if (this.isAlted == isAlted) return;
            this.isAlted = isAlted;
            
            UpdateCurrentKey();
        }
        
        public List<string> GetKeywords()
        {
            List<string> keywords = new() {mainKey.value};
            
            if (isShiftable) keywords.Add(shiftKey.value);
            if (isAltable) keywords.Add(altKey.value);
            if (isShiftable && isAltable) keywords.Add(shiftAltKey.value);
            
            return keywords;
        }
        
        
        // PRIVATE METHODS
        private void UpdateCurrentKey()
        {
            if (isShiftable && isAltable && isShifted && isAlted)
            {
                currentKey = shiftAltKey;
            }
            else if (isShiftable && isShifted)
            {
                currentKey = shiftKey;
            }
            else if (isAltable && isAlted)
            {
                currentKey = altKey;
            }
            else
            {
                currentKey = mainKey;
            }
            
            labelComponent.text = currentKey.label;
        }
        
        
        // EVENT LISTENER METHODS
        private void Button_onClick()
        {
            OnPressed?.Invoke(currentKey);
        }
    }
}
