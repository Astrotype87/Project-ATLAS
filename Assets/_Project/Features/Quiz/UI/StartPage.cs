using System;
using UnityEngine;
using UnityEngine.UI;
using ProjectATLAS.UI;
using TMPro;
using System.Collections.Generic;

namespace ProjectATLAS.Quiz.UI
{
    public class StartPage : UIPage
    {
        [Header("Data")]
        [SerializeField] private string title;
        [SerializeField] private string subtitle;
        [SerializeField, TextArea] private string description;
        [SerializeField, TextArea] private string details;
        
        [Header("Components")]
        [SerializeField] private Button startButton;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text subtitleText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text detailsText;
        
        
        // PROPERTIES
        public event Action OnStartClicked;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            startButton.onClick.AddListener(StartButton_onClick);
        }
        
        protected override void OnValidate()
        {
            if (titleText) titleText.text = title;
            if (subtitleText) subtitleText.text = subtitle;
            if (descriptionText) descriptionText.text = description;
            if (detailsText) detailsText.text = details;
        }
        
        
        // PUBLIC METHODS
        public void DisplayQuizInfo(QuizData quizData, List<QuizItem> quizItems)
        {
            if (titleText) titleText.text = quizData.Level;
            if (subtitleText) subtitleText.text = quizData.Name;
            if (descriptionText) descriptionText.text = quizData.Description;
            if (detailsText)
            {
                string text = $"{quizItems.Count} items\n{QuizData.GetQuizDetails(quizItems)}";
                detailsText.text = text;
            }
        }
        
        public void DisplayQuizInfo(string level, string name, string description, List<QuizItem> quizItems)
        {
            if (titleText) titleText.text = level;
            if (subtitleText) subtitleText.text = name;
            if (descriptionText) descriptionText.text = description;
            if (detailsText)
            {
                string text = $"{quizItems.Count} items\n{QuizData.GetQuizDetails(quizItems)}";
                detailsText.text = text;
            }
        }
        
        
        // EVENT LISTENER METHODS
        private void StartButton_onClick()
        {
            OnStartClicked?.Invoke();
        }
    }
}
