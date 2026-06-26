using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectATLAS.Quiz.UI
{
    public class ReviewItemView : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField, TextArea] private string question;
        [SerializeField, TextArea] private string points;
        [SerializeField, TextArea] private string answer;
        
        [Header("Components")]
        [SerializeField] private TMP_Text questionText;
        [SerializeField] private TMP_Text pointsText;
        [SerializeField] private TMP_Text answerText;
        [SerializeField] private Button editButton;
        
        // PROPERTIES
        public event Action<int> OnEditClicked;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            editButton.onClick.AddListener(EditButton_onClick);
        }
        
        private void OnValidate()
        {
            if (questionText)
            {
                int number = transform.GetSiblingIndex();
                questionText.text = $"{number}. {question}";
            }
            if (pointsText) pointsText.text = points;
            if (answerText) answerText.text = answer;
        }
        
        
        // PUBLIC METHODS
        public void UpdateText(string question, string points, string answer, bool hideDifficulty)
        {
            this.question = question;
            this.points = points;
            this.answer = answer;
            
            if (questionText)
            {
                int number = transform.GetSiblingIndex() + 1;
                questionText.text = $"{number}. {question}";
            }
            if (pointsText) pointsText.text = points;
            if (answerText) answerText.text = answer;
        }
        
        
        // EVENT LISTENER METHODS
        private void EditButton_onClick()
        {
            int index = transform.GetSiblingIndex();
            OnEditClicked?.Invoke(index);
        }
    }
}
