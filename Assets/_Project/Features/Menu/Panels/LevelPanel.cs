using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using ProjectATLAS.GameData;
using ProjectATLAS.Gameplay;
using System.Collections;
using PrimeTween;
using ProjectATLAS.UI;

namespace ProjectATLAS.Menu
{
    public class LevelPanel : MonoBehaviour
    {
        [Header("Display")]
        [SerializeField] private bool visible;
        [SerializeField] private CampaignLevelData campaignLevelData;
        [SerializeField] private LevelType levelType;
        [SerializeField] private string displayName = "Player";
        [SerializeField] private string level;
        [SerializeField] private new string name;
        [SerializeField] private string info;
        [SerializeField, TextArea] private string description;
        [SerializeField] private string testScore;
        [SerializeField] private string bestScore;
        [SerializeField] private bool isCompleted;
        [SerializeField] private bool isBronze;
        [SerializeField] private bool isSilver;
        [SerializeField] private bool isGold;
        [SerializeField] private string completeObjective;
        [SerializeField] private string bronzeObjective;
        [SerializeField] private string silverObjective;
        [SerializeField] private string goldObjective;
        [SerializeField] private Difficulty difficulty;
        [SerializeField, TextArea] private string mechanics;
        [SerializeField, TextArea] private string scoring;
        
        [Header("Details Tab Components")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text infoText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text testScoreText;
        [SerializeField] private GameObject bestScoreLabel;
        [SerializeField] private TMP_Text bestScoreText;
        [SerializeField] private TMP_Text completeObjectiveText;
        [SerializeField] private TMP_Text bronzeObjectiveText;
        [SerializeField] private TMP_Text silverObjectiveText;
        [SerializeField] private TMP_Text goldObjectiveText;
        [SerializeField] private GameObject[] incompletedIcons;
        [SerializeField] private GameObject[] completedIcons;
        [SerializeField] private GameObject[] emptyMedals;
        [SerializeField] private GameObject[] bronzeMedals;
        [SerializeField] private GameObject[] silverMedals;
        [SerializeField] private GameObject[] goldMedals;
        
        [Header("Scoring Tab Components")]
        [SerializeField] private TMP_Text mechanicsText;
        
        [Header("Scoring Tab Components")]
        [SerializeField] private TMP_Text scoringText;
        [SerializeField] private TimeBonusTableView timeBonusTableView;
        
        [Header("Tab Toggle & Pages")]
        [SerializeField] private UIToggleButton detailsToggle;
        [SerializeField] private UIToggleButton mechanicsToggle;
        [SerializeField] private UIToggleButton scoringToggle;
        [SerializeField] private UIPage detailsPage;
        [SerializeField] private UIPage mechanicsPage;
        [SerializeField] private UIPage scoringPage;
        
        [Header("Bottom Components")]
        // [SerializeField] private GameObject resourcesLabel;
        // [SerializeField] private Button guideButton;
        // [SerializeField] private Button termsButton;
        [SerializeField] private TMP_Dropdown difficultyDropdown;
        [SerializeField] private Button playButton;
        
        [Header("Animation")]
        [SerializeField] private float startOffset = 400;
        [SerializeField] private float animationTime = 0.4f;
        [SerializeField] private Ease openEase;
        [SerializeField] private Ease closeEase;
        
        private GameDataManager gameDataService;
        private LevelManager levelManager;
        
        private RectTransform rectTransform;
        private float originalXPos;
        private float originalXSize;
        
        private Sequence openAnimation;
        private Sequence closeAnimation;
        
        
        
        // PROPERTIES
        public bool IsAnimating { get; private set; }
        
        public event Action<CampaignLevelData> OnRecordsClicked;
        public event Action<int> OnLeaderboardsClicked;
        public event Action<int> OnGuideClicked;
        public event Action<int> OnTermsClicked;
        public event Action<CampaignLevelData, Difficulty> OnPlayLevelClicked;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            // guideButton.onClick.AddListener(GuideButton_onClick);
            // termsButton.onClick.AddListener(TermsButton_onClick);
            difficultyDropdown.onValueChanged.AddListener(DifficultyDropdown_onValueChanged);
            playButton.onClick.AddListener(PlayButton_onClick);
            
            gameDataService = GameDataManager.Instance;
            if (gameDataService == null)
                Debug.LogWarning("GameDataService not found!");
            
            levelManager = LevelManager.Instance;
            if (levelManager == null)
                Debug.LogWarning("LevelManager not found!");
            
            rectTransform = transform as RectTransform;
            
            originalXPos = rectTransform.anchoredPosition.x;
            originalXSize = rectTransform.sizeDelta.x;
        }
        
