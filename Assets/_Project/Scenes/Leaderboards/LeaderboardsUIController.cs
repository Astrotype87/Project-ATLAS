using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Leaderboards.Models;

namespace ProjectATLAS.Leaderboards
{
    public class LeaderboardsUIController : MonoBehaviour
    {
        [Header("References")]
        public LeaderboardsManager leaderboardsManager;

        [Header("UI Elements (5 entries)")]
        public Image[] avatarImages;
        public TextMeshProUGUI[] usernameTexts;
        public TextMeshProUGUI[] displayNameTexts;
        public TextMeshProUGUI[] progressTexts;
        public TextMeshProUGUI[] pointsTexts;

        [Header("Navigation")]
        public Button leftButton;
        public Button rightButton;
        public TextMeshProUGUI levelText;

        [Header("Settings")]
        public int currentLevel = 1;
        public int maxLevel = 10;
        public Sprite defaultAvatar;

        private async void Start()
        {
            // Try to auto-find manager if not set manually
            if (leaderboardsManager == null)
            {
                leaderboardsManager = FindObjectOfType<LeaderboardsManager>();
                if (leaderboardsManager == null)
                {
                    Debug.LogError("LeaderboardsManager reference not found in scene!");
                    return;
                }
            }

            // Assign button listeners
            if (leftButton != null)
                leftButton.onClick.AddListener(OnPreviousLevel);
            if (rightButton != null)
                rightButton.onClick.AddListener(OnNextLevel);

            await LoadLevelLeaderboardAsync(currentLevel);
        }

        private async void OnPreviousLevel()
        {
            if (currentLevel > 1)
            {
                currentLevel--;
                await LoadLevelLeaderboardAsync(currentLevel);
            }
        }

        private async void OnNextLevel()
        {
            if (currentLevel < maxLevel)
            {
                currentLevel++;
                await LoadLevelLeaderboardAsync(currentLevel);
            }
        }

        private async Task LoadLevelLeaderboardAsync(int level)
        {
            if (levelText != null)
                levelText.text = $"{level}";

            ClearLeaderboard();

            if (leaderboardsManager == null)
            {
                Debug.LogError("LeaderboardsManager reference missing!");
                return;
            }

            // Ensure Unity Services is initialized
            try
            {
                if (!Unity.Services.Core.UnityServices.State.Equals(Unity.Services.Core.ServicesInitializationState.Initialized))
                {
                    await Unity.Services.Core.UnityServices.InitializeAsync();
                    Debug.Log("Unity Services initialized for leaderboard access.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize Unity Services: {ex.Message}");
                return;
            }

            // Try to check auth, but don't stop if not signed in
            bool signedIn = false;
            try
            {
                signedIn = Unity.Services.Authentication.AuthenticationService.Instance.IsSignedIn;
            }
            catch
            {
                Debug.LogWarning("Authentication service not initialized � continuing as guest.");
            }

            if (!signedIn)
                Debug.Log("Loading public leaderboard data (guest mode)...");

            // Load leaderboard data
            LeaderboardScoresPage scoresPage = null;

            try
            {
                scoresPage = await leaderboardsManager.GetBestLevelScores(level, 0, 5);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error fetching leaderboard data: {ex.Message}");
                return;
            }

            if (scoresPage == null || scoresPage.Results == null || scoresPage.Results.Count == 0)
            {
                Debug.Log($"No leaderboard data for Level {level}");
                return;
            }

            int count = Mathf.Min(scoresPage.Results.Count, 5);

            for (int i = 0; i < count; i++)
            {
                var score = scoresPage.Results[i];
                string[] metadata = score.Metadata?.Split(',') ?? new string[0];

                string username = metadata.Length > 0 ? metadata[0] : "Unknown";
                string displayName = metadata.Length > 1 ? metadata[1] : "Player";
                string progress = metadata.Length > 3 ? metadata[3] : "0";

                if (i < avatarImages.Length && avatarImages[i] != null)
                    avatarImages[i].sprite = defaultAvatar;

                if (i < usernameTexts.Length && usernameTexts[i] != null)
                    usernameTexts[i].text = username; // show only username text (no title)

                if (i < displayNameTexts.Length && displayNameTexts[i] != null)
                    displayNameTexts[i].text = displayName; // show only display name (no title)

                if (i < progressTexts.Length && progressTexts[i] != null)
                    progressTexts[i].text = progress; // just number, no %

                if (i < pointsTexts.Length && pointsTexts[i] != null)
                    pointsTexts[i].text = score.Score.ToString("N0"); // just points
            }
        }

        private void ClearLeaderboard()
        {
            int entryCount = Mathf.Min(
                avatarImages.Length,
                usernameTexts.Length,
                displayNameTexts.Length,
                progressTexts.Length,
                pointsTexts.Length
            );

            for (int i = 0; i < entryCount; i++)
            {
                if (avatarImages[i] != null)
                    avatarImages[i].sprite = defaultAvatar;

                if (usernameTexts[i] != null)
                    usernameTexts[i].text = "-";

                if (displayNameTexts[i] != null)
                    displayNameTexts[i].text = "-";

                if (progressTexts[i] != null)
                    progressTexts[i].text = "-";

                if (pointsTexts[i] != null)
                    pointsTexts[i].text = "-";
            }
        }
    }
}
