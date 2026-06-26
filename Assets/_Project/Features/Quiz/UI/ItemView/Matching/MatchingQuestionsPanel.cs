using System;
using System.Collections.Generic;
using UnityEngine;
using KBCore.Refs;
using UnityEngine.UI;

namespace ProjectATLAS.Quiz.UI
{
    public class MatchingQuestionsPanel : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField, Child] private List<MatchingQuestionView> questionViews;
        [SerializeField, Self] private VerticalLayoutGroup verticalLayoutGroup;
        
        public event Action<int, string> OnAnswerDropped;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            for (int i = 0; i < questionViews.Count; i++)
            {
                questionViews[i].OnAnswerDropped += QuestionView_OnAnswerDropped;
            }
        }
        
        
        // PUBLIC METHODS
        public void DisplayQuestions(string[] questions)
        {
            // Handle answer button duplication logic
            int targetChildCount = Mathf.FloorToInt(questions.Length);
            int interval = targetChildCount - questionViews.Count;
            
            if (interval > 0)
            {
                for (int i = 0; i < interval; i++)
                {
                    AddQuestionView();
                }
            }
            else if (interval < 0)
            {
                interval = -interval; // flip the sign
                for (int i = 0; i < interval; i++)
                {
                    RemoveQuestionView();
                }
            }
            
            // Update question view text
            for (int i = 0; i < questionViews.Count; i++)
            {
                questionViews[i].DisplayQuestion($"{i + 1}. {questions[i]}");
            }
        }
        
        public string[] GetAssignedAnswers()
        {
            string[] answers = new string[questionViews.Count];
            for (int i = 0; i < questionViews.Count; i++)
            {
                answers[i] = questionViews[i].GetAssignedAnswer();
            }
            return answers;
        }
        
        public void AssignAnswer(MatchingAnswerView answerView, int index)
        {
            if (index >= questionViews.Count)
            {
                Debug.LogWarning("[MatchingQuestionList] Tried to assign a MatchingAnswer but index out of range.");
                return;
            }
            
            questionViews[index].AssignAnswer(answerView);
        }
        
        
        // PRIVATE METHODS
        private void AddQuestionView()
        {
            MatchingQuestionView template = questionViews[0];
            GameObject newGameObject = Instantiate(template.gameObject, transform);
            
            if (newGameObject.TryGetComponent(out MatchingQuestionView questionView))
            {
                questionView.OnAnswerDropped += QuestionView_OnAnswerDropped;
                questionViews.Add(questionView);
            }
        }
        
        private void RemoveQuestionView()
        {
            int lastIndex = questionViews.Count - 1;
            if (lastIndex <= 0) return;
            
            MatchingQuestionView viewToRemove = questionViews[lastIndex];
            
            viewToRemove.OnAnswerDropped += QuestionView_OnAnswerDropped;
            questionViews.Remove(viewToRemove);
            
            Destroy(viewToRemove.gameObject);
        }
        
        
        // EVENT LISTENER METHODS
        private void QuestionView_OnAnswerDropped(int index, string answer)
        {
            OnAnswerDropped?.Invoke(index, answer);
        }
    }
}
