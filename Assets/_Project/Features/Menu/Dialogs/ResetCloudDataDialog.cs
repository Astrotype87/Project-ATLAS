using UnityEngine;
using UnityEngine.UI;
using KBCore.Refs;

using ProjectATLAS.UI;
using ProjectATLAS.GameData;
using ProjectATLAS.CloudSave;

namespace ProjectATLAS.Menu
{
    public class ResetCloudDataDialog : UIDialog
    {
        [Header("Components")]
        [SerializeField] private Button resetCloudDataButton;
        
        [Header("References")]
        [SerializeField, Scene] private UIOverlay overlay;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            resetCloudDataButton.onClick.AddListener(OnSaveDataToCloud);
        }
        
        protected override void Start()
        {
            base.Start();
        }
        
        
        // PRIVATE METHODS
        private async void OnSaveDataToCloud()
        {
            // Close dialog first to avoid overlay being hidden behind
            base.CloseDialog();
            
            // Reset game data except details data
            GameDataManager.Instance.ResetDataExceptDetails();
            
            
            // Display save cloud data overlay
            overlay.OpenOverlay();
            overlay.DisplayLoading("Resetting cloud data...", "Please wait a moment.", UIOverlay.Icon.Loading, "");
            
            // Get CloudSaveManager instance
            CloudSaveManager cloudSaveManager = CloudSaveManager.Instance;
            if (cloudSaveManager == null)
            {
                overlay.DisplayMessage("Reset cloud data failed", "(Developer) CloudSaveManager instance is null.",
                    UIOverlay.Icon.Failed, "Press anywhere to close.");
                return;
            }
            
            // Save data to cloud
            var saveResult = await cloudSaveManager.SaveCloudDataAsync();
            if (saveResult.Status == SaveResult.Success)
            {
                overlay.DisplayMessage("Reset cloud data success", "Your local and cloud game data has been reset.",
                    UIOverlay.Icon.Success, "Press anywhere to close.");
            }
            else
            {
                overlay.DisplayMessage("Reset cloud data failed", saveResult.Message, UIOverlay.Icon.Failed, "Press anywhere to close.");
            }
        }
    }
}
