using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using ProjectATLAS.Gameplay;

namespace ProjectATLAS.Simulation.Sim02_ProjectileMotion
{
    public class Sim02_ProjectileMotion : SimulationSystem
    {
        [Header("Scene")]
        [SerializeField] private ProjectileLauncher projectileLauncher;
        [SerializeField] private Transform goalTransform;
        [SerializeField] private Vector2 minGoalPosition;
        [SerializeField] private Vector2 maxGoalPosition;
        
        [Header("Score Settings")]
        [SerializeField] private int maxGoalScore = 5;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text endText;
        
        [Header("Difficulty Size")]
        [SerializeField] private float easyGoalSize = 1.0f;
        [SerializeField] private float mediumGoalSize = 0.75f;
        [SerializeField] private float hardGoalSize = 0.5f;
        [SerializeField] private float goldPlayTime = 15f;
        
        private Vector3 originalGoalScale;
        private int currentGoalScore;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            projectileLauncher.OnGoalReached += ProjectileLauncher_OnGoalReached;
            projectileLauncher.OnGoalReset += ProjectileLauncher_OnGoalReset;
            
            // Save original scale (for ratio reference)
            if (goalTransform != null)
                originalGoalScale = goalTransform.localScale;
        }
        
        
        // EVENT LISTENER METHODS
        private void ProjectileLauncher_OnGoalReached()
        {
            // Increase score
            currentGoalScore++;
            currentGoalScore = Mathf.Min(currentGoalScore, maxGoalScore);
            UpdateScoreUI();
        }
        
        private void ProjectileLauncher_OnGoalReset()
        {
            RandomizeGoalPosition();
            
            
            // // Check if max score reached
            if (currentGoalScore >= maxGoalScore)
            {
                endText.text = "Congratulations for completing the goal score!\nYou can end the simulation by clicking the end button!";
                // FinishSimulation();
            }
            // else
            // {
            //     // Otherwise, randomize next goal position
            //     RandomizeGoalPosition();
            // }
        }
        
        // PROTECTED OVERRIDE METHODS
        protected override void OnInitializeSimulation(Difficulty difficulty)
        {
            currentGoalScore = 0;
            UpdateScoreUI();
            
            endText.text = "";
            
            // Update size of goal based on difficulty
            float sizeMultiplier = difficulty switch
            {
                Difficulty.Easy => easyGoalSize,
                Difficulty.Medium => mediumGoalSize,
                Difficulty.Hard => hardGoalSize,
                _ => easyGoalSize
            };
            
            // Apply uniform scaling based on original ratio
            goalTransform.localScale = originalGoalScale * sizeMultiplier;
        }
        
        protected override void OnStartSimulation()
        {
            currentGoalScore = 0;
            UpdateScoreUI();
            
            RandomizeGoalPosition();
        }
        
        protected override void OnFinishSimulation()
        {
            // PHASE 1: Calculate score
            
            // PHASE 2: Set simulation score data
            base.Score = currentGoalScore;
            base.MaxScore = maxGoalScore;
            base.PlayTime = (float)base.GetPlayTime().TotalSeconds;
            
            int simulationPoints = (int)(base.Score * base.scoreMultiplier);
            int timeBonus = (int)(CalculateTimeBonus(base.PlayTime) * base.ScorePercentage);
            base.PointsEntries = new PointsEntry[]
            {
                new("Score", simulationPoints),
                new("Time Bonus", timeBonus),
            };
            
            base.IsCompleted = true;        // Simulations are automatically completed
            base.IsBronzeAwarded = true;    // with bronze medal
            base.IsSilverAwarded = base.ScorePercentage >= 0.5f;
            base.IsGoldAwarded = base.ScorePercentage >= 1;
            
            
            // PHASE 3: Display scores to gameResultPage and customResultsView
            gameResultPage.OpenPageInGroup();
            gameResultPage.SetScore((int)Score, (int)MaxScore);
            gameResultPage.SetPlayTime((int)PlayTime);
            gameResultPage.SetPointsEntries(PointsEntries);
        }
        
        
        // PRIVATE METHODS
        private void RandomizeGoalPosition()
        {
            // Pick random position inside the defined bounds
            float randomX = UnityEngine.Random.Range(minGoalPosition.x, maxGoalPosition.x);
            float randomY = UnityEngine.Random.Range(minGoalPosition.y, maxGoalPosition.y);
            
            // Apply new position (2D: XY plane, keep Z unchanged)
            goalTransform.position = new Vector3(randomX, randomY, goalTransform.position.z);
        }
        
        private void UpdateScoreUI()
        {
            if (scoreText != null)
                scoreText.text = $"{currentGoalScore}/{maxGoalScore}";
        }
    }
}
