using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Exceptions;
using Unity.Services.Leaderboards.Models;

using ProjectATLAS.Architecture;
using ProjectATLAS.GameData;
using ProjectATLAS.Gameplay;
using ProjectATLAS.Authentication;

namespace ProjectATLAS.Leaderboards
{
    public class LeaderboardsManager : PersistentSingletonMonoBehaviour<LeaderboardsManager>
    {
        [SerializeField] private GameDataManager gameDataService;
        [SerializeField] private LevelManager levelManager;
        
        // Campaign-Score for the production leaderboards, Campaign-Score and Campaign-points for development leaderboards.
        private const string CampaignScoreLeaderboardID = "Campaign-Score";
        private const string LeaderboardsUpdateQueueKey = "Leaderboards-Queue";
        
        public bool IsFetching { get; private set; }
        
        
        // PUBLIC METHODS
        /// <summary>
        /// Request leaderboards update for campaign score or specific best level scores,
        /// which will be processed once the internet is available.
        /// <br/>
        /// <br/> Call this method inside GameplayManager start lesson/simulation/minigame routine.
        /// <br/> Call this when LeaderboardsPage is opened as well (OpenPage())
        /// </summary>
        public void EnqueueLeaderboardsUpdate(int levelNumber)
        {
            List<int> queue = LoadQueueFromPlayerPrefs();
            
            if (!queue.Contains(0))
                queue.Add(0);
            
            if (!queue.Contains(levelNumber))
                queue.Add(levelNumber);
            
            SaveQueueToPlayerPrefs(queue);
            
            Debug.Log($"[LeaderboardsManager] Queued leaderboard update: {levelNumber}");
        } 
        
        /// <summary>
        /// Call this method when a leaderboards update for the campaign score or a specific level is successful.
        /// </summary>
        public void DequeueLeaderboardsUpdate(int levelNumber)
        {
            List<int> queue = LoadQueueFromPlayerPrefs();
            
            if (queue.Remove(levelNumber))
            {
                SaveQueueToPlayerPrefs(queue);
                Debug.Log($"[LeaderboardsManager] Removed from update queue: {levelNumber}");
            }
        }
        
        public void ClearLeaderboardsUpdateQueue()
        {
            List<int> queue = new();
            SaveQueueToPlayerPrefs(queue);
        }
        
        /// <summary>
        /// Attempts to update all pending leaderboard updates from queue.
        /// </summary>
        public void TryUpdateQueuedLeaderboards()
        {
            _ = UpdateQueuedLeaderboardsUpdate();
        }
        
        
        // PRIVATE STATIC METHODS : Accessing PlayerPrefs
        private static List<int> LoadQueueFromPlayerPrefs()
        {
            string data = PlayerPrefs.GetString(LeaderboardsUpdateQueueKey, "");
            if (string.IsNullOrEmpty(data))
                return new();
            
            return data.Split(",")
                .Select(s => int.TryParse(s, out int n) ? n : -1)
                .Where(n => n >= 0)
                .ToList();
        }
        
        private static void SaveQueueToPlayerPrefs(List<int> queue)
        {
            string data = string.Join(",", queue);
            PlayerPrefs.SetString(LeaderboardsUpdateQueueKey, data);
            PlayerPrefs.Save();
        }
        
        
        // PRIVATE METHODS: Accessing LeaderboardsService
        /// <summary>
        /// Updates all campaign and level scores automatically.
        /// Called every time the leaderboards page is opened.
        /// </summary>
        private async Task UpdateQueuedLeaderboardsUpdate()
        {
            if (!levelManager) levelManager = LevelManager.Instance;
            
            if (levelManager && gameDataService)
            {
                string username = AuthenticationManager.Instance.CachedUsername;
                string displayName = gameDataService.DetailsData.displayName;
                int avatarID = gameDataService.AvatarData.avatarIndex;
                
                float gameProgress = levelManager.GetCampaignProgress();
                int campaignScore = gameDataService.RecordsData.GetCampaignScore();
                
                
                // Re-update all data
                ClearLeaderboardsUpdateQueue();
                
                // Upload campaign score
                await UpdateCampaignScoreAsync(campaignScore, username, displayName, avatarID, gameProgress);
                
                // Upload all best level scores
                for (int i = 0; i < gameDataService.CampaignData.levels.Count; i++)
                {
                    var level = gameDataService.CampaignData.levels[i];
                    var levelRecord = gameDataService.RecordsData.GetBestLevelPoints(level.levelID);
                    int score = levelRecord.levelPoints;
                    
                    int levelNumber = int.Parse(level.levelID.Split('-')[1]);
                    
                    await UpdateLevelScoreAsync(levelNumber, score, username, displayName, avatarID);
                }
                
                // // Update queues
                // List<int> queue = LoadQueueFromPlayerPrefs();
                
                // // Upload campaign score
                // if (queue.Contains(0))
                // {
                //     await UpdateCampaignScoreAsync(campaignScore, username, displayName, avatarID, gameProgress);
                //     queue.Remove(0);
                // }
                
                // // Upload all best level scores
                // for (int i = 0; i < queue.Count; i++)
                // {
                //     int levelNumber = queue[i];
                //     string levelID = $"LVL-{levelNumber:00}";
                //     var levelRecord = gameDataService.RecordsData.GetBestLevelPoints(levelID);
                    
                //     if (levelRecord != null)
                //     {
                //         int score = levelRecord.levelPoints;
                //         await UpdateLevelScoreAsync(levelNumber, score, username, displayName, avatarID);
                //     }
                // }
            }
        }
        
