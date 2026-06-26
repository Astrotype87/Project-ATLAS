using UnityEngine;

using ProjectATLAS.Input;
using ProjectATLAS.Gameplay;

namespace ProjectATLAS.Minigame.Mini02_DesertNavigation
{
    public class Mini02_DesertNavigation : MinigameSystem
    {
        [Header("Medal Time")]
        [SerializeField] private float goldTime = 60f;
        [SerializeField] private float silverTime = 70f;
        [SerializeField] private float bronzeTime = 80f;
        
        [Header("Scoring")]
        [SerializeField] private int baseEasyScore = 1000;
        [SerializeField] private int baseMediumScore = 1250;
        [SerializeField] private int baseHardScore = 1500;
        
        [Header("Components")]
        [SerializeField] private KinematicHovercraftPhysics hovercraftPhysics;
        [SerializeField] private SandstormWind sandstormWind;
        [SerializeField] private FinishLine finishLine;
        [SerializeField] private InputSlider speedSlider; // returns 0-1
        [SerializeField] private InputDirection inputDirection; // returns -180 - 180  // Direction, angle
        
        private int baseScore;
        private int currentScore;
        private bool isCrashed;
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            hovercraftPhysics.OnCrashed += HovercraftPhysics_OnCrashed;
            finishLine.OnFinishLineTriggered += FinishLine_OnFinishLineTriggered;
        }
        
        private void FinishLine_OnFinishLineTriggered()
        {
            base.FinishMinigame();
        }
        
        private void HovercraftPhysics_OnCrashed()
        {
            isCrashed = true;
            base.FinishMinigame();
        }
        
        
        // MINIGAME SYSTEM METHODS
        protected override void OnInitializeMinigame(Difficulty difficulty)
        {
            baseScore = difficulty switch
            {
                Difficulty.Easy => baseEasyScore,
                Difficulty.Medium => baseMediumScore,
                Difficulty.Hard => baseHardScore,
                _ => baseEasyScore,
            };
            
            currentScore = baseScore;
            
            sandstormWind.SetDifficulty(difficulty);
        }
        
        protected override void OnStartMinigame()
        {
            sandstormWind.StartSandstormSequence();
        }
        
        protected override void OnFinishMinigame()
        {
            // PHASE 0: Stop controls from moving
            speedSlider.SetValue(0f);
            inputDirection.SetAngle(0f);
            
            
            // PHASE 1: Calculate score
            float time = (float)base.GetPlayTime().TotalSeconds;
            float damagePenalty = 1 - hovercraftPhysics.CurrentDamage;
            
            float timePercentage;
            if (time <= goldTime) timePercentage = 1f;
            else if (time >= bronzeTime) timePercentage = 0f;
            else // Linearly map [goldTime → bronzeTime] to [1 → 0]
            {
                float t = (time - goldTime) / (bronzeTime - goldTime);
                timePercentage = Mathf.Lerp(1f, 0f, t);
            }
            
            base.Score = baseScore * timePercentage * damagePenalty;
            base.Score = Mathf.Max(0f, base.Score);
            
            
            // PHASE 2: Set minigame score data
            base.MaxScore = baseScore;
            base.PlayTime = (float)base.GetPlayTime().TotalSeconds;
            
            int minigamePoints = (int)(base.Score);
            int timeBonus = (int)(CalculateTimeBonus(base.PlayTime) * base.ScorePercentage);
            base.PointsEntries = new PointsEntry[]
            {
                new("Score", minigamePoints),
                new("Time Bonus", timeBonus),
            };
            
            base.IsCompleted = base.PlayTime < bronzeTime && !isCrashed;
            base.IsBronzeAwarded = base.IsCompleted && base.PlayTime < bronzeTime;
            base.IsSilverAwarded = base.IsCompleted && base.PlayTime < silverTime;
            base.IsGoldAwarded = base.IsCompleted && base.PlayTime < goldTime;
            
            
            // PHASE 3: Display scores to gameResultPage and customResultsView
            gameResultPage.OpenPageInGroup();
            gameResultPage.SetScore((int)Score, (int)MaxScore);
            gameResultPage.SetPlayTime((int)PlayTime);
            gameResultPage.SetPointsEntries(PointsEntries);
        }
    }
}
