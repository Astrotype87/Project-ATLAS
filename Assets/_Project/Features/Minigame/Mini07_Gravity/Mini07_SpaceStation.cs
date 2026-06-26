using UnityEngine;

using ProjectATLAS.Gameplay;

namespace ProjectATLAS.Minigame.Mini07_SpaceStation
{
    public class Mini07_SpaceStation : MinigameSystem
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
        [SerializeField] private MonoBehaviourPlayerController playerController;
        [SerializeField] private CongratsTrigger congratsTrigger;
        
        private int baseScore;
        private int currentScore;
        private bool isWin;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            congratsTrigger.OnGameWin += CongratsTrigger_OnGameWin;
            // cargoLevelManager.OnGameLoss += CargoLevelManager_OnGameLoss;
        }
        
        private void CongratsTrigger_OnGameWin()
        {
            isWin = true;
            base.FinishMinigame();
        }
        
        private void CargoLevelManager_OnGameLoss()
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
            playerController.StartTimer();
        }
        
        protected override void OnFinishMinigame()
        {
            // PHASE 2: Set minigame score data
            base.Score = 100;
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
            base.IsBronzeAwarded = isWin;
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
