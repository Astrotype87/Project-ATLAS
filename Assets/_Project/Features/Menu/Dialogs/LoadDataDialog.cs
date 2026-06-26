using UnityEngine;
using UnityEngine.UI;
using KBCore.Refs;

using ProjectATLAS.UI;
using ProjectATLAS.CloudSave;

namespace ProjectATLAS.Menu
{
    public class LoadDataDialog : UIDialog
    {
        [Header("Components")]
        [SerializeField] private Button loadDataButton;
        
        [Header("References")]
        [SerializeField, Scene] private UIOverlay overlay;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            loadDataButton.onClick.AddListener(OnLoadDataFromCloud);
        }
        
        protected override void Start()
        {
            base.Start();
        }
        
        
        // PRIVATE METHODS
        private async void OnLoadDataFromCloud()
        {
            // Close dialog first to avoid overlay being hidden behind
            base.CloseDialog();
            
            // Display load cloud data overlay
            overlay.OpenOverlay();
            overlay.DisplayLoading("Loading cloud data...", "Please wait a moment.", UIOverlay.Icon.Loading, "");
            
            // Get CloudSaveManager instance
            CloudSaveManager cloudSaveManager = CloudSaveManager.Instance;
            if (cloudSaveManager == null)
            {
                overlay.DisplayMessage("Cloud load failed", "(Developer) CloudSaveManager instance is null.", UIOverlay.Icon.Failed, "Press anywhere to close.");
                return;
            }
            
            // Load data from cloud
            var loadResult = await cloudSaveManager.LoadCloudDataAsync();
            if (loadResult.Status == LoadResult.Success)
            {
                overlay.DisplayMessage("Cloud load success", "Your game data has been successfully loaded from the cloud.", UIOverlay.Icon.Success, "Press anywhere to close.");
            }
            else
            {
                overlay.DisplayMessage("Cloud load failed", loadResult.Message, UIOverlay.Icon.Failed, "Press anywhere to close.");
            }
        }
    }
}
