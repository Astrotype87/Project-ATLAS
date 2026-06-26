using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using ProjectATLAS.GameData;
using ProjectATLAS.UI;
using KBCore.Refs;

namespace ProjectATLAS.Menu
{
    using static ProjectATLAS.GameData.RecordsData;
    
    public class RecordsPage : UIPage
    {
        [Header("Settings")]
        [SerializeField] private int currentLevel = 1;
        [SerializeField] private int maxLevel = 41;
        [SerializeField] private SortMode sortMode;
        
        public enum SortMode { BestScore, BestTime, RecentPlay }
        
        [Header("Components")]
        [SerializeField] private Button lastButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private TMP_Text levelText;
        [Space]
        [SerializeField] private UIToggleButton bestScoreToggle;
        [SerializeField] private UIToggleButton bestTimeToggle;
        [SerializeField] private UIToggleButton recentPlayToggle;
        [Space]
        [SerializeField] private TMP_Text bestScoreText;
        [SerializeField] private TMP_Text bestTimeText;
        [SerializeField] private TMP_Text lastPlayedText;
        [Space]
        [SerializeField, Child] private LevelRecordView[] levelRecordViews;
        
        private GameDataManager gameDataService;
        private List<LevelRecord> levelRecords = new();
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            lastButton.onClick.AddListener(LastButton_OnClick);
            nextButton.onClick.AddListener(NextButton_OnClick);
            
            bestScoreToggle.OnValueChanged += BestScoreToggle_OnValueChanged;
            bestTimeToggle.OnValueChanged += BestTimeToggle_OnValueChanged;
            recentPlayToggle.OnValueChanged += RecentPlayToggle_OnValueChanged;
            
            gameDataService = GameDataManager.Instance;
        }
        
        
        // UIPage METHODS
        public override void OpenPage()
        {
            base.OpenPage();
            
            UpdateUI();
        }
        
        
        // EVENT LISTENER METHODS
        private void LastButton_OnClick()
        {
            currentLevel = Mathf.Clamp(currentLevel - 1, 1, maxLevel);
            UpdateUI();
        }
        
        private void NextButton_OnClick()
        {
            currentLevel = Mathf.Clamp(currentLevel + 1, 1, maxLevel);
            UpdateUI();
        }
        
        private void BestScoreToggle_OnValueChanged(UIToggleButton toggleButton, bool value)
        {
            if (value)
            {
                sortMode = SortMode.BestScore;
                UpdateUI();
            }
        }
        
        private void BestTimeToggle_OnValueChanged(UIToggleButton toggleButton, bool value)
        {
            if (value)
            {
                sortMode = SortMode.BestTime;
                UpdateUI();
            }
        }
        
        private void RecentPlayToggle_OnValueChanged(UIToggleButton toggleButton, bool value)
        {
            if (value)
            {
                sortMode = SortMode.RecentPlay;
                UpdateUI();
            }
        }
        
        
        // PRIVATE METHODS
        private void UpdateUI()
        {
            if (!gameDataService) gameDataService = GameDataManager.Instance;
            if (gameDataService)
            {
                levelText.text = currentLevel.ToString();
                
                // Get records from best score, shortest time, and latest date time
                string levelID = $"LVL-{currentLevel:00}";
                var bestScoreRecord = gameDataService.RecordsData.GetBestLevelPoints(levelID);
                var bestTimeRecord = gameDataService.RecordsData.GetBestLevelTime(levelID);
                var lastPlayedRecord = gameDataService.RecordsData.GetLastTimePlayedLevel(levelID);
                
                // Display best score, shortest time, and latest date time
                bestScoreText.text = bestScoreRecord != null ? bestScoreRecord.levelPoints.ToString("N0") : "---";
                if (bestTimeRecord != null)
                {
                    float time = bestTimeRecord.time;
                    int minutes = (int)(time / 60f);
                    int seconds = (int)(time % 60f);
                    int centiseconds = (int)((time - Mathf.Floor(time)) * 100f);
                    string formatted = $"{minutes:00}:{seconds:00}:{centiseconds:00}";
                    
                    bestTimeText.text = formatted;
                }
                else
                {
                    bestTimeText.text = "---";
                }
                
                if (lastPlayedRecord != null
                    && DateTime.TryParseExact(lastPlayedRecord.dateTime,
                        Standard.GameData_DateTimeFormat,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime dt))
                {
                    lastPlayedText.text = dt.ToString("yyyy/MM/dd\nhh:mm:ss tt");
                }
                else
                {
                    lastPlayedText.text = "---";
                }
                
                
                // Get records within chosen level and selected sorting
                int items = 10;
                levelRecords = gameDataService.RecordsData.levelRecords
                    .Where(r => r.levelID == $"LVL-{currentLevel:00}").ToList();
                var currentRecords = sortMode switch
                {
                    SortMode.BestScore => levelRecords
                        .OrderByDescending(r => r.levelPoints)
                        .Take(items)
                        .ToList(),
                    
                    SortMode.BestTime => levelRecords
                        .OrderBy(r => r.time) // Lower time is better
                        .Take(items)
                        .ToList(),
                    
                    SortMode.RecentPlay => levelRecords
                        .OrderByDescending(r =>
                        {
                            if (DateTime.TryParseExact(r.dateTime,
                                Standard.GameData_DateTimeFormat,
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.None,
                                out DateTime dt))
                                    return dt;
                            return DateTime.MinValue;
                        })
                        .Take(items)
                        .ToList(),
                    
                    _ => levelRecords.Take(10).ToList()
                };
                
                // Display records for selected level and sorting to the list
                for (int i = 0; i < levelRecordViews.Length; i++)
                {
                    if (i < currentRecords.Count)
                    {
                        var record = currentRecords[i];
                        levelRecordViews[i].gameObject.SetActive(true);
                        levelRecordViews[i].DisplayLevelRecord(i + 1, record.levelPoints, record.time, record.dateTime);
                    }
                    else
                    {
                        levelRecordViews[i].gameObject.SetActive(false);
                    }
                }
            }
        }
        
        
    //     [Header("UI Elements")]
    //     public TMP_Text levelText;
    //     public Button leftButton;
    //     public Button rightButton;
        
