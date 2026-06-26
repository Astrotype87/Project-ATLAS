using UnityEngine;

using ProjectATLAS.Gameplay;

namespace ProjectATLAS.Simulation.Sim07_SpaceLaunch
{
    public class Sim07_SpaceLaunch : SimulationSystem
    {
        [SerializeField] private GoalPanelController goalPanelController;
        private bool isWin;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            goalPanelController.OnGameWin += GoalPanelController_OnGameWin;
        }
        
        private void GoalPanelController_OnGameWin()
        {
            isWin = true;
            base.FinishSimulation();
        }
        
        
        
        // SIMULATION SYSTEM METHODS
        protected override void OnInitializeSimulation(Difficulty difficulty)
        {
            
        }
        
        protected override void OnStartSimulation()
        {
            
        }
        
        protected override void OnFinishSimulation()
        {
            // PHASE 2: Set minigame score data
            base.Score = 100;
            base.MaxScore = 100;
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
            base.IsSilverAwarded = isWin;
            base.IsGoldAwarded = isWin;
            
            
            // PHASE 3: Display scores to gameResultPage and customResultsView
            gameResultPage.OpenPageInGroup();
            gameResultPage.SetScore((int)Score, (int)MaxScore);
            gameResultPage.SetPlayTime((int)PlayTime);
            gameResultPage.SetPointsEntries(PointsEntries);
        }
    }
}
