using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KBCore.Refs;

using ProjectATLAS.UI;
using ProjectATLAS.Gameplay;
using ProjectATLAS.GameData;
using ProjectATLAS.Cheats;

namespace ProjectATLAS.Menu
{
    using static ProjectATLAS.GameData.CampaignData;
    
    public class CampaignPage : UIPage
    {
        [Header("Pages")]
        [SerializeField] private LevelPage levelPage;
        [SerializeField] private TestDetailsPage testPage;
        [SerializeField] private LevelPanel levelPanel;
        [SerializeField] private Button[] closeButtons;
        
        [Header("References")]
        [SerializeField, Child] private LevelButton[] levelButtons;
        [SerializeField, Child] private TestButton[] testButtons;
        [SerializeField, Child] private ChapterInfo[] chapterInfos;
        [SerializeField] private TMP_Text levelsText;
        [SerializeField] private TMP_Text medalsText;
        [SerializeField] private TMP_Text campaignScoreText;
        [SerializeField] private ScrollFocus scrollFocus;
        [SerializeField] private ScrollRect levelsScrollRect;
        
        private Dictionary<LevelData, LevelButton> levelDataButtonMap;
        private Dictionary<TestData, TestButton> testDataButtonMap;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            levelDataButtonMap = new();
            testDataButtonMap = new();
            
            foreach (LevelButton levelButton in levelButtons)
            {
                levelDataButtonMap.Add(levelButton.LevelData, levelButton);
                levelButton.OnClicked += LevelButton_onClick;
            }
            
            foreach (TestButton testButton in testButtons)
            {
                testDataButtonMap.Add(testButton.TestData, testButton);
                testButton.OnClicked += TestButton_onClick;
            }
            
            foreach (var closeButton in closeButtons)
                closeButton.onClick.AddListener(CloseButton_onClick);
            
            ResetAllButtons();
        }
        
        
        // UIPage METHODS
        public override void OpenPage()
        {
            base.OpenPage();
            
            RecordsData recordsData = GameDataManager.Instance.RecordsData;
            LevelManager levelManager = LevelManager.Instance;
            
            bool unlockAll = CheatsManager.UnlockAllLevels;
            
            foreach (LevelButton levelButton in levelButtons)
            {
                LevelData levelData = levelButton.LevelData;
                
                bool isUnlocked = unlockAll || levelManager.IsLevelAvailable(levelData.Number);
                levelButton.SetEnable(isUnlocked);
                
                Level level = levelManager.GetLevelGameData(levelData);
                if (level != null)
                    levelButton.SetMedals(level.isBronze, level.isSilver, level.isGold);
            }
            
            foreach (TestButton testButton in testButtons)
            {
                if (testButton.TestData is PreTestData preTestData)
                {
                    bool isUnlocked = unlockAll || levelManager.IsPreTestAvailable(preTestData.Number);
                    testButton.SetEnable(isUnlocked);
                }
                else if (testButton.TestData is PostTestData postTestData)
                {
                    bool isUnlocked = unlockAll || levelManager.IsPostTestAvailable(postTestData.Number);
                    testButton.SetEnable(isUnlocked);
                }
            }
            
            foreach (ChapterInfo chapterInfo in chapterInfos)
            {
                int chapter = chapterInfo.Chapter;
                
                (int completedLevels, int totalLevels) = levelManager.GetCompletedAndTotalLevels(chapter);
                chapterInfo.SetLevels(completedLevels, totalLevels);
                
                (int obtainedMedals, int totalMedals) = levelManager.GetObtainedAndTotalMedals(chapter);
                chapterInfo.SetMedals(obtainedMedals, totalMedals);
            }
            
            (int completedL, int totalL) = levelManager.GetCompletedAndTotalLevels(0);
            (int obtainedM, int totalM) = levelManager.GetObtainedAndTotalMedals(0);
            
            levelsText.text = $"{completedL}/{totalL}";
            medalsText.text = $"{obtainedM}/{totalM}";
            campaignScoreText.text = recordsData.GetCampaignScore().ToString();
            
            foreach (var closeButton in closeButtons)
                closeButton.gameObject.SetActive(false);
        }

        public override void ClosePage()
        {
            base.ClosePage();
            
            levelPanel.ClosePanel(false);
        }
        
        
        // EVENT LISTENER METHODS
        private void LevelButton_onClick(LevelButton levelButton, LevelData levelData)
        {
            OpenLevel(levelButton.transform as RectTransform, levelData);
        }
        
        private void TestButton_onClick(TestButton testButton, TestData testData)
        {
            OpenLevel(testButton.transform as RectTransform, testData);
        }
        
        private void CloseButton_onClick()
        {
            CloseLevel();
        }
        
        
        // PRIVATE METHODS
        private void ResetAllButtons()
        {
            foreach (LevelButton levelButton in levelButtons)
            {
                levelButton.SetEnable(false);
                levelButton.SetMedals(false, false, false);
            }
            
            foreach (TestButton testButton in testButtons)
            {
                testButton.SetEnable(false);
            }
        }
        
        private void OpenLevel(RectTransform buttonToFocus, CampaignLevelData campaignLevelData)
        {
            if (levelPanel.IsAnimating) return;
            levelPanel.OpenPanel(campaignLevelData);
            
            scrollFocus.FocusContentOnTarget(buttonToFocus.transform as RectTransform);
            scrollFocus.SetExpandedScrollArea(true);
            
            foreach (var closeButton in closeButtons)
                closeButton.gameObject.SetActive(true);
        }
        
        private void CloseLevel()
        {
            levelPanel.ClosePanel();
            
            scrollFocus.UnfocusReference();
            scrollFocus.SetExpandedScrollArea(false);
            
            if (IsContentNotFillingViewport())
                scrollFocus.InterruptAnimation();
            
            foreach (var closeButton in closeButtons)
                closeButton.gameObject.SetActive(false);
        }
        
        public bool IsContentNotFillingViewport()
        {
            RectTransform contentTransform = levelsScrollRect.content;
            RectTransform viewportTransform = levelsScrollRect.viewport;
            
            float contentWidth = contentTransform.sizeDelta.x;
            float contentPosX = contentTransform.anchoredPosition.x;
            float viewportWidth = viewportTransform.rect.width;
            float offset = contentWidth + contentPosX - viewportWidth;
            
            Debug.Log($"contentWidth: {contentWidth}, contentPosX: {contentPosX}, viewportWidth: {viewportWidth}");
            Debug.Log($"offset: {offset}");
            
            return offset <= 0;
        }
    }
}
