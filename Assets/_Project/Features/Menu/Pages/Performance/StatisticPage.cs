using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using ProjectATLAS.GameData;
using ProjectATLAS.UI;
using ProjectATLAS.Utility;

namespace ProjectATLAS.Menu
{
    public class StatisticsPage : UIPage
    {
        [Header("Components")]
        [SerializeField] private TMP_Text timeSpentPlayingText;
        [SerializeField] private TMP_Text playsText;
        [SerializeField] private TMP_Text completeText;
        [SerializeField] private TMP_Text failText;
        [SerializeField] private TMP_Text timeViewingStatisticsText;
        [SerializeField] private RectTransform completeBar;
        
        private GameDataManager gameDataService;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            gameDataService = GameDataManager.Instance;
        }
        
        private void Update()
        {
            gameDataService.StatisticsData.timeSpentInStatistics += Time.deltaTime;
            RefreshTimeUI();
        }
        
        
        // UIPage METHODS
        public override void OpenPage()
        {
            base.OpenPage();
            
            UpdateUI();
        }
        
        public override void ClosePage()
        {
            base.ClosePage();
        }
        
        
        // PRIVATE METHODS
        private void UpdateUI()
        {
            if (!gameDataService) gameDataService = GameDataManager.Instance;
            if (!gameDataService) return;
            
            var statsData = gameDataService.StatisticsData;
            
            int completePlays = statsData.completePlays;
            int failedPlays = statsData.failedPlays;
            int totalPlays = completePlays + failedPlays;
            
            
            timeSpentPlayingText.text = FormatSecondsToTimeDisplay(statsData.timeSpentPlaying);
            playsText.text = $"{totalPlays}";
            
            completeText.text = $"Complete ({completePlays})";
            failText.text = $"Fail ({failedPlays})";
            
            timeViewingStatisticsText.text = FormatSecondsToTimeDisplay(statsData.timeSpentInStatistics);
            
            Vector2 anchorMax = completeBar.anchorMax;
            anchorMax.x = (completePlays / (float)totalPlays).NaNInfSafe();
            completeBar.anchorMax = anchorMax;
        }
        
        private void RefreshTimeUI()
        {
            if (!gameDataService) return;
            
            var statsData = gameDataService.StatisticsData;
            
            timeSpentPlayingText.text = FormatSecondsToTimeDisplay(statsData.timeSpentPlaying);
            timeViewingStatisticsText.text = FormatSecondsToTimeDisplay(statsData.timeSpentInStatistics);
        }
        
        
        private static string FormatSecondsToTimeDisplay(double time)
        {
            int hours = (int)(time / 3600);
            int minutes = (int)((time % 3600) / 60);
            int seconds = (int)(time % 60);
            
            return $"{hours:00}:{minutes:00}:{seconds:00}";
        }
        
        
        // [Header("UI Elements")]
        // public Slider progressSlider; // Make sure interactable is OFF in inspector
        // public TMP_Text completeText;
        // public TMP_Text failedText;
        // public TMP_Text timePlayingText;
        // public TMP_Text timeInStatisticsText;

        // private void OnEnable()
        // {
        //     // Reload latest data whenever this tab opens
        //     _ = RefreshStatisticsAsync();
        // }

        // private async Task RefreshStatisticsAsync()
        // {
        //     if (GameDataService.Instance != null)
        //     {
        //         await GameDataService.Instance.LoadDataAsync();

        //         var stats = GameDataService.Instance.StatisticsData;

        //         // Update slider (max 10)
        //         progressSlider.maxValue = 10;
        //         progressSlider.value = Mathf.Clamp(stats.completePlays, 0, 10);

        //         // Update texts
        //         completeText.text = $"{stats.completePlays}";
        //         failedText.text = $"{stats.failedPlays}";
        //         timePlayingText.text = $"{stats.timeSpentPlaying:F2}s";
        //         timeInStatisticsText.text = $"{stats.timeSpentInStatistics:F2}s";
        //     }
        // }
    }
    
}
