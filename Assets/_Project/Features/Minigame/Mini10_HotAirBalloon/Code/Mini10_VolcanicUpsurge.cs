using UnityEngine;

using ProjectATLAS.Gameplay;

namespace ProjectATLAS.Minigame.Mini10_VolcanicUpsurge
{
    public class Mini10_VolcanicUpsurge : MinigameSystem
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
        [SerializeField] private MiniGameManager miniGameManager;
        
        
        private int baseScore;
        private int currentScore;
        private bool isWin;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            miniGameManager.OnGameWin += MiniGameManager_OnGameWin;
            miniGameManager.OnGameLose += MiniGameManager_OnGameLose;
        }
        
        private void MiniGameManager_OnGameWin()
        {
            isWin = true;
            base.FinishMinigame();
        }
        
        private void MiniGameManager_OnGameLose()
        {
            isWin = false;
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
            
            base.IsCompleted = isWin;
            base.IsBronzeAwarded = base.IsCompleted;
            base.IsSilverAwarded = isWin && base.PlayTime < silverTime;
            base.IsGoldAwarded = isWin && base.PlayTime < goldTime;
            
            
            // PHASE 3: Display scores to gameResultPage and customResultsView
            gameResultPage.OpenPageInGroup();
            gameResultPage.SetScore((int)Score, (int)MaxScore);
            gameResultPage.SetPlayTime((int)PlayTime);
            gameResultPage.SetPointsEntries(PointsEntries);
        }
    }
}
