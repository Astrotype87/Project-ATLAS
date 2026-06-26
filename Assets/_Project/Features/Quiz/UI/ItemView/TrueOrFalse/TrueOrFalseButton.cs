using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectATLAS.Quiz.UI
{
    [Serializable]
    public class TrueOrFalseButton : MonoBehaviour
    {
        [Header("State")]
        [SerializeField] private bool isSelected;
        [SerializeField] private string text;
        [SerializeField] private bool isTrue;
        
        [Header("Style")]
        [SerializeField] private ButtonStyle normalStyle;
        [SerializeField] private ButtonStyle selectedStyle;
        
        [Header("Components")]
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text textComponent;
        [SerializeField] private Image backgroundComponent;
        
        public event Action OnClicked;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            if (button) button.onClick.AddListener(Button_onClick);
        }
        
        private void OnValidate()
        {
            if (textComponent) textComponent.text = text;
            
            SetButtonStyle(isSelected ? selectedStyle : normalStyle);
        }
        
        
        // PUBLIC METHODS
        public void SetSelected(bool isSelected)
        {
            this.isSelected = isSelected;
            SetButtonStyle(isSelected ? selectedStyle : normalStyle);
        }
        
        // PRIVATE METHODS
        private void SetButtonStyle(ButtonStyle style)
        {
            if (textComponent) textComponent.color = style.textColor;
            if (backgroundComponent) backgroundComponent.color = style.backgroundColor;
        }
        
        // EVENT LISTENER METHODS
        private void Button_onClick()
        {
            OnClicked?.Invoke();
            SetSelected(true);
        }
        
        
        [Serializable]
        public struct ButtonStyle
        {
            public Color backgroundColor;
            public Color textColor;
        }
    }
}
