using UnityEngine;
using TMPro;
using System.Linq;
using System.Collections.Generic;
using ProjectATLAS.GameData;

namespace ProjectATLAS
{
    public class ProgressRead : MonoBehaviour
    {
        [Header("Player Info")]
        public TMP_Text usernameText;

        [Header("Medals UI References")]
        public TMP_Text totalMedalsText;
        public TMP_Text goldText;
        public TMP_Text silverText;
        public TMP_Text bronzeText;

        [Header("Chapter UI References")]
        public TMP_Text chaptersText;
        public TMP_Text preTestsText;
        public TMP_Text postTestsText;

        [Header("Level UI References")]
        public TMP_Text totalLevelsText;
        public TMP_Text lessonsText;
        public TMP_Text simulationsText;
        public TMP_Text challengesText;

        [Header("Extra Sections")]
        public TMP_Text guidebooksText;
        public TMP_Text glossaryText;
        public TMP_Text achievementsText;

        private void OnEnable()
        {
            if (GameDataManager.Instance != null)
                GameDataManager.Instance.OnGameDataLoaded += HandleGameDataLoaded;
        }

        private void OnDisable()
        {
            if (GameDataManager.Instance != null)
                GameDataManager.Instance.OnGameDataLoaded -= HandleGameDataLoaded;
        }

        private void Start()
        {
            TryUpdateDisplay();
        }

        private void HandleGameDataLoaded(ProjectATLAS.GameData.GameData _)
        {
            TryUpdateDisplay();
        }

        private void TryUpdateDisplay()
        {
            var campaign = GameDataManager.Instance?.CampaignData;

            // -----------------------
            // Username
            // -----------------------
            // if (usernameText != null && account != null)
            //     usernameText.text = account.username;

            if (campaign == null)
                return; 

            // -----------------------
            // Medal Section
            // -----------------------
            List<CampaignData.Level> levels = campaign.levels ?? new List<CampaignData.Level>();

            int gold = levels.Count(l => l.isGold);
            int silver = levels.Count(l => l.isSilver);
            int bronze = levels.Count(l => l.isBronze);
            int totalEarned = gold + silver + bronze;

            int maxPerType = 53;
            int maxTotal = 159;

            if (goldText) goldText.text = $"{gold}/{maxPerType}";
            if (silverText) silverText.text = $"{silver}/{maxPerType}";
            if (bronzeText) bronzeText.text = $"{bronze}/{maxPerType}";
            if (totalMedalsText) totalMedalsText.text = $"{totalEarned}/{maxTotal}";

            // -----------------------
            // Chapter & Test Section
            // -----------------------
            int completedPreTests = campaign.preTests?.Count(t => t.isCompleted) ?? 0;
            int completedPostTests = campaign.postTests?.Count(t => t.isCompleted) ?? 0;

            int totalPreTests = 10;
            int totalPostTests = 10;

            int completedChapters = completedPreTests + completedPostTests;
            int totalChapters = totalPreTests + totalPostTests;

            if (chaptersText) chaptersText.text = $"{completedChapters}/{totalChapters}";
            if (preTestsText) preTestsText.text = $"{completedPreTests}/{totalPreTests}";
            if (postTestsText) postTestsText.text = $"{completedPostTests}/{totalPostTests}";

            // -----------------------
            // Level Section
            // -----------------------
            int completedLessons = levels.Count(l => l.isCompleted && l.levelID != null && l.levelID.Contains("LESSON"));
            int completedSimulations = levels.Count(l => l.isCompleted && l.levelID != null && l.levelID.Contains("SIM"));
            int completedChallenges = levels.Count(l => l.isCompleted && l.levelID != null && l.levelID.Contains("CHALLENGE"));

            int totalLessons = 21;
            int totalSimulations = 10;
            int totalChallenges = 10;
            int totalLevels = totalLessons + totalSimulations + totalChallenges;

            if (lessonsText) lessonsText.text = $"{completedLessons}/{totalLessons}";
            if (simulationsText) simulationsText.text = $"{completedSimulations}/{totalSimulations}";
            if (challengesText) challengesText.text = $"{completedChallenges}/{totalChallenges}";
            if (totalLevelsText) totalLevelsText.text = $"{completedLessons + completedSimulations + completedChallenges}/{totalLevels}";

            // -----------------------
            // Extra Sections
            // -----------------------
            //I dont know whats the ID for this 3, tried manuuall
            int completedGuidebooks = levels.Count(l => l.isCompleted);
            int totalGuidebooks = 40;

            int completedGlossary = 0;
            int totalGlossary = 92;

            int completedAchievements = 0;
            int totalAchievements = 90;

            if (guidebooksText) guidebooksText.text = $"{completedGuidebooks}/{totalGuidebooks}";
            if (glossaryText) glossaryText.text = $"{completedGlossary}/{totalGlossary}";
            if (achievementsText) achievementsText.text = $"{completedAchievements}/{totalAchievements}";
        }
    }
}
