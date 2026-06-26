using UnityEngine;
using UnityEngine.UI;
using KBCore.Refs;

using ProjectATLAS.UI;
using ProjectATLAS.CloudSave;

namespace ProjectATLAS.Menu
{
    public class SaveDataDialog : UIDialog
    {
        [Header("Components")]
        [SerializeField] private Button saveDataButton;
        
        [Header("References")]
        [SerializeField, Scene] private UIOverlay overlay;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            saveDataButton.onClick.AddListener(OnSaveDataToCloud);
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
            
            // Display save cloud data overlay
            overlay.OpenOverlay();
            overlay.DisplayLoading("Saving cloud data...", "Please wait a moment.", UIOverlay.Icon.Loading, "");
            
            // Get CloudSaveManager instance
            CloudSaveManager cloudSaveManager = CloudSaveManager.Instance;
            if (cloudSaveManager == null)
            {
                overlay.DisplayMessage("Cloud save failed", "(Developer) CloudSaveManager instance is null.",
                    UIOverlay.Icon.Failed, "Press anywhere to close.");
                return;
            }
            
            // Save data to cloud
            var saveResult = await cloudSaveManager.SaveCloudDataAsync();
            if (saveResult.Status == SaveResult.Success)
            {
                overlay.DisplayMessage("Cloud save success", "Your game data has been successfully saved to the cloud.",
                    UIOverlay.Icon.Success, "Press anywhere to close.");
            }
            else
            {
                overlay.DisplayMessage("Cloud save failed", saveResult.Message, UIOverlay.Icon.Failed, "Press anywhere to close.");
            }
        }
    }
}
