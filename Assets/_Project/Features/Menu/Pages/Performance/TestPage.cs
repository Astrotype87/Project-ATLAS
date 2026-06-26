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
    public class TestPage : UIPage
    {
        [Header("Components")]
        [SerializeField, Child] private ChapterTestScoreView[] chapterTestScoreViews;
        [SerializeField] private TMP_Text overallPreTestText;
        [SerializeField] private TMP_Text overallPostTestText;
        [SerializeField] private TMP_Text overallDifferenceText;
        [SerializeField] private Image resultImage;
        
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
            
            
            if (gameDataService)
                gameDataService = GameDataManager.Instance;
            
            if (levelManager)
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
                    
                    
                    int preTestMaxScore = preTestRecord != null ? preTestRecord.maxScore : preTestQuizSettings.totalItems;
                    int postTestMaxScore = postTestRecord != null ? postTestRecord.maxScore : postTestQuizSettings.totalItems;
                    
                    int preTestScore = preTestRecord != null ? preTestRecord.score : 0;
                    int postTestScore = postTestRecord != null ? postTestRecord.score : 0;
                    
                    allPreTestMax += preTestMaxScore;
                    allPostTestMax += postTestMaxScore;
                    
                    allPreTestScore += preTestScore;
                    allPostTestScore += postTestScore;
                    
                    allPreTestScoreDiff += preTestRecord != null && isTestCompleted ? preTestRecord.score : 0;
                    allPostTestScoreDiff += postTestRecord != null && isTestCompleted ? postTestRecord.score : 0;
                    
                    
                    chapterTestScoreViews[i].DisplayChapterTestScore(
                        i + 1, preTestScore, preTestMaxScore, postTestScore,postTestMaxScore,
                        preTestRecord != null, postTestRecord != null);
                }
                
                float difference = ((allPostTestScore / (float)allPostTestMax) - (allPreTestScore / (float)allPreTestMax)) * 100f;
                
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
            }
        }
        
        
        
        
        
        
        // [Header("Chapter Test Scores")]
        // public TMP_Text[] preTestTexts;
        // public TMP_Text[] postTestTexts;
        // public TMP_Text[] resultTexts;
        // public TMP_Text[] differenceTexts;

        // [Header("Chapter Status Icons (Improved / Retained / Declined)")]
        // public Image[] statusIcons;
        // public Sprite improvedIcon;
        // public Sprite retainedIcon;
        // public Sprite declinedIcon;

        // [Header("Overall Summary")]
        // public TMP_Text overallPreTestText;
        // public TMP_Text overallPostTestText;
        // public TMP_Text overallDifferenceText;
        // public Image overallStatusIcon;

        // [Header("Difference Status Icons")]
        // public Image[] differenceStatusIcons; // assign 10 images in Inspector


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

        //     if (records == null)
        //     {
        //         Debug.LogWarning("RecordsData is missing in TestPage!");
        //         return;
        //     }

        //     var preTests = records.preTestRecords;
        //     var postTests = records.postTestRecords;

        //     if (preTests == null || postTests == null)
        //     {
        //         Debug.LogWarning("Pre-test or Post-test data is null!");
        //         return;
        //     }

        //     int totalPreScore = 0;
        //     int totalPreMax = 0;
        //     int totalPostScore = 0;
        //     int totalPostMax = 0;

        //     for (int i = 0; i < 10; i++)
        //     {
        //         var pre = i < preTests.Count ? preTests[i] : null;
        //         var post = i < postTests.Count ? postTests[i] : null;

        //         // --- Pre-Test Display ---
        //         string preDisplay = pre != null ? $"{pre.score}/{pre.maxScore}" : "-";
        //         if (preTestTexts.Length > i) preTestTexts[i].text = preDisplay;

        //         // --- Post-Test Display ---
        //         string postDisplay = post != null ? $"{post.score}/{post.maxScore}" : "-";
        //         if (postTestTexts.Length > i) postTestTexts[i].text = postDisplay;

        //         // --- Result & Difference ---
        //         string result = "-";
        //         string diffDisplay = "-";
        //         Sprite iconToUse = retainedIcon;

        //         if (pre != null && post != null)
        //         {
        //             float prePercent = (float)pre.score / pre.maxScore;
        //             float postPercent = (float)post.score / post.maxScore;
        //             float diffPercent = (postPercent - prePercent) * 100f;

        //             // Determine result based on post-test performance
        //             if (postPercent >= 1f)
        //                 result = "PERFECT";
        //             else if (postPercent >= 0.5f)
        //                 result = "PASSED";
        //             else
        //                 result = "FAILED";

        //             // Determine trend icon
        //             if (diffPercent > 0)
        //                 iconToUse = improvedIcon;
        //             else if (diffPercent < 0)
        //                 iconToUse = declinedIcon;
        //             else
        //                 iconToUse = retainedIcon;

        //             // Totals
        //             totalPreScore += pre.score;
        //             totalPreMax += pre.maxScore;
        //             totalPostScore += post.score;
        //             totalPostMax += post.maxScore;

        //             diffDisplay = $"{diffPercent:+0;-0;0}%";
        //         }

        //         if (resultTexts.Length > i) resultTexts[i].text = result;
        //         if (differenceTexts.Length > i) differenceTexts[i].text = diffDisplay;
        //         if (statusIcons.Length > i && iconToUse != null)
        //             statusIcons[i].sprite = iconToUse;

        //         if (differenceStatusIcons.Length > i && iconToUse != null)
        //             differenceStatusIcons[i].sprite = iconToUse;
        //     }

        //     // --- Overall Summary ---
        //     if (totalPreMax > 0)
        //         overallPreTestText.text = $"{totalPreScore}/{totalPreMax}";
        //     else
        //         overallPreTestText.text = "-";

        //     if (totalPostMax > 0)
        //         overallPostTestText.text = $"{totalPostScore}/{totalPostMax}";
        //     else
        //         overallPostTestText.text = "-";

        //     if (totalPreMax > 0 && totalPostMax > 0)
        //     {
        //         float overallPrePercent = (float)totalPreScore / totalPreMax;
        //         float overallPostPercent = (float)totalPostScore / totalPostMax;
        //         float overallDiff = (overallPostPercent - overallPrePercent) * 100f;

        //         overallDifferenceText.text = $"{overallDiff:+0;-0;0}%";

        //         // Overall trend icon
        //         if (overallDiff > 0)
        //             overallStatusIcon.sprite = improvedIcon;
        //         else if (overallDiff < 0)
        //             overallStatusIcon.sprite = declinedIcon;
        //         else
        //             overallStatusIcon.sprite = retainedIcon;
        //     }
        //     else
        //     {
        //         overallDifferenceText.text = "-";
        //     }
        // }
    }
}
