using System;
using UnityEngine;

using ProjectATLAS.UI;
using TMPro;

namespace ProjectATLAS.Quiz.UI
{
    public class FillInTheBlanksView : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private TMP_Text questionText;
        [SerializeField] private FillInTheBlanksDropDownsPanel fillInTheBlanksDropDownsPanel;
        [SerializeField] private FillInTheBlanksChoicePanel fillInTheBlanksChoicePanel;
        
        private string[] answers;
        
        // PROPERTIES
        public event Action<string[]> OnAnswerUpdated;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            fillInTheBlanksDropDownsPanel.OnChoiceDropped += FillInTheBlanksDropDownsPanel_OnChoiceDropped;
            fillInTheBlanksChoicePanel.OnContainedChoicesChanged += FillInTheBlanksChoicePanel_OnContainedChoicesChanged;
        }
        
        // PUBLIC METHODS
        public void DisplayItem(FillInTheBlanksItem fillInTheBlanksItem, FillInTheBlanksAnswer fillInTheBlanksAnswer)
        {
            // Initialize answers variable
            int blankCount = fillInTheBlanksItem.GetCorrectAnswers().Length;
            string question = fillInTheBlanksItem.GetQuestion();
            string[] choices = fillInTheBlanksItem.GetChoices();
            string[] answers;
            
            if (fillInTheBlanksAnswer == null || fillInTheBlanksAnswer.answers == null || fillInTheBlanksAnswer.answers.Length == 0)
                answers = new string[choices.Length];
            else
                answers = fillInTheBlanksAnswer.answers;
            
            // Display dropdowns and choices
            questionText.SetText(question);
            fillInTheBlanksChoicePanel.DisplayChoices(choices);
            fillInTheBlanksDropDownsPanel.DisplayDropDowns(blankCount);
            
            // Assign answers
            for (int i = 0; i < answers.Length; i++)
            {
                FillInTheBlanksChoiceView choiceView = fillInTheBlanksChoicePanel.GetChoiceView(answers[i]);
                if (choiceView == null) continue;
                
                fillInTheBlanksDropDownsPanel.AssignAnswer(choiceView, i);
            }
            
            this.answers = answers;
        }
        
        
        // EVENT LISTENER METHODS
        private void FillInTheBlanksDropDownsPanel_OnChoiceDropped(int index, string choice)
        {
            // Debug.Log($"index: {index}, answer: {answer}");
            // answers[index] = answer;
            
            answers = fillInTheBlanksDropDownsPanel.GetAssignedChoices();
            OnAnswerUpdated?.Invoke(answers);
        }
        
        private void FillInTheBlanksChoicePanel_OnContainedChoicesChanged()
        {
            answers = fillInTheBlanksDropDownsPanel.GetAssignedChoices();
            OnAnswerUpdated?.Invoke(answers);
        }
    }
}
