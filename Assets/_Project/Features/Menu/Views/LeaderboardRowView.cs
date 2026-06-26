using ProjectATLAS.Avatar;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectATLAS
{
    public class LeaderboardRowView : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private bool isDisabled;
        [SerializeField] private int number;
        [SerializeField] private AvatarProfile avatarProfile;
        [SerializeField] private string displayName;
        [SerializeField] private string username;
        [SerializeField] private int score;
        [Space]
        [SerializeField] private bool campaignScoreMode;
        [SerializeField] private float progress;
        
        [Header("Components")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TMP_Text numberText;
        [SerializeField] private Image headAvatarImage;
        [SerializeField] private TMP_Text displayNameText;
        [SerializeField] private TMP_Text usernameText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text progressText;
        
        // MONOBEHAVIOUR METHODS
        private void OnValidate()
        {
            if (campaignScoreMode)
                DisplayCampaignScore(number, avatarProfile, displayName, username, score, progress);
            else
                DisplayBestLevelScore(number, avatarProfile, displayName, username, score);
            
            SetDisabled(isDisabled, campaignScoreMode);
        }
        
        // PUBLIC METHODS
        public void DisplayBestLevelScore(
            int number, AvatarProfile avatarProfile,
            string displayName, string username, int score)
        {
            this.number = number;
            this.avatarProfile = avatarProfile;
            this.displayName = displayName;
            this.username = username;
            this.score = score;
            
            this.campaignScoreMode = false;
            
            if (numberText) numberText.text = number.ToString();
            if (headAvatarImage)
            {
                headAvatarImage.gameObject.SetActive(true);
                headAvatarImage.sprite = avatarProfile.characterHead;
            }
            if (displayNameText) displayNameText.text = displayName;
            if (usernameText) usernameText.text = username;
            if (scoreText) scoreText.text = score.ToString();
            if (progressText)
            {
                progressText.gameObject.SetActive(false);
            }
        }
        
        public void DisplayCampaignScore(
            int number, AvatarProfile avatarProfile,
            string displayName, string username,
            int score, float progress)
        {
            this.number = number;
            this.avatarProfile = avatarProfile;
            this.displayName = displayName;
            this.username = username;
            this.score = score;
            this.progress = progress;
            
            this.campaignScoreMode = true;
            
            if (numberText) numberText.text = number.ToString();
            if (headAvatarImage)
            {
                headAvatarImage.gameObject.SetActive(true);
                headAvatarImage.sprite = avatarProfile.characterHead;
            }
            if (displayNameText) displayNameText.text = displayName;
            if (usernameText) usernameText.text = username;
            if (scoreText) scoreText.text = score.ToString();
            if (progressText)
            {
                progressText.gameObject.SetActive(true);
                progressText.text = $"{progress:F2}%";
            }
        }
        
        public void SetDisabled(bool isDisabled, bool campaignScoreMode)
        {
            this.isDisabled = isDisabled;
            this.campaignScoreMode = campaignScoreMode;
            
            canvasGroup.alpha = isDisabled ? 0.5f : 1f;
            
            if (isDisabled)
            {
                if (numberText) numberText.text = "-";
                if (headAvatarImage)
                {
                    headAvatarImage.sprite = null;
                    headAvatarImage.gameObject.SetActive(false);
                }
                if (displayNameText) displayNameText.text = "- - - - -";
                if (usernameText) usernameText.text = "- - - - -";
                if (scoreText) scoreText.text = "0";
                if (progressText)
                {
                    progressText.text = "0%";
                    progressText.gameObject.SetActive(campaignScoreMode);
                }
            }
        }
        
    }
}
