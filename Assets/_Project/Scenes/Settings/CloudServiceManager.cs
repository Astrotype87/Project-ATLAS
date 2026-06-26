using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using ProjectATLAS.GameData; // Make sure GameData class is inside this namespace

namespace ProjectATLAS.Cloud
{
    [Obsolete("CloudServiceManager is deprecated. Please use CloudSaveManager instead.")]
    public class CloudServiceManager : MonoBehaviour
    {
        public static CloudServiceManager Instance { get; private set; }

        public ProjectATLAS.GameData.GameData CurrentGameData { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            if (transform.parent == null)
                DontDestroyOnLoad(gameObject);
        }

        public async Task InitializeUGSAsync()
        {
            try
            {
                await UnityServices.InitializeAsync();

                if (!AuthenticationService.Instance.IsSignedIn)
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();

                Debug.Log("UGS Initialized and signed in successfully!");
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to initialize UGS: " + e.Message);
            }
        }

        public async Task SaveGameDataAsync()
        {
            if (CurrentGameData == null)
            {
                Debug.LogWarning("CurrentGameData is null!");
                return;
            }

            try
            {
                string json = JsonUtility.ToJson(CurrentGameData);
                var data = new Dictionary<string, object>
                {
                    { "GameData", json }
                };

                await CloudSaveService.Instance.Data.Player.SaveAsync(data);
                Debug.Log("GameData saved to cloud successfully!");
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to save GameData: " + e.Message);
            }
        }

        public async Task LoadGameDataAsync()
        {
            try
            {
                var keys = new HashSet<string> { "GameData" };
                var result = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

                if (result.TryGetValue("GameData", out var item))
                {
                    string json = item.Value.GetAsString();

                    if (!string.IsNullOrEmpty(json))
                    {
                        CurrentGameData = JsonUtility.FromJson<ProjectATLAS.GameData.GameData>(json);
                        Debug.Log("GameData loaded successfully!");
                    }
                    else
                    {
                        Debug.LogWarning("Cloud GameData is empty!");
                    }
                }
                else
                {
                    Debug.LogWarning("No GameData found in cloud!");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load GameData: " + e.Message);
            }
        }

        public void ReplaceGameData(ProjectATLAS.GameData.GameData newData)
        {
            if (newData == null)
            {
                Debug.LogWarning("ReplaceGameData called with null!");
                return;
            }

            CurrentGameData = newData;
            Debug.Log("CurrentGameData replaced locally!");
        }
    }
}
