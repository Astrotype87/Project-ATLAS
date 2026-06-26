using System;
using System.Collections.Generic;
using UnityEngine;
using KBCore.Refs;
using UnityEngine.UI;

using ProjectATLAS.UI;

namespace ProjectATLAS.Quiz.UI
{
    public class MatchingAnswersPanel : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField, Child] private List<MatchingAnswerView> answerViews;
        [SerializeField, Self] private HorizontalLayoutGroup horizontalLayoutGroup;
        [SerializeField] private UIDropArea dropArea;
        
        private int previousChildCount;
        
        // PROPERTIES
        public event Action OnContainedAnswersChanged;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            previousChildCount = transform.childCount;
        }
        
        private void Update()
        {
            if (transform.childCount != previousChildCount)
            {
                OnContainedAnswersChanged?.Invoke();
                previousChildCount = transform.childCount;
            }
        }
        
        
        // PUBLIC METHODS
        public void DisplayAnswers(string[] answers)
        {
            // Drop all answer views back to answers list container text
            for (int i = 0; i < answerViews.Count; i++)
            {
                dropArea.DropObject(answerViews[i].gameObject);
            }
            
            // Handle answer button duplication logic
            int targetChildCount = Mathf.FloorToInt(answers.Length);
            int interval = targetChildCount - answerViews.Count;
            
            if (interval > 0)
            {
                for (int i = 0; i < interval; i++)
                {
                    AddAnswerView();
                }
            }
            else if (interval < 0)
            {
                interval = -interval; // flip the sign
                for (int i = 0; i < interval; i++)
                {
                    RemoveAnswerView();
                }
            }
            
            // Update answer view text
            for (int i = 0; i < answerViews.Count; i++)
            {
                answerViews[i].DisplayAnswer(answers[i]);
            }
        }
        
        public MatchingAnswerView GetAnswerView(string answer)
        {
            for (int i = 0; i < answerViews.Count; i++)
            {
                if (answerViews[i].Answer == answer) return answerViews[i];
            }
            return null;
        }
        
        // PRIVATE METHODS
        private void AddAnswerView()
        {
            MatchingAnswerView template = answerViews[0];
            GameObject newGameObject = Instantiate(template.gameObject, transform);
            
            if (newGameObject.TryGetComponent(out MatchingAnswerView answerView))
            {
                answerViews.Add(answerView);
            }
        }
        
        private void RemoveAnswerView()
        {
            int lastIndex = answerViews.Count - 1;
            if (lastIndex <= 0) return;
            
            MatchingAnswerView viewToRemove = answerViews[lastIndex];
            
            answerViews.Remove(viewToRemove);
            
            Destroy(viewToRemove.gameObject);
        }
    }
}
