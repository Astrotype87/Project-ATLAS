using UnityEngine;
using UnityEngine.UI;
using TMPro;

using ProjectATLAS.Gameplay;

namespace ProjectATLAS.Minigame.Mini01_HovercraftAssembly
{
    public class Mini01_HovercraftAssembly : MinigameSystem
    {
        [Header("Mini 01: Hovercraft Assembly")]
        [SerializeField] private AssemblyPart[] assemblyParts;
        [SerializeField] private Vector2 easySnap = new(0.1f, 0.1f);
        [SerializeField] private Vector2 mediumSnap = new(0.05f, 0.05f);
        [SerializeField] private Vector2 hardSnap = new(0.025f, 0.025f);
        /// <summary> Granted gold medal when complete minigame under goldTIme seconds. </summary>
        [SerializeField] private float goldTime = 30f;
        
        [Header("Mini 01: UI")]
        [SerializeField] private AssemblyResultsView assemblyResultsView;
        
        
        // MONOBEHAVIOUR METHODS
        // private void Awake()
        // {
            
        // }
        
        // EVENT LISTENER METHODS
        // private void AnyUIElement_OnEvent()
        // {
            
        // }
        
        
        // PROTECTED OVERRIDE METHODS
        protected override void OnInitializeMinigame(Difficulty difficulty)
        {
            Vector2 gridSnap = difficulty switch
            {
                Difficulty.Easy => easySnap,
                Difficulty.Medium => mediumSnap,
                Difficulty.Hard => hardSnap,
                _ => easySnap
            };
            
            // Update snap based on difficulty
            foreach (var part in assemblyParts)
            {
                part.SetGridSnap(gridSnap);
            }
        }
        
        protected override void OnStartMinigame()
        {
            
        }
        
        protected override void OnFinishMinigame()
        {
            // PHASE 1: Calculate score
            int correctlyAssembled = 0;
            foreach (var part in assemblyParts)
            {
                if (part.CurrentPosition == part.AssembledPosition)
                    correctlyAssembled++;
            }
            
            
            // PHASE 2: Set minigame score data
            base.Score = correctlyAssembled;
            base.MaxScore = assemblyParts.Length;
            base.PlayTime = (float)base.GetPlayTime().TotalSeconds;
            
            int minigamePoints = (int)(base.Score * base.scoreMultiplier);
            int timeBonus = (int)(CalculateTimeBonus(base.PlayTime) * base.ScorePercentage);
            base.PointsEntries = new PointsEntry[]
            {
                new("Score", minigamePoints),
                new("Time Bonus", timeBonus),
            };
            
            base.IsCompleted = base.ScorePercentage >= 0.5f;
            base.IsBronzeAwarded = base.ScorePercentage >= 0.5f;
            base.IsSilverAwarded = base.ScorePercentage >= 1f;
            base.IsGoldAwarded = base.ScorePercentage >= 1 && base.PlayTime <= goldTime;
            
            
            // PHASE 3: Display scores to gameResultPage and customResultsView
            gameResultPage.OpenPageInGroup();
            gameResultPage.SetScore((int)Score, (int)MaxScore);
            gameResultPage.SetPlayTime((int)PlayTime);
            gameResultPage.SetPointsEntries(PointsEntries);
            
            var results = new (string partName, Vector2 correct, Vector2 answer)[assemblyParts.Length];
            for (int i = 0; i < assemblyParts.Length; i++)
            {
                var part = assemblyParts[i];
                results[i] = (part.Name, part.AssembledPosition, part.CurrentPosition);
            }
            assemblyResultsView.DisplayResults(results);
        }
    }
}
