using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.CloudSave;
using Unity.Services.Authentication;
using UnityEngine.Networking;

using ProjectATLAS.Architecture;
using ProjectATLAS.GameData;

namespace ProjectATLAS.CloudSave
{
    using GameData = ProjectATLAS.GameData.GameData;
    
    /// <summary> Facade singleton class for accessing Unity Cloud Save features. </summary>
    public class CloudSaveManager : PersistentSingletonMonoBehaviour<CloudSaveManager>
    {
        [SerializeField] private bool disableAutoSave = false;
        [SerializeField] private string websiteToPing = "https://google.com";
        [SerializeField] private int requestTimeout = 5;
        [SerializeField] private GameDataManager gameDataManager;
        
        public const string GameDataKey = "GameData";
        
        
        // PUBLIC METHODS
        /// <summary> Checks the network status of the device. </summary>
        public async Task<CloudSaveResult> CheckNetworkStatusAsync()
        {
            if (IsDeviceOffline())
            {
                return new CloudSaveResult(NetworkResult.Offline, "The device is offline.");
            }
            else if (!await CheckInternetAccessAsync())
            {
                return new CloudSaveResult(NetworkResult.NoInternetAccess, "Cannot connect to the internet.");
            }
            else
            {
                return new CloudSaveResult(NetworkResult.Online, "Internet connection is available.");
            }
        }
        
        /// <summary> Loads game data from UGS Cloud Save and assigns the data to an instance of GameDataManager. </summary>
        public async Task<CloudSaveResult> LoadCloudDataAsync()
        {
            // Check if GameDataManager is assigned
            if (gameDataManager == null)
                return new CloudSaveResult(LoadResult.GameDataManagerNotAssigned, "GameDataManager instance is not assigned.");
            
            // Check network status
            var networkStatus = await CheckNetworkStatusAsync();
            if (networkStatus.Status != NetworkResult.Online)
                return networkStatus;
            
            // Check if UGS is initialized
            if (UnityServices.State != ServicesInitializationState.Initialized)
                return new CloudSaveResult(LoadResult.UGSNotInitialized, "Unity Gaming Services is not initialized.");
            
            // Check if user is signed in
            if (!AuthenticationService.Instance.IsSignedIn)
                return new CloudSaveResult(LoadResult.NotSignedIn, "User is not signed in to Unity Gaming Services.");
            
            
            // Load data from Cloud Save
            try
            {
                var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(
                    new HashSet<string> { GameDataKey });
                
                if (!playerData.ContainsKey(GameDataKey))
                    return new CloudSaveResult(LoadResult.DataNotFound, "No game data found in Cloud Save.");
                
                string json = playerData[GameDataKey].Value.GetAsString();
                
                try
                {
                    GameData loadedData = JsonUtility.FromJson<GameData>(json);
                    gameDataManager.GameData = loadedData;
                    
                    return new CloudSaveResult(LoadResult.Success, "Game data loaded successfully from Cloud Save.");
                }
                catch (Exception e)
                {
                    return new CloudSaveResult(LoadResult.DeserializationFailed, $"Failed to deserialize game data: {e.Message}");
                }
            }
            catch (Exception e)
            {
                return new CloudSaveResult(LoadResult.RequestFailed, $"Failed to load data from Cloud Save: {e.Message}");
            }
        }
        
        /// <summary> Takes the game data from GameDataManager instance and saves it to UGS Cloud Save. </summary>
        public async Task<CloudSaveResult> SaveCloudDataAsync()
        {
            // Check if GameDataManager is assigned
            if (gameDataManager == null)
                return new CloudSaveResult(SaveResult.GameDataManagerNotAssigned, "GameDataManager instance is not assigned.");
            
            // Check network status
            var networkStatus = await CheckNetworkStatusAsync();
            if (networkStatus.Status != NetworkResult.Online)
                return networkStatus;
            
            // Check if UGS is initialized
            if (UnityServices.State != ServicesInitializationState.Initialized)
                return new CloudSaveResult(SaveResult.UGSNotInitialized, "Unity Gaming Services is not initialized.");
            
            // Check if user is signed in
            if (!AuthenticationService.Instance.IsSignedIn)
                return new CloudSaveResult(SaveResult.NotSignedIn, "User is not signed in to Unity Gaming Services.");
            
            
            // Serialize game data to JSON
            string json = JsonUtility.ToJson(gameDataManager.GameData, true);
            var playerData = new Dictionary<string, object>
            {
                { GameDataKey, json }
            };
            
            
            // Save data to Cloud Save
            try
            {
                await CloudSaveService.Instance.Data.Player.SaveAsync(playerData);
                return new CloudSaveResult(SaveResult.Success, "Game data saved successfully to Cloud Save.");
            }
            catch (Exception e)
            {
                return new CloudSaveResult(SaveResult.RequestFailed, $"Failed to save data to Cloud Save: {e.Message}");
            }
        }
        
        
        public async Task<CloudSaveResult> DeleteCloudDataAsync()
        {
            // Check network status
            _ = await CheckNetworkStatusAsync();
            
            // Check if UGS is initialized
            if (UnityServices.State != ServicesInitializationState.Initialized)
                return new CloudSaveResult(DeleteResult.UGSNotInitialized, "Unity Gaming Services is not initialized.");
            
            // Check if user is signed in
            if (!AuthenticationService.Instance.IsSignedIn)
                return new CloudSaveResult(DeleteResult.NotSignedIn, "User is not signed in to Unity Gaming Services.");
            
            
            // Delete data from Cloud Save
            try
            {
                await CloudSaveService.Instance.Data.Player.DeleteAllAsync();
                return new CloudSaveResult(DeleteResult.Success, "Game data deleted successfully from Cloud Save.");
            }
            catch (Exception e)
            {
                return new CloudSaveResult(DeleteResult.RequestFailed, $"Failed to delete data from Cloud Save: {e.Message}");
            }
        }
        
        
        /// <summary> Saves game data to cloud automatically if auto-save is enabled. </summary>
        public async Task<CloudSaveResult> SaveCloudDataUnderAutoSave()
        {
            if (disableAutoSave)
                return new CloudSaveResult(SaveResult.AutoSaveDisabled, "Auto-save is disabled.");
            
            return await SaveCloudDataAsync();
        }
        
        
        
        // PRIVATE METHODS
        private bool IsDeviceOffline()
        {
            return Application.internetReachability == NetworkReachability.NotReachable;
        }
        
        private async Task<bool> CheckInternetAccessAsync()
        {
            using UnityWebRequest request = UnityWebRequest.Head(websiteToPing);
            request.timeout = requestTimeout;
            await request.SendWebRequest();
            
            bool isConnectionError = request.result == UnityWebRequest.Result.ConnectionError;
            bool isProtocolError = request.result == UnityWebRequest.Result.ProtocolError;
            
            return !isConnectionError && !isProtocolError && request.responseCode == 200;
        }
    }
}
