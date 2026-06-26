using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectATLAS.Quiz.UI
{
    public class MultipleChoiceAnswerButton : MonoBehaviour
    {
        [Header("State")]
        [SerializeField] private bool isSelected;
        [SerializeField] private string letter;
        [SerializeField, TextArea] private string answer;
        
        [Header("Style")]
        [SerializeField] private ButtonStyle normalStyle;
        [SerializeField] private ButtonStyle selectedStyle;
        
        [Header("Components")]
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text letterComponent;
        [SerializeField] private TMP_Text answerComponent;
        [SerializeField] private Image backgroundComponent;
        
        public string Answer => answer;
        public event Action<string> OnClicked;
        
        
        private void Awake()
        {
            if (button) button.onClick.AddListener(Button_onClick);
        }
        
        private void OnValidate()
        {
            gameObject.name = string.IsNullOrWhiteSpace(letter) ? "AnswerButton" : letter;
            
            if (letterComponent) letterComponent.text = letter;
            if (answerComponent) answerComponent.text = answer;
            
            SetButtonStyle(isSelected ? selectedStyle : normalStyle);
        }
        
        
        // PUBLIC METHODS
        /// <summary> Set answer display with letter and answer. (ex: A. Acceleration) </summary>
        public void SetAnswerDisplay(string letter, string answer)
        {
            this.letter = letter;
            this.answer = answer;
            
            if (letterComponent) letterComponent.text = letter;
            if (answerComponent) answerComponent.text = answer;
        }
        
        /// <summary> Set answer as selected or not. </summary>
        public void SetSelected(bool isSelected)
        {
            this.isSelected = isSelected;
            SetButtonStyle(isSelected ? selectedStyle : normalStyle);
        }
        
        
        // PRIVATE METHODS
        private void SetButtonStyle(ButtonStyle style)
        {
            if (letterComponent) letterComponent.color = style.textColor;
            if (answerComponent) answerComponent.color = style.textColor;
            if (backgroundComponent) backgroundComponent.color = style.backgroundColor;
        }
        
        private void Button_onClick()
        {
            OnClicked?.Invoke(answer);
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
