using UnityEngine;

using ProjectATLAS.UI;
using TMPro;
using ProjectATLAS.GameData;
using ProjectATLAS.Authentication;
using UnityEngine.UI;
using ProjectATLAS.Avatar;
using ProjectATLAS.Gameplay;

namespace ProjectATLAS.Menu
{
    public class MenuPage : UIPage
    {
        [Header("Components")]
        [SerializeField] private Image avatarImage;
        [SerializeField] private TMP_Text displayNameText;
        [SerializeField] private TMP_Text usernameText;
        [SerializeField] private Image progressFill;
        [SerializeField] private TMP_Text gameProgressText;
        
        private GameDataManager gameDataManager;
        private AuthenticationManager authenticationService;
        private LevelManager levelManager;
        private AvatarManager avatarManager;
        
        
        private void Awake()
        {
            gameDataManager = GameDataManager.Instance;
            authenticationService = AuthenticationManager.Instance;
            levelManager = LevelManager.Instance;
            avatarManager = AvatarManager.Instance;
            
            authenticationService.OnInitialized += AuthenticationService_OnInitialized;
        }
        
        private void AuthenticationService_OnInitialized()
        {
            RefreshUI();
        }
        
        
        public override void OpenPage()
        {
            base.OpenPage();
            
            RefreshUI();
        }
        
        private void RefreshUI()
        {
            if (gameDataManager == null)
                gameDataManager = GameDataManager.Instance;
            
            if (authenticationService == null)
                authenticationService = AuthenticationManager.Instance;
            
            if (levelManager == null)
                levelManager = LevelManager.Instance;
            
            if (avatarManager == null)
                avatarManager = AvatarManager.Instance;
            
            
            if (gameDataManager && authenticationService)
            {
                string displayName = gameDataManager.DetailsData.displayName;
                
                bool isLoggedIn = authenticationService.IsInitialized && authenticationService.IsLoggedIn || authenticationService.IsLoginCached;
                string usernameName = isLoggedIn ? authenticationService.CachedUsername : "Local";
                
                float percentage = levelManager.GetCampaignProgress();
                string gameProgress = $"{percentage:F2}%";
                
                AvatarProfile avatarProfile = avatarManager.GetCurrentAvatarProfile();
                
                avatarImage.sprite = avatarProfile.characterHead;
                displayNameText.text = displayName;
                usernameText.text = usernameName;
                progressFill.fillAmount = percentage / 100f;
                gameProgressText.text = gameProgress;
                
                gameDataManager.SaveData();
            }
        }
    }
}
