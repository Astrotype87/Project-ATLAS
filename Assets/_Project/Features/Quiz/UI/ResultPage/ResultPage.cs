using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using ProjectATLAS.UI;
using ProjectATLAS.Gameplay;
using ProjectATLAS.Gameplay.UI;

namespace ProjectATLAS.Quiz.UI
{
    public class ResultPage : UIPage
    {
        [Header("Data")]
        [SerializeField] private string points;
        [SerializeField] private string time;
        [SerializeField] private string totalScore;
        
        [Header("Components")]
        [SerializeField] private TMP_Text pointsText;
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private ResultListPanel resultListPanel;
        [SerializeField] private PointsListPanel pointsListPanel;
        [SerializeField] private TMP_Text totalScoreText;
        [SerializeField] private Button nextButton;
        
        
        // PROPERTIES
        public event Action OnNextClicked;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            nextButton.onClick.AddListener(NextButton_OnNextClicked);
        }
        
        protected override void OnValidate()
        {
            pointsText.text = points.ToString();
            timeText.text = time.ToString();
            totalScoreText.text = totalScore.ToString();
        }
        
        
        // PUBLIC METHODS
        public void DisplayResults(QuizResultData resultData, PointsEntry[] pointsEntries)
        {
            points = $"{resultData.score}/{resultData.maxScore}";
            time = $"{resultData.time.Minutes:D2}:{resultData.time.Seconds:D2}";
            totalScore = PointsEntry.GetTotalPoints(pointsEntries).ToString();
            
            pointsText.text = points;
            timeText.text = time;
            resultListPanel.DisplayResultData(resultData);
            pointsListPanel.DisplayPointsEntries(pointsEntries);
            totalScoreText.text = totalScore;
        }
        
        
        // EVENT LISTENER METHODS
        public void NextButton_OnNextClicked()
        {
            OnNextClicked?.Invoke();
        }
    }
}
