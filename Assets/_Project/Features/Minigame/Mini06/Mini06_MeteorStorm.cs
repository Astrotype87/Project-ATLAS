using UnityEngine;

using ProjectATLAS.Gameplay;

namespace ProjectATLAS.Minigame.Mini06_MeteorStorm
{
    public class Mini06_MeteorStorm : MinigameSystem
    {
        [Header("Damage")]
        [SerializeField] private float goldDamage = 0f;
        [SerializeField] private float silverDamage = 0.5f;
        [SerializeField] private float bronzeDamage = 1f;
        
        [Header("Scoring")]
        [SerializeField] private int easyScore = 1000;
        [SerializeField] private int mediumScore = 1250;
        [SerializeField] private int hardScore = 1500;
        
        [Header("Components")]
        [SerializeField] private AirshipController aircraftController;
        
        private int baseScore;
        private int currentScore;
        private bool isWin;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            aircraftController.OnGameWin += AircraftController_OnGameWin;
            aircraftController.OnGameLoss += AircraftController_OnGameLoss;
        }
        
        private void AircraftController_OnGameWin()
        {
            isWin = true;
            base.FinishMinigame();
        }
        
        private void AircraftController_OnGameLoss()
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
            
            aircraftController.SetDifficulty(difficulty);
        }
        
        protected override void OnStartMinigame()
        {
            aircraftController.StartGame();
        }
        
        protected override void OnFinishMinigame()
        {
            // PHASE 0: Stop controls from moving
            
            
            // PHASE 1: Calculate score
            
            
            // PHASE 2: Set minigame score data
            base.Score = 1000;
            base.MaxScore = 1000;
            base.PlayTime = (float)base.GetPlayTime().TotalSeconds;
            
            int minigamePoints = (int)(base.Score);
            int timeBonus = (int)(CalculateTimeBonus(base.PlayTime) * base.ScorePercentage);
            base.PointsEntries = new PointsEntry[]
            {
                new("Score", minigamePoints),
                new("Time Bonus", timeBonus),
            };
            
            base.IsCompleted = isWin;
            base.IsBronzeAwarded = aircraftController.CurrentDamage <= goldDamage;
            base.IsSilverAwarded = aircraftController.CurrentDamage <= silverDamage;
            base.IsGoldAwarded = aircraftController.CurrentDamage <= bronzeDamage;
            
            
            // PHASE 3: Display scores to gameResultPage and customResultsView
            gameResultPage.OpenPageInGroup();
            gameResultPage.SetScore((int)Score, (int)MaxScore);
            gameResultPage.SetPlayTime((int)PlayTime);
            gameResultPage.SetPointsEntries(PointsEntries);
        }
    }
}
