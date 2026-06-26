using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using ProjectATLAS.Gameplay;

namespace ProjectATLAS.Simulation.Sim01_PartsMeasurements
{
    public class Sim01_PartsMeasurements : SimulationSystem
    {
        [Header("Sim 01: Parts Measurements")]
        [SerializeField] private MeasuredPart[] measuredParts;
        [SerializeField, Range(0, 4)] private int rounding = 1;
        
        [Header("Sim 01: UI")]
        [SerializeField] private MeasurementsPanel measurementsPanel;
        [SerializeField] private MeasurementResultsView measurementResultsView;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            measurementsPanel.OnCheckClicked += MeasurementsPanel_OnCheckClicked;
        }
        
        // EVENT LISTENER METHODS
        private void MeasurementsPanel_OnCheckClicked()
        {
            (string, float)[] answers = measurementsPanel.GetAnswers();
            
            foreach (var (id, value) in answers)
            {
                Debug.Log($"{id}");
                
                var part = Array.Find(measuredParts, p => p.Name == id);
                if (part == null) continue;
                
                float correct = (float)Math.Round(part.Length, rounding);
                bool isCorrect = Mathf.Approximately(correct, value);
                
                // Tell the input panel to highlight
                measurementsPanel.HighlightField(id, !isCorrect);
            }
        }
        
        // PROTECTED OVERRIDE METHODS
        protected override void OnInitializeSimulation(Difficulty difficulty)
        {
            // Generate custom lengths
            foreach (var measuredPart in measuredParts)
            {
                float length = (float)Math.Round(measuredPart.RandomRange.GetRandom(), rounding);
                measuredPart.SetLength(length);
            }
        }
        
        protected override void OnStartSimulation()
        {
            
        }
        
        protected override void OnFinishSimulation()
        {
            // PHASE 1: Calculate score
            (string, float)[] answers = measurementsPanel.GetAnswers();
            var results = new (string partName, float correct, float answer)[answers.Length];
            
            float measurementScore = 0;
            for (int i = 0; i < answers.Length; i++)
            {
                var (id, value) = answers[i];
                var part = Array.Find(measuredParts, p => p.Name == id);
                if (part == null) continue;
                
                float correct = (float)Math.Round(part.Length, rounding);
                bool isCorrect = Mathf.Approximately(correct, value);
                
                if (isCorrect) measurementScore++;
                measurementsPanel.HighlightField(id, !isCorrect);
                
                results[i] = (id, correct, value); // Build results entry
            }
            
            
            // PHASE 2: Set simulation score data
            base.Score = measurementScore;
            base.MaxScore = measuredParts.Length;
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
            
            measurementResultsView.DisplayResults(results);
        }
    }
}