        private void OnValidate()
        {
            DisplayLevelDetails(campaignLevelData);
        }
        
        
        // PUBLIC METHODS
        public void OpenPanel(CampaignLevelData campaignLevelData, bool isAnimated = true)
        {
            if (this.campaignLevelData == campaignLevelData) return;
            if (IsAnimating) return;
            
            this.campaignLevelData = campaignLevelData;
            DisplayLevelDetails(campaignLevelData);
            
            if (openAnimation.isAlive) openAnimation.Stop();
            if (closeAnimation.isAlive) closeAnimation.Stop();
            
            if (isAnimated)
            {
                StartCoroutine(AnimateOpen());
            }
            else
            {
                RectTransform rectTransform = transform as RectTransform;
                Vector2 anchoredPosition = rectTransform.anchoredPosition;
                anchoredPosition.x = originalXPos;
                rectTransform.anchoredPosition = anchoredPosition;
                
                canvasGroup.alpha = 1;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }
        
        public void ClosePanel(bool isAnimated = true)
        {
            this.campaignLevelData = null;
            
            if (openAnimation.isAlive) openAnimation.Stop();
            if (closeAnimation.isAlive) closeAnimation.Stop();
            
            if (isAnimated)
            {
                StartCoroutine(AnimateClose());
            }
            else
            {
                RectTransform rectTransform = transform as RectTransform;
                Vector2 anchoredPosition = rectTransform.anchoredPosition;
                anchoredPosition.x = originalXPos + originalXSize;
                rectTransform.anchoredPosition = anchoredPosition;
                
                canvasGroup.alpha = 0;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }
        
        
        
        // PRIVATE METHODS
        public IEnumerator AnimateOpen()
        {
            IsAnimating = true;
            
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            visible = true;
            
            openAnimation = Sequence.Create()
                .Chain(Tween.UIAnchoredPositionX(rectTransform, originalXPos + startOffset, originalXPos, animationTime, openEase))
                .Group(Tween.Alpha(canvasGroup, 0, 1, animationTime, openEase));
            yield return openAnimation.ToYieldInstruction();
            
            IsAnimating = false;
        }
        
        public IEnumerator AnimateClose()
        {
            IsAnimating = true;
            
            closeAnimation = Sequence.Create()
                .Chain(Tween.UIAnchoredPositionX(rectTransform, originalXPos + startOffset, animationTime, closeEase))
                .Group(Tween.Alpha(canvasGroup, 1, 0, animationTime, closeEase));
            yield return closeAnimation.ToYieldInstruction();
            
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            visible = false;
            
            IsAnimating = false;
        }
        
        
        // PRIVATE METHODS
        private void InterruptAnimation()
        {
            
        }
        
        private void DisplayLevelDetails(CampaignLevelData campaignLevelData)
        {
            if (campaignLevelData)
            {
                UpdateDetailsData(campaignLevelData);
                LoadInformationFromGameData(campaignLevelData);
            }
            UpdateDetailsDisplay();
            UpdateTabsAndPages();
        }
        
        private void UpdateDetailsData(CampaignLevelData campaignLevelData)
        {
            this.levelType = campaignLevelData.Type;
            
            string prefix = campaignLevelData.Type switch
            {
                LevelType.PreTest => $"PRE-TEST",
                LevelType.PostTest => $"POST-TEST",
                _ => $"LEVEL",
            };
            this.level = $"{prefix} {campaignLevelData.Number} • {campaignLevelData.Type}";
            this.name = campaignLevelData.Title;
            this.info = campaignLevelData.Type switch
            {
                LevelType.PreTest => $"Intro Dialogue > Quiz",
                LevelType.Lesson => $"Lesson Dialogue > Quiz",
                LevelType.Simulation => $"Short Dialogue > Simulation",
                LevelType.Challenge => $"Short Dialogue > Minigame",
                LevelType.PostTest => $"Quiz > Closing Dialogue",
                _ => "",
            };
            this.description = campaignLevelData.Description;
            
            if (campaignLevelData.Type == LevelType.PreTest
                || campaignLevelData.Type == LevelType.PostTest)
            {
                TestData testData = campaignLevelData as TestData;
                completeObjective = testData.Objectives;
            }
            else
            {
                LevelData levelData = campaignLevelData as LevelData;
                bronzeObjective = levelData.BronzeObjective.Replace("{goldTime}", levelData.GoldTime.ToString());
                silverObjective = levelData.SilverObjective.Replace("{goldTime}", levelData.GoldTime.ToString());
                goldObjective = levelData.GoldObjective.Replace("{goldTime}", levelData.GoldTime.ToString());
                
                // Display time bonus
                timeBonusTableView.DisplayTimeBonus(levelData.TimeBonus);
            }
            
            this.mechanics = campaignLevelData.Mechanics;
            this.scoring = campaignLevelData.Scoring;
        }
        
        private void LoadInformationFromGameData(CampaignLevelData campaignLevelData)
        {
            if (levelManager == null)
                levelManager = LevelManager.Instance;
            
            if (levelManager != null && !levelManager.Equals(null))
            {
                displayName = levelManager.DisplayName;
                difficulty = levelManager.SelectedDifficulty;
                
                bool hasGameDataService = gameDataService != null && !gameDataService.Equals(null);
                    
                if (campaignLevelData.Type == LevelType.PreTest || campaignLevelData.Type == LevelType.PostTest)
                {
                    TestData testData = campaignLevelData as TestData;
                    CampaignData.Test test = testData != null ? levelManager.GetTestGameData(testData) : null;
                    RecordsData.TestRecord testRecord = null;
                    
                    if (hasGameDataService && gameDataService.RecordsData != null)
                        testRecord = gameDataService.RecordsData.GetBestTestScore(testData != null ? testData.ID : "");
                    
                    isCompleted = test != null && test.isCompleted;
                    testScore = testRecord != null
                        ? $"{testRecord.score}/{testRecord.maxScore}"
                        : "Not Taken";
                }
                else
                {
                    LevelData levelData = campaignLevelData as LevelData;
                    CampaignData.Level levelGameData = levelData != null ? levelManager.GetLevelGameData(levelData) : null;
                    
                    isBronze = levelGameData != null && levelGameData.isBronze;
                    isSilver = levelGameData != null && levelGameData.isSilver;
                    isGold = levelGameData != null && levelGameData.isGold;
                    
                    RecordsData.LevelRecord levelRecord = null;
                    if (hasGameDataService && gameDataService.RecordsData != null)
                        levelRecord = gameDataService.RecordsData.GetBestLevelPoints(levelData != null ? levelData.ID : "");
                    
                    bestScore = levelRecord != null ? levelRecord.levelPoints.ToString() : "0";
                }
            }
            else
            {
                displayName = "Player";
                isCompleted = false;
                isBronze = false;
                isSilver = false;
                isGold = false;
                bestScore = "0";
            }
        }
        
        private void UpdateDetailsDisplay()
        {
            canvasGroup.alpha = visible ? 1 : 0;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
            
            if (levelText) levelText.text = level;
            if (nameText) nameText.text = name;
            if (infoText) infoText.text = info;
            if (descriptionText) descriptionText.text = description.Replace("{player}", displayName);
            
            bool isTest = levelType == LevelType.PreTest
                || levelType == LevelType.PostTest;
            bool isLevel = !isTest;
            
            if (testScoreText)
            {
                testScoreText.gameObject.SetActive(isTest);
                testScoreText.text = testScore;
            }
            
            if (bestScoreText)
            {
                bestScoreText.gameObject.SetActive(isLevel);
                bestScoreText.text = bestScore;
            }
            
            bestScoreLabel.SetActive(isLevel);
            
            foreach (var incompletedIcon in incompletedIcons) {
                incompletedIcon.SetActive(isTest);
            }
            foreach (var completedIcon in completedIcons) {
                completedIcon.SetActive(isTest && isCompleted);
            }
            foreach (var emptyMedal in emptyMedals) {
                emptyMedal.SetActive(isLevel);
            }
            foreach (var bronzeMedal in bronzeMedals) {
                bronzeMedal.SetActive(isLevel && isBronze);
            }
            foreach (var silverMedal in silverMedals) {
                silverMedal.SetActive(isLevel && isSilver);
            }
            foreach (var goldMedal in goldMedals) {
                goldMedal.SetActive(isLevel && isGold);
            }
            
            if (completeObjectiveText)
            {
                completeObjectiveText.gameObject.SetActive(isTest);
                completeObjectiveText.text = completeObjective;
            }
            if (bronzeObjectiveText)
            {
                bronzeObjectiveText.gameObject.SetActive(isLevel);
                bronzeObjectiveText.text = bronzeObjective;
            }
            if (silverObjectiveText)
            {
                silverObjectiveText.gameObject.SetActive(isLevel);
                silverObjectiveText.text = silverObjective;
            }
            if (goldObjectiveText)
            {
                goldObjectiveText.gameObject.SetActive(isLevel);
                goldObjectiveText.text = goldObjective;
            }
            
            bool isLesson = levelType == LevelType.Lesson;
            
            // if (resourcesLabel) resourcesLabel.SetActive(isLesson);
            // if (guideButton) guideButton.gameObject.SetActive(isLesson);
            // if (termsButton) termsButton.gameObject.SetActive(isLesson);
            
            if (difficultyDropdown)
            {
                difficultyDropdown.gameObject.SetActive(isLevel && levelType != LevelType.Simulation);
                difficultyDropdown.value = (int)difficulty;
            }
            
            if (mechanicsText) mechanicsText.text = mechanics;
            if (scoringText) scoringText.text = scoring;
        }
        
        private void UpdateTabsAndPages()
        {
            if (levelType is LevelType.PreTest or LevelType.PostTest)
            {
                if (scoringPage.IsVisible) detailsPage.OpenPageInGroup();
                if (scoringToggle.IsOn) detailsToggle.SetValue(true);
                
                scoringToggle.gameObject.SetActive(false);
            }
            else
            {
                scoringToggle.gameObject.SetActive(true);
            }
        }
        
        
        // EVENT LISTENER METHODS
        private void GuideButton_onClick()
        {
            OnGuideClicked?.Invoke(campaignLevelData.Number);
        }
        
        private void TermsButton_onClick()
        {
            OnTermsClicked?.Invoke(campaignLevelData.Number);
        }
        
        private void DifficultyDropdown_onValueChanged(int value)
        {
            difficulty = (Difficulty)value;
            gameDataService.CampaignData.selectedDifficulty = (Difficulty)value;
        }
        
        private void PlayButton_onClick()
        {
            OnPlayLevelClicked?.Invoke(campaignLevelData, difficulty);
        }
    }
}
