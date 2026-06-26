using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using KBCore.Refs;

using ProjectATLAS.Authentication;
using ProjectATLAS.CloudSave;
using ProjectATLAS.UI;
using ProjectATLAS.GameData;

namespace ProjectATLAS.Menu
{
    public class LogoutDialog : UIDialog
    {
        [Header("Components")]
        [SerializeField] private Button logoutButton;
        
        [Header("References")]
        [SerializeField, Scene] private UIOverlay overlay;
        [SerializeField, Scene] private AccountPage accountPage;
        [SerializeField, Scene] private WelcomePage welcomePage;
        
        
        // MONOBEHAVIOUR METHODS
        protected override void Start()
        {
            base.Start();
            
            logoutButton.onClick.AddListener(OnLogout);
        }
        
        
        // EVENT LISTENER METHODS
        private async void OnLogout()
        {
            await Logout();
        }
        
        
        // PRIVATE METHODS
        private async Task Logout()
        {
            // Close dialog first to avoid overlay being hidden behind
            base.CloseDialog();
            
            // Check internet access and display overlay message
            overlay.DisplayLoading("Logging out...", "Checking internet access for logout.", UIOverlay.Icon.Loading, "");
            AuthResult authResult = await AuthenticationManager.Instance.CheckNetworkStatusAsync();
            
            if (authResult.Status != Authentication.NetworkResult.Online)
            {
                overlay.DisplayMessage("Logout failed", authResult.Message, UIOverlay.Icon.Failed, "Press anywhere to close");
                return;
            }
            
            
            // Display saving cloud data overlay message
            overlay.DisplayLoading("Logging out...", "Saving game progress to cloud...", UIOverlay.Icon.Loading, "");
            
            // Get CloudSaveManager instance
            CloudSaveManager cloudSaveManager = CloudSaveManager.Instance;
            if (cloudSaveManager == null)
            {
                overlay.DisplayMessage("Logout failed", "(Developer) CloudSaveManager instance is null, cannot save to cloud.",
                    UIOverlay.Icon.Failed, "Press anywhere to close.");
                return;
            }
            
            // Save data to cloud
            var saveResult = await cloudSaveManager.SaveCloudDataAsync();
            if (saveResult.Status == SaveResult.Success)
            {
                overlay.DisplayLoading("Logging out...", "Logging out from the account...", UIOverlay.Icon.Loading, "");
            }
            else
            {
                overlay.DisplayMessage("Logout failed", saveResult.Message, UIOverlay.Icon.Failed, "Press anywhere to close.");
                return;
            }
            
            
            // Call logout backend
            authResult = await AuthenticationManager.Instance.LogOut();
            if (authResult.Status == LogoutResult.Success)
            {
                overlay.DisplayMessage("Logout successful",
                    "Logged out successfully. Local game data is cleared.",
                    UIOverlay.Icon.Success, "Return to profile screen");
                
                overlay.DoAfterClosing(() =>
                {
                    welcomePage.OpenPageInGroup();
                    welcomePage.PageGroup.ClearPageStack();
                });
            }
            else
            {
                overlay.DisplayMessage("Logout failed",
                    authResult.Message,
                    UIOverlay.Icon.Failed, "Press anywhere to close");
            }
            
            GameDataManager.Instance.SaveData();
        }
        
    }
}