    //     public Button bestScoreButton;
    //     public Button bestTimeButton;
    //     public Button recentPlayButton;
        
    //     [Header("Records Display")]
    //     public TMP_Text[] scoreTexts;     
    //     public TMP_Text[] timeTexts;      
    //     public TMP_Text[] dateTimeTexts;   
        
    //     private int currentLevel = 1;     
    //     private string selectedFilter = "BestScore";
        
    //     private List<LevelRecord> currentRecords = new();
        
    //     protected override void Start()
    //     {
    //         base.Start();
            
    //         leftButton.onClick.AddListener(OnLeftClicked);
    //         rightButton.onClick.AddListener(OnRightClicked);
            
    //         bestScoreButton.onClick.AddListener(() => OnFilterClicked("BestScore"));
    //         bestTimeButton.onClick.AddListener(() => OnFilterClicked("BestTime"));
    //         recentPlayButton.onClick.AddListener(() => OnFilterClicked("RecentPlay"));
            
    //         // Subscribe to GameData loaded event
    //         GameDataService.Instance.OnGameDataLoaded += OnGameDataLoaded;
            
    //         // // Try loading data (if not already loaded)
    //         // GameDataService.Instance.LoadData();
    //     }
        
    //     private void OnDestroy()
    //     {
    //         if (GameDataService.Instance != null)
    //             GameDataService.Instance.OnGameDataLoaded -= OnGameDataLoaded;
    //     }
        
    //     // Called when game data is loaded
    //     private void OnGameDataLoaded(GameData.GameData data)
    //     {
    //         currentLevel = 1;
    //         OnFilterClicked("BestScore"); // Apply initial filter and refresh display
    //     }
        
        
        
        
    //     private void OnLeftClicked()
    //     {
    //         if (currentLevel > 1) currentLevel--;
    //         RefreshDisplay();
    //     }
        
    //     private void OnRightClicked()
    //     {
    //         if (currentLevel < 10) currentLevel++;
    //         RefreshDisplay();
    //     }
        
    //     private void OnFilterClicked(string filter)
    //     {
    //         selectedFilter = filter;
            
    //         // Toggle button visuals (optional, highlights selected filter)
    //         bestScoreButton.interactable = filter != "BestScore";
    //         bestTimeButton.interactable = filter != "BestTime";
    //         recentPlayButton.interactable = filter != "RecentPlay";
            
    //         RefreshDisplay();
    //     }
        
    //     private void RefreshDisplay()
    //     {
    //         if (levelText != null)
    //             levelText.text = $"Level {currentLevel}";
            
    //         var allRecords = GameDataService.Instance.RecordsData.levelRecords;
            
    //         // Filter records by current level
    //         var levelRecords = allRecords.Where(r => r.levelID == $"LVL-{currentLevel:D2}").ToList();
            
    //          // Apply selected filter
    //         currentRecords = selectedFilter switch
    //         {
    //             "BestScore" => levelRecords
    //                 .OrderByDescending(r => r.score)
    //                 .ThenByDescending(r => r.levelPoints)
    //                 .Take(10)
    //                 .ToList(),

    //             "BestTime" => levelRecords
    //                 .OrderBy(r => r.time) // Lower time is better
    //                 .Take(10)
    //                 .ToList(),

    //             "RecentPlay" => levelRecords
    //                 .OrderByDescending(r =>
    //                 {
    //                     if (DateTime.TryParseExact(r.dateTime,
    //                                             "M/d/yyyy h:mm:ss tt",
    //                                             CultureInfo.InvariantCulture,
    //                                             DateTimeStyles.None,
    //                                             out DateTime dt))
    //                         return dt;
    //                     return DateTime.MinValue;
    //                 })
    //                 .Take(10)
    //                 .ToList(),

    //             _ => levelRecords.Take(10).ToList()
    //         };

    //         // Clear previous display
    //         for (int i = 0; i < 10; i++)
    //         {
    //             scoreTexts[i].text = "";
    //             timeTexts[i].text = "";
    //             dateTimeTexts[i].text = "";
    //         }

    //         // Fill display
    //         for (int i = 0; i < currentRecords.Count && i < 10; i++)
    //         {
    //             scoreTexts[i].text = $"{currentRecords[i].levelPoints}";
    //             timeTexts[i].text = $"{currentRecords[i].time:F2}s";
    //             dateTimeTexts[i].text = currentRecords[i].dateTime;
    //         }

    //     }
    
    }
}
