using TMPro;
using UnityEngine;
using UnityEngine.UI;

using ProjectATLAS.Authentication;
using ProjectATLAS.UI;
using ProjectATLAS.GameData;
using ProjectATLAS.Gameplay;
using ProjectATLAS.Avatar;

namespace ProjectATLAS.Menu
{
    public class ProfilePage : UIPage
    {
        [Header("Settings")]
        [SerializeField] private Color loggedInColor = Color.green;
        [SerializeField] private Color loggedOutColor = Color.red;
        
        [Header("Components")]
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text displayName;
        [SerializeField] private TMP_Text details;
        [Space]
        [SerializeField] private TMP_Text gameProgress;
        [SerializeField] private TMP_Text campaignPoints;
        [SerializeField] private Image avatarImage;
        [Space]
        [SerializeField] private Image statusImage;
        [SerializeField] private TMP_Text accountStatus;
        [SerializeField] private TMP_Text username;
        [SerializeField] private Button loginButton;
        [SerializeField] private Button registerButton;
        [SerializeField] private Button accountButton;
        
        
        // MONOBEHAVIOUR METHODS
        protected override void Start()
        {
            base.Start();
        }
        
        private void OnEnable()
        {
            AuthenticationManager.Instance.OnInitialized += RefreshProfilePreview;
        }
        
        private void OnDisable()
        {
            AuthenticationManager.Instance.OnInitialized -= RefreshProfilePreview;
        }
        
        
        // PUBLIC METHODS
        public override void OpenPage()
        {
            base.OpenPage();
            
            RefreshProfilePreview();
        }
        
        
        // PRIVATE METHODS
        private void RefreshProfilePreview()
        {
            Debug.Log($"ProfilePage.RefreshProfilePreview()");
            
            AuthenticationManager authManager = AuthenticationManager.Instance;
            GameDataManager gameDataManager = GameDataManager.Instance;
            
            if (gameDataManager)
            {
                // Get all relevant data
                AvatarProfile avatarProfile = AvatarManager.Instance.GetCurrentAvatarProfile();
                
                int age = gameDataManager.DetailsData.age;
                string gender = gameDataManager.DetailsData.gender;
                
                float progressValue = LevelManager.Instance.GetCampaignProgress();
                float campaignPointsValue = gameDataManager.RecordsData.GetCampaignScore();
                
                
                // Display data to UI elements
                iconImage.sprite = avatarProfile.characterHead;
                displayName.text = gameDataManager.DetailsData.displayName;
                details.text = $"{age}, {gender}";
                
                gameProgress.text = $"{progressValue:F2}%";
                campaignPoints.text = $"{campaignPointsValue:N0}";
                
                avatarImage.sprite = avatarProfile.characterSprite;
            }
            
            
            if (authManager.IsInitialized && authManager.IsLoginCached && (authManager.IsLoggedIn || authManager.IsPreviouslyLoggedIn))
            {
                if (statusImage)    statusImage.color = loggedInColor;
                if (accountStatus)  accountStatus.text = "Logged In";
                if (username)       username.text = authManager.CachedUsername;
                if (accountButton)  accountButton.gameObject.SetActive(true);
                if (loginButton)    loginButton.gameObject.SetActive(false);
                if (registerButton) registerButton.gameObject.SetActive(false);
            }
            else
            {
                if (statusImage)    statusImage.color = loggedOutColor;
                if (accountStatus)  accountStatus.text = "Not Logged In";
                if (username)       username.text = "-----";
                if (accountButton)  accountButton.gameObject.SetActive(false);
                if (loginButton)    loginButton.gameObject.SetActive(true);
                if (registerButton) registerButton.gameObject.SetActive(true);
            }
        }
    }
}
