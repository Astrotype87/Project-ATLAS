using System;
using UnityEngine;
using UnityEngine.UI;

using ProjectATLAS.Gameplay;
using ProjectATLAS.Lesson;
using TMPro;

namespace ProjectATLAS.Minigame.Mini08_WaveCalibration
{
    using Range = ProjectATLAS.Types.Range;
    
    public class Mini08_WaveCalibration : MinigameSystem
    {
        [Header("Sliders")]
        [SerializeField] private VariableSlider frequencySlider;
        [SerializeField] private VariableSlider amplitudeSlider;
        [SerializeField] private VariableSlider phaseSlider;
        
        [Header("Calibration")]
        [SerializeField] private CalibrationUnit[] calibrations;
        [SerializeField] private Button calibrateButton;
        
        [Header("Progress")]
        [SerializeField] private Image accuracyFill;
        [SerializeField] private TMP_Text calibrationPercent;
        
        [Header("Medal Time")]
        [SerializeField] private float goldTime = 60f;
        [SerializeField] private float silverTime = 70f;
        [SerializeField] private float bronzeTime = 80f;
        
        [Header("Scoring")]
        [SerializeField] private int easyScore = 1000;
        [SerializeField] private int mediumScore = 1250;
        [SerializeField] private int hardScore = 1500;
        
        private int currentCalibration = 1;
        private int currentScore;
        private int maxScore;
        
        
        private bool isWin;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            calibrateButton.onClick.AddListener(CalibrateButton_onClick);
        }
        
        private void CalibrateButton_onClick()
        {
            // Check calibration
            
            
            
            
            
            // End minigame if all calibrated
            if (currentCalibration > calibrations.Length)
            {
                base.FinishMinigame();
            }
        }
        
        
        
        // MINIGAME SYSTEM METHODS
        protected override void OnInitializeMinigame(Difficulty difficulty)
        {
            
        }
        
        protected override void OnStartMinigame()
        {
            // Pick random range for values. Set them to calibration units
            foreach (var calibration in calibrations)
            {
                float frequency = GetRandomValueWithSnap(frequencySlider.Range, frequencySlider.Snap);
                calibration.SetFrequency(frequency);
                
                float amplitude = GetRandomValueWithSnap(amplitudeSlider.Range, amplitudeSlider.Snap);
                calibration.SetAmplitude(amplitude);
                
                float phase = GetRandomValueWithSnap(phaseSlider.Range, phaseSlider.Snap);
                calibration.SetPhase(phase);
            }
        }
        
        protected override void OnFinishMinigame()
        {
            // PHASE 0: Stop controls from moving
            
            
            
            // PHASE 1: Calculate score
            
            
            
            // PHASE 2: Set minigame score data
            base.Score = currentScore;
            base.MaxScore = calibrations.Length * 3;
            base.PlayTime = (float)base.GetPlayTime().TotalSeconds;
            
            int minigamePoints = (int)(base.Score) * 100;
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
        
        
        
        // PRIVATE METHODS
        private float GetRandomValueWithSnap(Range range, float snap)
        {
            float value = range.GetRandom();
            float snapped = Mathf.Round(value / snap) * snap;
            return Mathf.Clamp(snapped, range.min, range.max);
        }
        
        
        
    }
}
