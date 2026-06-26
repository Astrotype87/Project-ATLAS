using UnityEngine;
using UnityEngine.UI;
using TMPro;

using ProjectATLAS.UI;
using System;

namespace ProjectATLAS.Gameplay.UI
{
    public class GameResultPage : UIPage
    {
        [Header("Details")]
        [SerializeField] private string title;
        [SerializeField] private string subtitle;
        [SerializeField] private int score;
        [SerializeField] private int maxScore;
        [SerializeField] private float time; // display time as mm:ss.pp, minutes extends past 60, no hours display
        [SerializeField] private PointsEntry[] pointsEntries;
        [SerializeField] private int points;
        
        [Header("Components")]
        [SerializeField] private TMP_Text titleComponent;
        [SerializeField] private TMP_Text subtitleComponent;
        [SerializeField] private TMP_Text scoreComponent;
        [SerializeField] private TMP_Text timeComponent;
        [SerializeField] private PointsListPanel pointsListPanel;
        [SerializeField] private TMP_Text pointsComponent;
        [SerializeField] private Button nextButton;
        [SerializeField] private GameObject scorePanel;
        
        // PROPERTIES
        public Button QuitButton => nextButton;
        
        public event Action OnNextClicked;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            nextButton.onClick.AddListener(NextButton_onClick);
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();
            
            SetTitle(title);
            SetSubtitle(subtitle);
            SetScore(score, maxScore);
            SetPlayTime(time);
        }
        
        
        // PUBLIC METHODS
        public void SetTitle(string title)
        {
            this.title = title;
            if (titleComponent != null) titleComponent.text = title;
        }
        
        public void SetSubtitle(string subtitle)
        {
            this.subtitle = subtitle;
            if (subtitleComponent != null) subtitleComponent.text = subtitle;
        }
        
        public void SetScore(int score, int maxScore)
        {
            this.score = score;
            if (scoreComponent != null) scoreComponent.text = $"{score}/{maxScore}";
        }
        
        public void SetPlayTime(float time)
        {
            this.time = time;
            if (timeComponent != null) timeComponent.text = FormatTime(time);
        }
        
        public void SetPointsEntries(PointsEntry[] pointsEntries)
        {
            this.pointsEntries = pointsEntries;
            if (pointsListPanel) pointsListPanel.DisplayPointsEntries(pointsEntries);
            if (pointsComponent) pointsComponent.text = PointsEntry.GetTotalPoints(pointsEntries).ToString();
        }
        
        public void HideScoreAndTimeBonusPanel(bool hide)
        {
            scorePanel.SetActive(!hide);
        }
        
        
        
        // PRIVATE METHODS
        private string FormatTime(float time)
        {
            int totalMinutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            // int centiseconds = Mathf.FloorToInt((time * 100f) % 100f); // two decimals (hundredths)
            
            return $"{totalMinutes:00}:{seconds:00}";
        }
        
        
        // EVENT LISTENER METHODS
        private void NextButton_onClick()
        {
            OnNextClicked?.Invoke();
        }
        
    }
}
