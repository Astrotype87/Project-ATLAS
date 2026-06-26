using System;
using UnityEngine;
using TMPro;

namespace ProjectATLAS.Quiz.UI
{
    public class SolvingView : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField, TextArea] private string question;
        
        [Header("Components")]
        [SerializeField] private TMP_Text questionText;
        [SerializeField] private SolvingVariablesPanel variablesPanel;
        [SerializeField] private SolvingAnswerView answerView;
        
        // PROPERTIES
        public event Action<double> OnAnswerUpdated;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            answerView.OnAnswerChanged += AnswerView_OnAnswerChanged;
        }
        
        private void OnValidate()
        {
            if (questionText) questionText.text = question;
        }
        
        
        // PUBLIC METHODS
        public void DisplayItem(SolvingItem solvingItem, SolvingAnswer solvingAnswer)
        {
            // Display question
            if (questionText) questionText.text = solvingItem.GetQuestion();
            
            // Display variables
            variablesPanel.DisplayVariables(solvingItem.GetVariables().ToArray(), solvingItem.Rounding);
            
            // Display answer
            SolvingItem item = solvingItem;
            double? answer = solvingAnswer?.answer;
            answerView.DisplayAnswer(item.Variable, item.Name, answer, item.Unit, item.Rounding);
        }
        
        
        // EVENT LISTENER METHODS
        public void AnswerView_OnAnswerChanged(double answer)
        {
            OnAnswerUpdated?.Invoke(answer);
        }
    }
}
