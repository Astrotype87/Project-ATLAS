using System;
using UnityEngine;
using TMPro;

namespace ProjectATLAS.Quiz.UI
{
    public class SolvingAnswerView : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private string identifier;
        [SerializeField] private new string name;
        [SerializeField] private double answer;
        [SerializeField] private string unit;
        [SerializeField] [Range(0, 4)] private int rounding = 2;
        
        [Header("Components")]
        [SerializeField] private TMP_Text identifierText;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text valueText;
        [SerializeField] private TMP_InputField answerInput;
        [SerializeField] private TMP_Text placeholderText;
        
        // PROPERTIES
        public event Action<double> OnAnswerChanged;
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            answerInput.onEndEdit.AddListener(AnswerInput_onEndEdit);
            answerInput.onValueChanged.AddListener(AnswerInput_onValueChanged);
        }
        
        private void OnValidate()
        {
            answer = Math.Round(answer, rounding);
            
            if (identifierText) identifierText.text = identifier;
            if (nameText) nameText.text = name;
            if (valueText) valueText.text = $"{answer.ToString($"F{rounding}")} {unit}";
            if (answerInput) answerInput.text = answer.ToString();
            if (placeholderText) placeholderText.text = $"{0.ToString($"F{rounding}")} {unit}";
        }
        
        // PUBLIC METHODS
        public void DisplayAnswer(string identifier, string name, double? answer, string unit, int rounding)
        {
            this.identifier = identifier;
            this.name = name;
            this.answer = answer == null ? 0.0 : Math.Round(answer.Value, rounding);
            this.unit = unit;
            this.rounding = rounding;
            
            if (identifierText) identifierText.text = identifier;
            if (nameText) nameText.text = name;
            if (valueText)
            {
                double num = answer == null ? 0.0 : answer.Value;
                valueText.text = $"{num.ToString($"F{rounding}")} {unit}";
            }
            if (answerInput) answerInput.text = answer == null ? "" : answer.Value.ToString($"F{rounding}");
            if (placeholderText) placeholderText.text = $"{0.ToString($"F{rounding}")}";
        }
        
        // EVENT LISTENER METHODS
        private void AnswerInput_onValueChanged(string endText)
        {
            this.answer = 0;
            try { this.answer = Math.Round(double.Parse(endText), rounding); }
            catch { }
            
            if (valueText) valueText.text = $"{answer.ToString($"F{rounding}")} {unit}";
        }
        
        private void AnswerInput_onEndEdit(string endText)
        {
            this.answer = 0;
            try { this.answer = Math.Round(double.Parse(endText), rounding); }
            catch { }
            
            OnAnswerChanged?.Invoke(this.answer);
            
            if (answerInput) answerInput.text = this.answer.ToString($"F{rounding}");
            if (valueText) valueText.text = $"{answer.ToString($"F{rounding}")} {unit}";
        }
    }
}
