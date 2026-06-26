using ProjectATLAS.GameData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectATLAS.Cloud
{
    public class CloudLoadButton : MonoBehaviour
    {
        public Button loadButton;

        private void Awake()
        {
            if (loadButton != null)
                loadButton.onClick.AddListener(OnLoadButtonClicked);
        }

        private void OnDestroy()
        {
            if (loadButton != null)
                loadButton.onClick.RemoveListener(OnLoadButtonClicked);
        }

        private async void OnLoadButtonClicked()
        {
            //  Load cloud data
            await CloudServiceManager.Instance.LoadGameDataAsync();

            // Update local data
            // GameDataManager.Instance.ReplaceGameData(CloudServiceManager.Instance.CurrentGameData);
        }
    }
}
