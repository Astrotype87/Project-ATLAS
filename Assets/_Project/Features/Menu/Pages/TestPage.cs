using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using ProjectATLAS.Gameplay;
using ProjectATLAS.UI;
using ProjectATLAS.GameData;

namespace ProjectATLAS.Menu
{
    public class TestDetailsPage : UIPage
    {
        [SerializeField] private TestData testData;
        [SerializeField] private Details details;
        [SerializeField] private Components components;
        
        private GameDataManager gameDataService;
        
        // PROPERTIES
        public event Action<TestData> OnRecordsClicked;
        public event Action<TestData> OnPlayTestClicked;
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            components.recordsButton.onClick.AddListener(RecordsButton_onClick);
            components.playButton.onClick.AddListener(PlayButton_onClick);
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();
            
            if (!gameObject.activeInHierarchy) return;
            
            if (testData)
            {
                UpdateTestDetails(testData);
            }
            else
            {
                components.UpdateComponents(details);
            }
        }
        
        // UIPage METHODS
        public override void OpenPage()
        {
            base.OpenPage();
            components.levelHeader.Show();
        }
        
        public override void ClosePage()
        {
            components.levelHeader.Hide();
            base.ClosePage();
        }
        
        
        
        // PUBLIC METHODS
        public void UpdateTestDetails(TestData testData)
        {
            this.testData = testData;
            
            string prefix = testData.Type switch
            {
                LevelType.PreTest => $"PRE-TEST",
                LevelType.PostTest => $"POST-TEST",
                _ => $"LEVEL",
            };
            details.title = $"{prefix} {testData.Number} - {testData.Title}";
            
            details.type = testData.Type;
            details.info = testData.Info;
            
            details.description = testData.Description;
            details.objectives = testData.Objectives;
            
            
            if (gameDataService == null)
                gameDataService = GameDataManager.Instance;
            
            if (gameDataService)
            {
                details.displayName = gameDataService.DetailsData.displayName;
                
                int maxScore = testData is PostTestData ? 8 : 4;
                RecordsData.TestRecord testRecord = gameDataService.RecordsData.GetBestTestScore(testData.ID);
                details.bestScore = testRecord == null ? "0" : $"{testRecord.score}/{testRecord.maxScore}";
            }
            else if (Application.isPlaying)
            {
                details.displayName = "Player";
                details.bestScore = "0/4";
            }
            
            components.UpdateComponents(details);
        }
        
        
        // EVENT LISTENER METHODS
        private void RecordsButton_onClick()
        {
            OnRecordsClicked?.Invoke(testData);
        }
        
        private void PlayButton_onClick()
        {
            OnPlayTestClicked?.Invoke(testData);
        }
        
        
        // STRUCTS
        [Serializable] public class Components
        {
            public Header header;
            public LevelHeader levelHeader;
            
            public TMP_Text descriptionText;
            public TMP_Text objectivesText;
            
            public TMP_Text bestScoreText;
            
            public Button recordsButton;
            public Button playButton;
            
            public void UpdateComponents(Details details)
            {
                if (header)
                {
                    header.SetText(details.title, details.allCaps);
                    header.SetBarMode(Header.BarMode.Full);
                    header.SetVisible(true);
                }
                if (levelHeader)
                {
                    levelHeader.SetLevelType(details.type);
                    levelHeader.SetInfo(details.info);
                }
                
                if (descriptionText) descriptionText.text = details.description.Replace("{player}", details.displayName);
                if (objectivesText) objectivesText.text = details.objectives;
                
                if (bestScoreText) bestScoreText.text = details.bestScore;
            }
        }
        
        [Serializable] public class Details
        {
            public string displayName;
            public string title;
            public bool allCaps;
            public LevelType type;
            public string info;
            [TextArea(3, 5)] public string description;
            [TextArea(3, 5)] public string objectives;
            public string bestScore;
        }
    }
}
