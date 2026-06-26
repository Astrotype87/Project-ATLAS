using UnityEngine;

using ProjectATLAS.Input;
using ProjectATLAS.Gameplay;

namespace ProjectATLAS.Minigame.Mini03_HoverRacing
{
    public class Mini03_HoverRacing : MinigameSystem
    {
        [Header("Medal Time")]
        [SerializeField] private float goldTime = 60f;
        [SerializeField] private float silverTime = 70f;
        [SerializeField] private float bronzeTime = 80f;
        
        [Header("Scoring")]
        [SerializeField] private int easyScore = 1000;
        [SerializeField] private int mediumScore = 1250;
        [SerializeField] private int hardScore = 1500;
        
        [Header("Components")]
        [SerializeField] private FinishLine finishLine;
        [SerializeField] private InputSlider thrustSlider; // returns 0-1
        
        private int baseScore;
        private int currentScore;
        
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            finishLine.OnFinishLineTriggered += FinishLine_OnFinishLineTriggered;
        }
        
        private void FinishLine_OnFinishLineTriggered()
        {
            base.FinishMinigame();
        }
        
        
        // MINIGAME SYSTEM METHODS
        protected override void OnInitializeMinigame(Difficulty difficulty)
        {
            baseScore = difficulty switch
            {
                Difficulty.Easy => easyScore,
                Difficulty.Medium => mediumScore,
                Difficulty.Hard => hardScore,
                _ => easyScore,
            };
            
            currentScore = baseScore;
        }
        
        protected override void OnStartMinigame()
        {
            
        }
        
        protected override void OnFinishMinigame()
        {
            // PHASE 0: Stop controls from moving
            thrustSlider.SetValue(0f);
            
            
            // PHASE 1: Calculate score
            
            
            
            // PHASE 2: Set minigame score data
            base.Score = currentScore;
            base.MaxScore = baseScore;
            base.PlayTime = (float)base.GetPlayTime().TotalSeconds;
            
            int minigamePoints = (int)(base.Score);
            int timeBonus = (int)(CalculateTimeBonus(base.PlayTime) * base.ScorePercentage);
            base.PointsEntries = new PointsEntry[]
            {
                new("Score", minigamePoints),
                new("Time Bonus", timeBonus),
            };
            
            base.IsCompleted = base.PlayTime < bronzeTime;
            base.IsBronzeAwarded = base.PlayTime < bronzeTime;
            base.IsSilverAwarded = base.PlayTime < silverTime;
            base.IsGoldAwarded = base.PlayTime < goldTime;
            
            
            // PHASE 3: Display scores to gameResultPage and customResultsView
            gameResultPage.OpenPageInGroup();
            gameResultPage.SetScore((int)Score, (int)MaxScore);
            gameResultPage.SetPlayTime((int)PlayTime);
            gameResultPage.SetPointsEntries(PointsEntries);
        }
    }
}
