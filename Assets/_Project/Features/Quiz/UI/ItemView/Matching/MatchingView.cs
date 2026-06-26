using System;
using System.Linq;
using UnityEngine;

namespace ProjectATLAS.Quiz.UI
{
    public class MatchingView : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private MatchingQuestionsPanel matchingQuestionsPanel;
        [SerializeField] private MatchingAnswersPanel matchingAnswersPanel;
        
        private string[] answers;
        
        // PROPERTIES
        public event Action<string[]> OnAnswerUpdated;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            matchingQuestionsPanel.OnAnswerDropped += MatchingQuestionsPanel_OnAnswerDropped;
            matchingAnswersPanel.OnContainedAnswersChanged += MatchingAnswersPanel_OnContainedAnswersChanged;
        }
        
        
        // PUBLIC METHODS
        public void DisplayItem(MatchingItem matchingItem, MatchingAnswer matchingAnswer)
        {
            // Initialize answers variable
            string[] questions = matchingItem.GetQuestions();
            string[] choices = matchingItem.GetChoices();
            string[] answers;
            
            if (matchingAnswer == null || matchingAnswer.answers == null || matchingAnswer.answers.Length == 0)
                answers = new string[questions.Length];
            else
                answers = matchingAnswer.answers;
            
            // Display questions
            matchingAnswersPanel.DisplayAnswers(choices);
            matchingQuestionsPanel.DisplayQuestions(questions);
            
            // Assign answers
            for (int i = 0; i < answers.Length; i++)
            {
                MatchingAnswerView answerView = matchingAnswersPanel.GetAnswerView(answers[i]);
                if (answerView == null) continue;
                
                matchingQuestionsPanel.AssignAnswer(answerView, i);
            }
            
            this.answers = answers;
        }
        
        
        // EVENT LISTENER METHODS
        private void MatchingQuestionsPanel_OnAnswerDropped(int index, string answer)
        {
            // Debug.Log($"index: {index}, answer: {answer}");
            // answers[index] = answer;
            
            answers = matchingQuestionsPanel.GetAssignedAnswers();
            OnAnswerUpdated?.Invoke(answers);
        }
        
        private void MatchingAnswersPanel_OnContainedAnswersChanged()
        {
            answers = matchingQuestionsPanel.GetAssignedAnswers();
            OnAnswerUpdated?.Invoke(answers);
        }
    }
}
