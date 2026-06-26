using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KBCore.Refs;

using ProjectATLAS.UI;
using ProjectATLAS.Leaderboards;
using ProjectATLAS.Avatar;

namespace ProjectATLAS.Menu
{
    public class LeaderboardsPage : UIPage
    {
        [Header("Data")]
        [SerializeField] private int selectedLevel;
        [SerializeField] private int maxLevel;
        
        [Header("Components")]
        [SerializeField, Child] private List<LeaderboardRowView> leaderboardRowViews;
        [SerializeField] private GameObject loadingOverlay;
        [SerializeField] private TMP_Text loadingText;
        [Space]
        [SerializeField] private TMP_Text headerText;
        [SerializeField] private UIToggleButton campaignScoreButton;
        [SerializeField] private UIToggleButton bestLevelScoreButton;
        [Space]
        [SerializeField] private CanvasGroup levelFilterGroup;
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        [SerializeField] private TMP_Text levelText;
        
        private LeaderboardsManager leaderboardsManager;
        private AvatarManager avatarManager;
        
        private bool isLoadingResults;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            leaderboardsManager = LeaderboardsManager.Instance;
            avatarManager = AvatarManager.Instance;
            
            campaignScoreButton.OnValueChanged += CampaignScoreButton_OnValueChanged;
            bestLevelScoreButton.OnValueChanged += BestLevelScoreButton_OnValueChanged;
            leftButton.onClick.AddListener(LeftButton_onClick);
            rightButton.onClick.AddListener(RightButton_onClick);
        }
        
        
        // UIPage METHODS
        public override void OpenPage()
        {
            base.OpenPage();
            
            if (!leaderboardsManager) leaderboardsManager = LeaderboardsManager.Instance;
            if (leaderboardsManager)
            {
                leaderboardsManager.TryUpdateQueuedLeaderboards();
            }
            
            
            if (campaignScoreButton.isOn)
            {
                if (headerText) headerText.text = "Campaign Score";
                if (levelFilterGroup) levelFilterGroup.alpha = 0.5f;
                
                leftButton.interactable = false;
                rightButton.interactable = false;
                
                StartCoroutine(LoadCampaignScore());
            }
            else if (bestLevelScoreButton.isOn)
            {
                if (headerText) headerText.text = $"Level {selectedLevel} Best Score";
                if (levelFilterGroup) levelFilterGroup.alpha = 1f;
                
                leftButton.interactable = true;
                rightButton.interactable = true;
                
                StartCoroutine(LoadBestLevelScore(selectedLevel));
            }
        }
        
        
        // PRIVATE METHODS
        public IEnumerator LoadCampaignScore()
        {
            // add click blocker gameobject
            // add bool isFetching status for enabling and disabling clicks during leaderboard loading
            // - on open page and on any other clicks
            // - basically on any LoadCampaignScore() and LoadLevelScore() calls
            // - show loading animation while scores are loading
            
            // If leaderboards is currently loading results, ignore this method call
            if (isLoadingResults) yield break;
            
            // Get reference to leaderboardsManager and avatarManager if null
            if (!leaderboardsManager) leaderboardsManager = LeaderboardsManager.Instance;
            if (!avatarManager) avatarManager = AvatarManager.Instance;
            
            if (leaderboardsManager)
            {
                // OPEN LOADING OVERLAY
                loadingOverlay.SetActive(true);
                isLoadingResults = true;
                
                // LOAD LEADERBOARDS
                int page = 1;
                int range = 10;
                var getCampaignScoresTask = leaderboardsManager.GetCampaignScores(page, range);
                
                yield return new WaitWhile(() => leaderboardsManager.IsFetching);
                yield return getCampaignScoresTask;
                
                // CLOSE LOADING OVERLAY
                isLoadingResults = false;
                loadingOverlay.SetActive(false);
                
                if (getCampaignScoresTask.Result == null) yield break;
                
                
                // GET RESULTS AND DISPLAY TO EACH LEADERBOARD ROW
                if (levelText) levelText.text = $"{selectedLevel}";
                
                foreach (var leaderboardRowView in leaderboardRowViews)
                    leaderboardRowView.SetDisabled(true, true);
                
                var resultPage = getCampaignScoresTask.Result;
                for (int i = 0; i < resultPage.Results.Count; i++)
                {
                    var result = resultPage.Results[i];
                    var metadata = JsonUtility.FromJson<CampaignScoreMetadata>(result.Metadata);
                    
                    int number = i + 1;
                    string username = metadata.username;
                    string displayName = metadata.displayName;
                    int avatarID = metadata.avatarID;
                    AvatarProfile avatarProfile = avatarManager.GetAvatarProfileByID(avatarID);
                    float gameProgress = metadata.gameProgress;
                    int score = (int)result.Score;
                    
                    leaderboardRowViews[i].DisplayCampaignScore(
                        number, avatarProfile, displayName, username, score, gameProgress);
                    leaderboardRowViews[i].SetDisabled(false, true);
                }
            }
        }
        
