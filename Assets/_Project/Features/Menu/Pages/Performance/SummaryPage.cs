using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KBCore.Refs;

using ProjectATLAS.GameData;
using ProjectATLAS.UI;
using ProjectATLAS.Gameplay;
using ProjectATLAS.Quiz;

namespace ProjectATLAS.Menu
{
    public class SummaryPage : UIPage
    {
        [Header("Components")]
        [SerializeField] private TMP_Text campaignScoreText;
        [SerializeField] private TMP_Text overallPreTestText;
        [SerializeField] private TMP_Text overallPostTestText;
        [SerializeField] private TMP_Text overallDifferenceText;
        [SerializeField] private Image resultImage;
        [SerializeField, Child] private BestLevelScoreView[] bestLevelScoreViews;
        
        [Header("Test Settings")]
        [SerializeField] private QuizSettings preTestQuizSettings;
        [SerializeField] private QuizSettings postTestQuizSettings;
        
        [Header("Images")]
        [SerializeField] private Sprite improvedIcon;
        [SerializeField] private Sprite declinedIcon;
        [SerializeField] private Sprite retainedIcon;
        
        private GameDataManager gameDataService;
        private LevelManager levelManager;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            gameDataService = GameDataManager.Instance;
            levelManager = LevelManager.Instance;
        }
        
        
        // UIPage METHODS
        public override void OpenPage()
        {
            base.OpenPage();
            Debug.Log($"SummaryPage.OpenPage()");
            
            UpdateUI();
        }
        
        
        // PRIVATE METHODS
        private void UpdateUI()
        {
            if (!gameDataService)
                gameDataService = GameDataManager.Instance;
            
            if (!levelManager)
                levelManager = LevelManager.Instance;
            
            if (gameDataService && levelManager)
            {
                int chapters = 10;
                
                int allPreTestMax = 0;
                int allPostTestMax = 0;
                
                int allPreTestScore = 0;
                int allPostTestScore = 0;
                
                int allPreTestScoreDiff = 0;
                int allPostTestScoreDiff = 0;
                
                bool isFirstChapterCompleted = levelManager.LastCompletedPostTest() >= 1;
                
                for (int i = 0; i < chapters; i++)
                {
                    string preTestID = $"PRE-{i + 1:00}";
                    string postTestID = $"POST-{i + 1:00}";
                    
                    bool isPreTestCompleted = gameDataService.CampaignData.IsTestCompleted(preTestID);
                    bool isPostTestCompleted = gameDataService.CampaignData.IsTestCompleted(postTestID);
                    bool isTestCompleted = isPreTestCompleted && isPostTestCompleted;
                    
                    var preTestRecord = gameDataService.RecordsData.GetBestTestScore(preTestID);
                    var postTestRecord = gameDataService.RecordsData.GetBestTestScore(postTestID);
                    
                    
                    allPreTestMax += preTestRecord != null ? preTestRecord.maxScore : preTestQuizSettings.totalItems;
                    allPostTestMax += postTestRecord != null ? postTestRecord.maxScore : postTestQuizSettings.totalItems;
                    
                    allPreTestScore += preTestRecord != null ? preTestRecord.score : 0;
                    allPostTestScore += postTestRecord != null ? postTestRecord.score : 0;
                    
                    allPreTestScoreDiff += preTestRecord != null && isTestCompleted ? preTestRecord.score : 0;
                    allPostTestScoreDiff += postTestRecord != null && isTestCompleted ? postTestRecord.score : 0;
                }
                
                float difference = ((allPostTestScore / (float)allPostTestMax) - (allPreTestScore / (float)allPreTestMax)) * 100f;
                
                if (campaignScoreText) campaignScoreText.text = gameDataService.RecordsData.GetCampaignScore().ToString("N0");
                if (overallPreTestText) overallPreTestText.text = $"{allPreTestScore} / {allPreTestMax}";
                if (overallPostTestText) overallPostTestText.text = $"{allPostTestScore} / {allPostTestMax}";
                if (overallDifferenceText) overallDifferenceText.text = isFirstChapterCompleted ? $"{difference:00.00}%" : "-----";
                
                if (resultImage)
                {
                    float preTestPercent = allPreTestScoreDiff / allPreTestMax;
                    float postTestPercent = allPostTestScoreDiff / allPostTestMax;
                    
                    resultImage.sprite = !isFirstChapterCompleted || preTestPercent == postTestPercent ? retainedIcon
                        : postTestPercent >= preTestPercent ? improvedIcon
                        : declinedIcon;
                }
                
                int levels = 41;
                for (int i = 0; i < levels; i++)
                {
                    string levelID = $"LVL-{i + 1:00}";
                    var levelRecord = gameDataService.RecordsData.GetBestLevelPoints(levelID);
                    
                    if (levelRecord == null)
                    {
                        bestLevelScoreViews[i].DisplayScore(i + 1, 0);
                        bestLevelScoreViews[i].SetDisable(true);
                    }
                    else
                    {
                        bestLevelScoreViews[i].DisplayScore(i + 1, levelRecord.levelPoints);
                        bestLevelScoreViews[i].SetDisable(false);
                    }
                }
            }
        }
        
        
        // [Header("Campaign Score Section")]
        // public TMP_Text totalCampaignScoreText;
        // public TMP_Text[] levelBestScoreTexts; 

