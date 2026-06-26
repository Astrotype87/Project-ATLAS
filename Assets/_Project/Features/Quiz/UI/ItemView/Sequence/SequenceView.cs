using System;
using UnityEngine;

using ProjectATLAS.UI;
using TMPro;

namespace ProjectATLAS.Quiz.UI
{
    public class SequenceView : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private TMP_Text questionText;
        [SerializeField] private SequenceAnswersPanel sequenceAnswersPanel;
        
        // PROPERTIES
        public event Action<string[]> OnAnswerUpdated;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            sequenceAnswersPanel.OnContainedAnswersChanged += SequenceAnswersPanel_OnContainedAnswersChanged;
        }
        
        // PUBLIC METHODS
        public void DisplayItem(SequenceItem sequenceItem, SequenceAnswer sequenceAnswer)
        {
            // Initialize answers variable
            string question = sequenceItem.GetQuestion();
            string[] answers;
            
            if (sequenceAnswer == null || sequenceAnswer.answers == null || sequenceAnswer.answers.Length == 0)
                answers = sequenceItem.GetShuffledAnswers();
            else
                answers = sequenceAnswer.answers;
            
            // Display dropdowns and answers
            questionText.SetText(question);
            sequenceAnswersPanel.DisplayAnswers(answers);
            
            OnAnswerUpdated?.Invoke(answers);
        }
        
        // EVENT LISTENER METHODS
        private void SequenceAnswersPanel_OnContainedAnswersChanged(string[] answers)
        {
            OnAnswerUpdated?.Invoke(answers);
        }
    }
}
