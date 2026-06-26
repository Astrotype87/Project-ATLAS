using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

using ProjectATLAS.Architecture;

namespace ProjectATLAS.GameData
{
    public class GameDataManager : PersistentSingletonMonoBehaviour<GameDataManager>
    {
        [Header("File Settings")]
        [SerializeField] private string fileName = "gamedata.json";
        [Header("Game Data")]
        [SerializeField] private GameData gameData;
        
        public bool IsLoaded { get; private set; } = false;
        
        // Saving queue management
        private bool isSaving = false;
        private bool hasPendingSave = false;
        
        // PROPERTIES
        public GameData GameData { get => gameData; set => gameData = value; }
        
        public AccountCacheData AccountCacheData => gameData.accountCacheData ??= new AccountCacheData();
        public DetailsData DetailsData => gameData.detailsData ??= new DetailsData();
        public AvatarData AvatarData => gameData.avatarData ??= new AvatarData();
        public CampaignData CampaignData => gameData.campaignData ??= new CampaignData();
        public RecordsData RecordsData => gameData.recordsData ??= new RecordsData();
        public StatisticsData StatisticsData => gameData.statisticsData ??= new StatisticsData();
        public SettingsData SettingsData => gameData.settingsData ??= new SettingsData();
        
        public event Action<GameData> OnGameDataLoaded;
        
        
        // PUBLIC METHODS
        public void ResetData()
        {
            gameData = new GameData();
            SaveData();
        }
        
        public void ResetDataWithPlaceholder()
        {
            gameData = new GameData
            {
                detailsData = new DetailsData
                {
                    displayName = "Player",
                    age = 18,
                    gender = "Male",
                    completedFirstTime = false
                }
            };
            
            SaveData();
        }
        
        public void ResetDataExceptDetails()
        {
            DetailsData detailsData = gameData.detailsData;
            
            gameData = new GameData
            {
                detailsData = detailsData
            };
            
            SaveData();
        }
        
        public async Task SaveDataAsync()
        {
            // Queue save if one is already in progress
            if (isSaving)
            {
                hasPendingSave = true;
                return;
            }
            
            isSaving = true;
            
            try
            {
                string filePath = GetFilePath();
                string json = JsonUtility.ToJson(gameData, true);
                await File.WriteAllTextAsync(filePath, json);
                Debug.Log($"Game data saved to {filePath}!");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameDataService] Error while saving data: {ex.Message}");
            }
            finally
            {
                isSaving = false;
                
                // If another save was requested during the save, run it now
                if (hasPendingSave)
                {
                    hasPendingSave = false;
                    _ = SaveDataAsync(); // fire and forget queued save
                }
            }
        }
        
        public async Task LoadDataAsync()
        {
            string filePath = GetFilePath();
            if (File.Exists(filePath))
            {
                string json = await File.ReadAllTextAsync(filePath);
                gameData = JsonUtility.FromJson<GameData>(json);
                Debug.Log($"Game data loaded from {GetFilePath()}!");
            }
            else
            {
                Debug.LogWarning("Save file not found.");
                gameData = new GameData();
            }
            
            // Ensure all data fields exist
            gameData ??= new GameData();
            gameData.detailsData ??= new DetailsData();
            gameData.avatarData ??= new AvatarData();
            gameData.campaignData ??= new CampaignData();
            gameData.recordsData ??= new RecordsData();
            gameData.statisticsData ??= new StatisticsData();
            gameData.settingsData ??= new SettingsData();
            
            OnGameDataLoaded?.Invoke(gameData);
            
            IsLoaded = true;
        }
        
        public void SaveData()
        {
            _ = SaveDataAsync();
        }
        
        public void LoadData()
        {
            _ = LoadDataAsync();
        }
        
        public void UnloadData()
        {
            gameData = new();
        }
        
        public string GetFilePath()
        {
            return $"{Application.persistentDataPath}/{fileName}";
        }
    }
}
