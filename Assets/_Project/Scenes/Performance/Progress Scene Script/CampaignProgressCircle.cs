using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using ProjectATLAS.GameData;

namespace ProjectATLAS
{
    public class CampaignProgressCircle : MonoBehaviour
    {
        [Header("UI References")]
        public Image fillImage;          // Black radial fill image
        public TMP_Text percentText;     // Text inside small circle

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
            UpdateDisplay();
        }

        private void HandleGameDataLoaded(GameData.GameData _)
        {
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            var campaign = GameDataManager.Instance?.CampaignData;
            if (campaign == null)
            {
                Debug.LogWarning("Campaign data is null.");
                return;
            }

            // Compute total and completed progress
            int totalLevels = campaign.levels.Count;
            int completedLevels = campaign.levels.Count(l => l.isCompleted);
            float progress = totalLevels > 0 ? (float)completedLevels / totalLevels : 0f;

            // Update the circle fill
            if (fillImage)
                fillImage.fillAmount = progress;

            // Update the percentage text
            if (percentText)
                percentText.text = $"{Mathf.RoundToInt(progress * 100)}%";
        }
    }
}
