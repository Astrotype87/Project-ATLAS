using ProjectATLAS.GameData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectATLAS.Cloud
{
    public class CloudSaveButton : MonoBehaviour
    {
        public Button saveButton;

        private void Awake()
        {
            if (saveButton != null)
                saveButton.onClick.AddListener(OnSaveButtonClicked);
        }

        private void OnDestroy()
        {
            if (saveButton != null)
                saveButton.onClick.RemoveListener(OnSaveButtonClicked);
        }

        private async void OnSaveButtonClicked()
        {
            // Push local data to CloudServiceManager
            CloudServiceManager.Instance.ReplaceGameData(GameDataManager.Instance.GameData);

            // Save to cloud
            await CloudServiceManager.Instance.SaveGameDataAsync();
        }
    }
}
