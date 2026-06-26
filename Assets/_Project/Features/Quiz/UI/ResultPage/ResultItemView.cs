using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectATLAS.Quiz.UI
{
    public class ResultItemView : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private bool isCorrect;
        [SerializeField, TextArea] private string question;
        [SerializeField, TextArea] private string points;
        [SerializeField, TextArea] private string answer;
        [SerializeField, TextArea] private string correct;
        
        [Header("Components")]
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text questionText;
        [SerializeField] private TMP_Text pointsText;
        [SerializeField] private TMP_Text answerText;
        [SerializeField] private TMP_Text correctText;
        
        [Header("Style")]
        [SerializeField] private Sprite correctIcon;
        [SerializeField] private Sprite wrongIcon;
        
        
        // MONOBEHAVIOUR METHODS
        private void OnValidate()
        {
            if (iconImage) iconImage.sprite = isCorrect ? correctIcon : wrongIcon;
            if (questionText)
            {
                int number = transform.GetSiblingIndex();
                questionText.text = $"{number}. {question}";
            }
            if (pointsText) pointsText.text = points;
            if (answerText) answerText.text = answer;
            if (correctText) correctText.text = correct;
        }
        
        
        // PUBLIC METHODS
        public void DisplayResult(QuizResultData.ItemResult itemResult)
        {
            isCorrect = itemResult.isCorrect;
            question = itemResult.question;
            points = itemResult.points;
            answer = itemResult.answer;
            correct = itemResult.correct;
            
            if (iconImage) iconImage.sprite = isCorrect ? correctIcon : wrongIcon;
            if (questionText)
            {
                int number = transform.GetSiblingIndex() + 1;
                questionText.text = $"{number}. {question}";
            }
            if (pointsText) pointsText.text = points;
            if (answerText) answerText.text = answer;
            if (correctText) correctText.text = correct;
        }
    }
}
