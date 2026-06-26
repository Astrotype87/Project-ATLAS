using UnityEngine;

using ProjectATLAS.Gameplay;
using ProjectATLAS.Minigame.Mini03_HoverRacing;
using ProjectATLAS.Gameplay.UI;

namespace ProjectATLAS.Simulation.Sim03_HovercraftTesting
{
    public class Sim03_HovercraftTesting : SimulationSystem
    {
        [Header("Components")]
        [SerializeField] private HovercraftPhysics hovercraftPhysics;
        
        [Header("Parameters")]
        [SerializeField] private SimParameter massParameter;
        [SerializeField] private SimParameter gravityParameter;
        [SerializeField] private SimParameter hoverForceParameter;
        [SerializeField] private SimParameter thrustForceParameter;
        [SerializeField] private SimParameter airbrakeDragParameter;
        
        private bool simulationStarted;
        
        // MONOBEHAVIOUR METHODS
        private void Update()
        {
            if (simulationStarted)
            {
                Debug.Log("Signal");
                hovercraftPhysics.Mass = massParameter.Value;
                hovercraftPhysics.Gravity = new(0f, gravityParameter.Value);
                hovercraftPhysics.HoverForce = hoverForceParameter.Value;
                hovercraftPhysics.ThrustForce = thrustForceParameter.Value;
                hovercraftPhysics.AirbrakeDrag = airbrakeDragParameter.Value;
            }
        }
        
        
        // SIMULATION SYSTEM METHODS
        protected override void OnInitializeSimulation(Difficulty difficulty)
        {
             
             
        }
        
        protected override void OnStartSimulation()
        {
            simulationStarted = true;
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
            
            base.IsCompleted = true;
            base.IsBronzeAwarded = IsCompleted;
            base.IsSilverAwarded = IsCompleted;
            base.IsGoldAwarded = IsCompleted;
            
            
            // PHASE 3: Display scores to gameResultPage and customResultsView
            gameResultPage.OpenPageInGroup();
            gameResultPage.SetScore((int)Score, (int)MaxScore);
            gameResultPage.SetPlayTime((int)PlayTime);
            gameResultPage.SetPointsEntries(PointsEntries);
        }
    }
}
