using System;
using UnityEngine;
using TMPro;
using ProjectATLAS.UI;

namespace ProjectATLAS.Quiz.UI
{
    public class MatchingQuestionView : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private string question;
        
        [Header("Components")]
        [SerializeField] private TMP_Text questionText;
        [SerializeField] private UIDropArea dropArea;
        
        // PROPERTIES
        public event Action<int, string> OnAnswerDropped;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            dropArea.OnDropped += DropArea_OnDropped;
        }
        
        private void OnValidate()
        {
            if (questionText) questionText.text = question;
        }
        
        
        // PUBLIC METHODS
        public void DisplayQuestion(string question)
        {
            this.question = question;
            
            if (questionText) questionText.text = question;
        }
        
        public string GetAssignedAnswer()
        {
            if (dropArea)
            {
                GameObject gameObject = dropArea.GetFirstStored();
                if (gameObject == null) return string.Empty;
                
                if (gameObject.TryGetComponent(out MatchingAnswerView matchingAnswerView))
                {
                    return matchingAnswerView.Answer;
                }
            }
            
            return string.Empty;
        }
        
        public void AssignAnswer(MatchingAnswerView answerView)
        {
            dropArea.DropObject(answerView.gameObject);
        }
        
        // EVENT LISTENER METHODS
        private void DropArea_OnDropped(GameObject gameObject)
        {
            if (gameObject.TryGetComponent(out MatchingAnswerView matchingAnswerView))
            {
                int index = transform.GetSiblingIndex();
                string answer = matchingAnswerView.Answer;
                OnAnswerDropped?.Invoke(index, answer);
            }
        }
    }
}
