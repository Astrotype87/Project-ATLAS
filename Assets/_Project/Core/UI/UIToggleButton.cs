using System;
using UnityEngine;
using UnityEngine.UI;
using KBCore.Refs;
using TMPro;

namespace ProjectATLAS.UI
{
    public class UIToggleButton : MonoBehaviour
    {
        public Style onStyle;
        public Style offStyle;
        public bool isOn;
        
        [Header("Components")]
        [SerializeField, Self] private Button buttonComponent;
        [SerializeField, Self(Flag.Optional | Flag.Editable)] private Image buttonImage;
        [SerializeField, Child(Flag.Optional | Flag.Editable)] private TMP_Text textComponent;
        [SerializeField, Child(Flag.Optional | Flag.ExcludeSelf | Flag.Editable)] private Image iconImage;
        
        public bool IsOn => isOn;
        public event Action<UIToggleButton, bool> OnValueChanged;
        
        // CONSTRUCTOR
        public UIToggleButton()
        {
            onStyle = new Style(Color.white, Color.black, Color.black, "On", default, default);
            offStyle = new Style(Color.white, Color.black, Color.black, "Off", default, default);
        }
        
        
        // MONOBEHAVIOUR METHODS
        private void Start()
        {
            buttonComponent.onClick.AddListener(OnToggle);
        }
        
        private void OnValidate()
        {
            this.ValidateRefs();
            RefreshStyle();
        }
        
        // PUBLIC METHODS
        public void Toggle()
        {
            SetValue(!isOn);
        }
        
        public void SetValue(bool isOn)
        {
            bool hasValueChanged = this.isOn != isOn;
            
            this.isOn = isOn;
            RefreshStyle();
            if (hasValueChanged) OnValueChanged?.Invoke(this, isOn);
        }
        
        public void SetValueIsolated(bool isOn)
        {
            this.isOn = isOn;
            RefreshStyle();
        }
        
        public void RefreshStyle()
        {
            SetStyle(isOn ? onStyle : offStyle);
        }
        
        
        
        // PRIVATE METHODS
        private void OnToggle()
        {
            isOn = !isOn;
            OnValueChanged?.Invoke(this, isOn);
            RefreshStyle();
        }
        
        private void SetStyle(Style style)
        {
            if (buttonImage)
            {
                buttonImage.sprite = style.buttonSprite;
                buttonImage.color = style.buttonColor;
            }
            if (iconImage)
            {
                iconImage.sprite = style.iconSprite;
                iconImage.color = style.iconColor;
            }
            if (textComponent)
            {
                textComponent.text = style.text;
                textComponent.color = style.textColor;
            }
        }
        
        
        // STRUCTS
        [Serializable]
        public struct Style
        {
            public Color buttonColor;
            public Color iconColor;
            public Color textColor;
            public string text;
            public Sprite buttonSprite;
            public Sprite iconSprite;
            
            public Style(Color buttonColor, Color iconColor, Color textColor, string text, Sprite buttonSprite, Sprite iconSprite)
            {
                this.buttonColor = buttonColor;
                this.iconColor = iconColor;
                this.textColor = textColor;
                this.text = text;
                this.buttonSprite = buttonSprite;
                this.iconSprite = iconSprite;
            }
        }
    }
}