        // [Header("Tests Section")]
        // public TMP_Text overallPreTestText;
        // public TMP_Text overallPostTestText;
        // public TMP_Text overallDifferenceText;

        // private void OnEnable()
        // {
        //     if (GameDataService.Instance != null)
        //         GameDataService.Instance.OnGameDataLoaded += HandleGameDataLoaded;
        // }

        // private void OnDisable()
        // {
        //     if (GameDataService.Instance != null)
        //         GameDataService.Instance.OnGameDataLoaded -= HandleGameDataLoaded;
        // }

        // protected override void Start()
        // {
        //     base.Start();
            
        //     TryUpdateDisplay();
        // }

        // private void HandleGameDataLoaded(ProjectATLAS.GameData.GameData _)
        // {
        //     TryUpdateDisplay();
        // }

        // private void TryUpdateDisplay()
        // {
        //     var records = GameDataService.Instance?.RecordsData;
        //     var campaign = GameDataService.Instance?.CampaignData;

        //     if (records == null || campaign == null)
        //     {
        //         Debug.LogWarning(" Missing campaign or record data in PerformancePage!");
        //         return;
        //     }


        //     //  Total Campaign Score

        //     int totalScore = records.GetCampaignScore();
        //     if (totalCampaignScoreText)
        //         totalCampaignScoreText.text = totalScore.ToString();


        //     //  Level Best Scores (based on levelPoints)

        //     string[] levelIDs = {
        //         "LVL-01","LVL-02","LVL-03","LVL-04","LVL-05",
        //         "LVL-06","LVL-07","LVL-08","LVL-09","LVL-10"
        //     };

        //     for (int i = 0; i < levelBestScoreTexts.Length; i++)
        //     {
        //         string id = i < levelIDs.Length ? levelIDs[i] : null;

        //         if (string.IsNullOrEmpty(id))
        //         {
        //             levelBestScoreTexts[i].text = "-";
        //             continue;
        //         }

        //         var record = records.levelRecords
        //             .Where(r => r.levelID == id)
        //             .OrderByDescending(r => r.levelPoints)
        //             .FirstOrDefault();

        //         if (record != null && record.levelPoints > 0)
        //             levelBestScoreTexts[i].text = record.levelPoints.ToString();
        //         else
        //             levelBestScoreTexts[i].text = "-";
        //     }

        //     //  Pre-Test / Post-Test Summary

        //     int totalPre = records.preTestRecords?.Sum(r => r.score) ?? 0;
        //     int totalPost = records.postTestRecords?.Sum(r => r.score) ?? 0;
        //     int difference = totalPost - totalPre;

        //     if (overallPreTestText) overallPreTestText.text = totalPre.ToString();
        //     if (overallPostTestText) overallPostTestText.text = totalPost.ToString();
        //     if (overallDifferenceText) overallDifferenceText.text = difference.ToString();
    }
}