        /// <summary>
        /// Updates the player's total campaign score on the global leaderboard.
        /// </summary>
        private async Task UpdateCampaignScoreAsync(double score, string username, string displayName, int avatarID, float gameProgress)
        {
            // Create metadata
            CampaignScoreMetadata metadata = new(username, displayName, avatarID, gameProgress);
            
            try
            {
                IsFetching = true;
                // If the score exists, it only overwrites it.
                // If the score is lower than store stored in the leaderboards, it keeps the higher score.
                await LeaderboardsService.Instance.AddPlayerScoreAsync(
                    CampaignScoreLeaderboardID,
                    score,
                    new AddPlayerScoreOptions { Metadata = metadata }
                );
                
                // Remove from queue after successful update
                DequeueLeaderboardsUpdate(0);
                
                Debug.Log($"Campaign score updated: {displayName} | {score} pts | progress {gameProgress}");
            }
            catch (LeaderboardsException ex)
            {
                Debug.LogError($"Leaderboard error updating campaign score: {ex.Reason} - {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Unexpected error updating campaign score: {ex.Message}");
            }
            finally
            {
                IsFetching = false;
            }
        }
        
        /// <summary>
        /// Updates the player's score for a specific level leaderboard.
        /// </summary>
        private async Task UpdateLevelScoreAsync(int level, double score, string username, string displayName, int avatarID)
        {
            // Create metadata
            BestLevelScoreMetadata metadata = new(username, displayName, avatarID);
            string levelLeaderboardID = $"Level-{level}";
            
            try
            {
                IsFetching = true;
                
                // If the score exists, it only overwrites it.
                // If the score is lower than store stored in the leaderboards, it keeps the higher score.
                await LeaderboardsService.Instance.AddPlayerScoreAsync(
                    levelLeaderboardID,
                    score,
                    new AddPlayerScoreOptions { Metadata = metadata }
                );
                
                // Remove from queue after successful update
                DequeueLeaderboardsUpdate(level);
                
                Debug.Log($"Level {level} score updated: {displayName} | {score} pts");
            }
            catch (LeaderboardsException ex)
            {
                Debug.LogError($"Leaderboard error updating level {level}: {ex.Reason} - {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Unexpected error updating level {level}: {ex.Message}");
            }
            finally
            {
                IsFetching = false;
            }
        }
        
        
        
        /// <summary>
        /// Gets campaign leaderboard scores after ensuring all updates are done.
        /// Returns null on failure.
        /// </summary>
        public async Task<LeaderboardScoresPage> GetCampaignScores(int page, int range)
        {
            if (IsFetching) return null;
            
            IsFetching = true;
            try
            {
                string leaderboardID = CampaignScoreLeaderboardID;
                int offset = (page - 1) * range;
                int limit = range;
                
                var scoreResponse = await LeaderboardsService.Instance.GetScoresAsync(
                    leaderboardID,
                    new GetScoresOptions
                    {
                        IncludeMetadata = true,
                        Offset = offset,
                        Limit = limit
                    }
                );
                
                return scoreResponse;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to fetch campaign scores: {ex.Message}");
                return null;
            }
            finally
            {
                IsFetching = false;
            }
        }
        
        /// <summary>
        /// Gets level leaderboard scores after ensuring all updates are done.
        /// Returns null on failure.
        /// </summary>
        public async Task<LeaderboardScoresPage> GetBestLevelScores(int level, int page, int range)
        {
            if (IsFetching) return null;
            
            IsFetching = true;
            try
            {
                string leaderboardID = $"Level-{level}";
                int offset = (page - 1) * range;
                int limit = range;
                
                var scoreResponse = await LeaderboardsService.Instance.GetScoresAsync(
                    leaderboardID,
                    new GetScoresOptions
                    {
                        IncludeMetadata = true,
                        Offset = offset,
                        Limit = limit
                    }
                );
                
                return scoreResponse;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to fetch level {level} scores: {ex.Message}");
                return null;
            }
            finally
            {
                IsFetching = false;
            }
        }
    }
    
    
    [Serializable]
    public struct CampaignScoreMetadata
    {
        public string username;
        public string displayName;
        public int avatarID;
        public float gameProgress;
        
        public CampaignScoreMetadata(string username, string displayName, int avatarID, float gameProgress)
        {
            this.username = username;
            this.displayName = displayName;
            this.avatarID = avatarID;
            this.gameProgress = gameProgress;
        }
    }
    
    [Serializable]
    public struct BestLevelScoreMetadata
    {
        public string username;
        public string displayName;
        public int avatarID;
        
        public BestLevelScoreMetadata(string username, string displayName, int avatarID)
        {
            this.username = username;
            this.displayName = displayName;
            this.avatarID = avatarID;
        }
    }
}
