using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using ProjectATLAS.Gameplay;
using ProjectATLAS.UI;
using ProjectATLAS.GameData;

namespace ProjectATLAS.Menu
{
    public class LevelPage : UIPage
    {
        [SerializeField] private LevelData levelData;
        [SerializeField] private Details details;
        [SerializeField] private Components components;
        
        private GameDataManager gameDataService;
        private LevelManager levelManager;
        
        // PROPERTIES
        public event Action<LevelData> OnRecordsClicked;
        public event Action<int> OnLeaderboardsClicked;
        public event Action<int> OnGuideClicked;
        public event Action<int> OnTermsClicked;
        public event Action<LevelData, Difficulty> OnPlayLevelClicked;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            components.recordsButton.onClick.AddListener(RecordsButton_onClick);
            components.leaderboardsButton.onClick.AddListener(LeaderboardsButton_onClick);
            components.guideButton.onClick.AddListener(GuideButton_onClick);
            components.termsButton.onClick.AddListener(TermsButton_onClick);
            components.difficultyDropdown.onValueChanged.AddListener(DifficultyDropdown_onValueChanged);
            components.playButton.onClick.AddListener(PlayButton_onClick);
            
            levelManager = LevelManager.Instance;
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();
            
            if (!gameObject.activeInHierarchy) return;
            
            if (levelData)
            {
                UpdateLevelDetails(levelData);
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
        public void UpdateLevelDetails(LevelData levelData)
        {
            this.levelData = levelData;
            
            string prefix = levelData.Type switch
            {
                LevelType.PreTest => $"PRE-TEST",
                LevelType.PostTest => $"POST-TEST",
                _ => $"LEVEL",
            };
            details.title = $"{prefix} {levelData.Number} - {levelData.Title}";
            
            details.type = levelData.Type;
            details.info = levelData.Info;
            
            details.description = levelData.Description;
            details.objectives = levelData.Objectives;
            
            details.bronzeObjective = levelData.BronzeObjective;
            details.silverObjective = levelData.SilverObjective;
            details.goldObjective = levelData.GoldObjective;
            
            
            if (levelManager == null)
                levelManager = LevelManager.Instance;
            
            if (levelManager)
            {
                CampaignData.Level levelGameData = levelManager.GetLevelGameData(levelData);
                if (levelGameData != null)
                {
                    details.isBronze = levelGameData.isBronze;
                    details.isSilver = levelGameData.isSilver;
                    details.isGold = levelGameData.isGold;
                }
                else
                {
                    details.isBronze = false;
                    details.isSilver = false;
                    details.isGold = false;
                }
                
                details.displayName = levelManager.DisplayName;
                
                RecordsData.LevelRecord levelRecord = gameDataService.RecordsData.GetBestLevelPoints(levelData.ID);
                details.bestScore = levelRecord == null ? "0" : levelRecord.levelPoints.ToString();
                details.difficulty = levelManager.SelectedDifficulty;
            }
            else if (Application.isPlaying)
            {
                details.displayName = "Player";
                details.isBronze = false;
                details.isSilver = false;
                details.isGold = false;
                details.bestScore = "0";
            }
            
            components.UpdateComponents(details);
        }
        
        
        
        
        // EVENT LISTENER METHODS
        private void RecordsButton_onClick()
        {
            OnRecordsClicked?.Invoke(levelData);
        }
        
        private void LeaderboardsButton_onClick()
        {
            OnLeaderboardsClicked?.Invoke(levelData.Number);
        }
        
        private void GuideButton_onClick()
        {
            OnGuideClicked?.Invoke(levelData.Number);
        }
        
        private void TermsButton_onClick()
        {
            OnTermsClicked?.Invoke(levelData.Number);
        }
        
        private void DifficultyDropdown_onValueChanged(int value)
        {
            details.difficulty = (Difficulty)value;
            
            if (levelManager) levelManager.SelectedDifficulty = (Difficulty)value;
        }
        
        private void PlayButton_onClick()
        {
            OnPlayLevelClicked?.Invoke(levelData, details.difficulty);
        }
        
        
        // STRUCTS
        [Serializable] public class Components
        {
            public Header header;
            public LevelHeader levelHeader;
            public TMP_Text typeText;
            public TMP_Text infoText;
            
            public TMP_Text descriptionText;
            public TMP_Text objectivesText;
            
            public TMP_Text bronzeText;
            public TMP_Text silverText;
            public TMP_Text goldText;
            
            public Image bronzeImage;
            public Image silverImage;
            public Image goldImage;
            
            public TMP_Text bestScoreText;
            
            public Button recordsButton;
            public Button leaderboardsButton;
            public Button guideButton;
            public Button termsButton;
            public TMP_Dropdown difficultyDropdown;
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
                
                if (bronzeText) bronzeText.text = details.bronzeObjective;
                if (silverText) silverText.text = details.silverObjective;
                if (goldText) goldText.text = details.goldObjective;
                
                if (bronzeImage) bronzeImage.gameObject.SetActive(details.isBronze);
                if (silverImage) silverImage.gameObject.SetActive(details.isSilver);
                if (goldImage) goldImage.gameObject.SetActive(details.isGold);
                
                if (bestScoreText) bestScoreText.text = details.bestScore;
                
                if (leaderboardsButton) leaderboardsButton.interactable =
                    details.type != LevelType.PreTest && details.type != LevelType.PostTest;
                if (guideButton) guideButton.interactable = details.type == LevelType.Lesson;
                if (termsButton) termsButton.interactable = details.type == LevelType.Lesson;
                if (difficultyDropdown)
                {
                    difficultyDropdown.value = (int)details.difficulty;
                    difficultyDropdown.interactable =
                        details.type != LevelType.PreTest && details.type != LevelType.PostTest;
                }
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
            public string bronzeObjective;
            public string silverObjective;
            public string goldObjective;
            public bool isBronze;
            public bool isSilver;
            public bool isGold;
            public string bestScore;
            public Difficulty difficulty;
        }
    }
}