        public IEnumerator LoadBestLevelScore(int level)
        {
            // If leaderboards is currently loading results, ignore this method call
            if (isLoadingResults) yield break;
            
            // Get reference to leaderboardsManager and avatarManager if null
            if (!leaderboardsManager) leaderboardsManager = LeaderboardsManager.Instance;
            if (!avatarManager) avatarManager = AvatarManager.Instance;
            
            if (leaderboardsManager)
            {
                // OPEN LOADING OVERLAY
                loadingOverlay.SetActive(true);
                isLoadingResults = true;
                
                // LOAD LEADERBOARDS
                int page = 1;
                int range = 10;
                var getLevelScoresTask = leaderboardsManager.GetBestLevelScores(level, page, range);
                
                yield return new WaitWhile(() => leaderboardsManager.IsFetching);
                yield return getLevelScoresTask;
                
                // CLOSE LOADING OVERLAY
                isLoadingResults = false;
                loadingOverlay.SetActive(false);
                
                
                // GET RESULTS AND DISPLAY TO EACH LEADERBOARD ROW
                if (levelText) levelText.text = $"{selectedLevel}";
                
                foreach (var leaderboardRowView in leaderboardRowViews)
                    leaderboardRowView.SetDisabled(true, false);
                
                var resultPage = getLevelScoresTask.Result;
                for (int i = 0; i < resultPage.Results.Count; i++)
                {
                    var result = resultPage.Results[i];
                    var metadata = JsonUtility.FromJson<CampaignScoreMetadata>(result.Metadata);
                    
                    int number = i + 1;
                    string username = metadata.username;
                    string displayName = metadata.displayName;
                    int avatarID = metadata.avatarID;
                    AvatarProfile avatarProfile = avatarManager.GetAvatarProfileByID(avatarID);
                    int score = (int)result.Score;
                    
                    leaderboardRowViews[i].DisplayBestLevelScore(
                        number, avatarProfile, displayName, username, score);
                    leaderboardRowViews[i].SetDisabled(false, false);
                }
            }
        }
        
        
        // EVENT LISTENER METHODS
        private void CampaignScoreButton_OnValueChanged(UIToggleButton toggleButton, bool isOn)
        {
            if (isOn)
            {
                if (headerText) headerText.text = "Campaign Score";
                if (levelFilterGroup) levelFilterGroup.alpha = 0.5f;
                
                leftButton.interactable = false;
                rightButton.interactable = false;
                
                StartCoroutine(LoadCampaignScore());
            }
        }
        
        private void BestLevelScoreButton_OnValueChanged(UIToggleButton toggleButton, bool isOn)
        {
            if (isOn)
            {
                if (headerText) headerText.text = $"Level {selectedLevel} Best Score";
                if (levelFilterGroup) levelFilterGroup.alpha = 1f;
                
                leftButton.interactable = true;
                rightButton.interactable = true;
                
                StartCoroutine(LoadBestLevelScore(selectedLevel));
            }
        }
        
        private void LeftButton_onClick()
        {
            selectedLevel = Mathf.Clamp(selectedLevel - 1, 1, maxLevel);
            if (levelText) levelText.text = $"{selectedLevel}";
            if (headerText) headerText.text = $"Level {selectedLevel} Best Score";
            
            if (leftButton) leftButton.interactable = selectedLevel != 1;
            if (rightButton) rightButton.interactable = selectedLevel <= maxLevel;
            
            StartCoroutine(LoadBestLevelScore(selectedLevel));
        }
        
        private void RightButton_onClick()
        {
            selectedLevel = Mathf.Clamp(selectedLevel + 1, 1, maxLevel);
            if (levelText) levelText.text = $"{selectedLevel}";
            if (headerText) headerText.text = $"Level {selectedLevel} Best Score";
            
            if (leftButton) leftButton.interactable = selectedLevel != 1;
            if (rightButton) rightButton.interactable = selectedLevel <= maxLevel;
            
            StartCoroutine(LoadBestLevelScore(selectedLevel));
        }
    }
}
