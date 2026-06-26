using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;

using ProjectATLAS.Gameplay;

namespace ProjectATLAS.GameData
{
    [Serializable]
    public class GameData
    {
        public AccountCacheData accountCacheData;
        public DetailsData detailsData;
        public AvatarData avatarData;
        public CampaignData campaignData;
        public RecordsData recordsData;
        public StatisticsData statisticsData;
        public SettingsData settingsData;
        
        public GameData()
        {
            accountCacheData = new AccountCacheData();
            detailsData = new DetailsData();
            avatarData = new AvatarData();
            campaignData = new CampaignData();
            recordsData = new RecordsData();
            statisticsData = new StatisticsData();
            settingsData = new SettingsData();
        }
    }
    
    
    // OTHER CLASSES
    [Serializable]
    public class AccountCacheData
    {
        public string username;
    }
    
    [Serializable]
    public class DetailsData
    {
        public string displayName;
        public int age;
        public string gender;
        
        /// <summary> Has displayed welcome screen when you first play the game? </summary>
        public bool completedFirstTime;
    }
    
    [Serializable]
    public class AvatarData
    {
        public int avatarIndex;
        public Color primaryColor;
        public Color secondaryColor;
    }
    
    [Serializable]
    public class CampaignData
    {
        public List<Level> levels = new();
        public List<Test> preTests = new();
        public List<Test> postTests = new();
        public Difficulty selectedDifficulty; // Selected difficulty in campaign level page
        
        /// <summary> Update level progress in game data. </summary>
        public void UpdateLevelProgress(string levelID, bool isCompleted, bool isGold, bool isSilver, bool isBronze)
        {
            Level level = levels.Where(l => l.levelID == levelID).FirstOrDefault();
            
            if (level == null)
            {
                levels.Add(new(levelID, true, isGold, isSilver, isBronze));
            }
            else
            {
                level.isCompleted |= isCompleted;
                level.isGold |= isGold;
                level.isSilver |= isSilver;
                level.isBronze |= isBronze;
            }
        }
        
        /// <summary> Update pre-test or post-test progress in game data. </summary>
        public void UpdateTestProgress(string testID, bool isCompleted)
        {
            // Convert PRE-1 or POST-1 into { PRE, 1 } or { POST, 1 }
            string testType = testID.Split("-")[0];
            
            if (testType == "PRE")
            {
                Test test = preTests.Where(t => t.testID == testID).FirstOrDefault();
                
                if (test == null) preTests.Add(new(testID, isCompleted));
                else test.isCompleted |= isCompleted;
            }
            else if (testType == "POST")
            {
                Test test = postTests.Where(t => t.testID == testID).FirstOrDefault();
                
                if (test == null) postTests.Add(new(testID, isCompleted));
                else test.isCompleted |= isCompleted;
            }
        }
        
        /// <summary> Check if level is completed by levelID. </summary>
        public bool IsLevelCompleted(string levelID)
        {
            Level level = levels.FirstOrDefault(l => l.levelID == levelID);
            return level != null && level.isCompleted;
        }
        
        /// <summary> Check if pre-test or post-test is completed by testID. </summary>
        public bool IsTestCompleted(string testID)
        {
            string testType = testID.Split("-")[0];
            
            if (testType == "PRE")
            {
                Test test = preTests.FirstOrDefault(t => t.testID == testID);
                return test != null && test.isCompleted;
            }
            else if (testType == "POST")
            {
                Test test = postTests.FirstOrDefault(t => t.testID == testID);
                return test != null && test.isCompleted;
            }
            
            return false; // Unknown test type
        }
        
        [Serializable]
        public class Level
        {
            public string levelID;
            public bool isCompleted;
            public bool isGold;
            public bool isSilver;
            public bool isBronze;
            
            public Level(string levelData, bool isCompleted, bool isGold, bool isSilver, bool isBronze)
            {
                this.levelID = levelData;
                this.isCompleted = isCompleted;
                this.isGold = isGold;
                this.isSilver = isSilver;
                this.isBronze = isBronze;
            }
        }
        
        [Serializable]
        public class Test
        {
            public string testID;
            public bool isCompleted;
            
            public Test(string testID, bool isCompleted)
            {
                this.testID = testID;
                this.isCompleted = isCompleted;
            }
        }
    }
    
    [Serializable]
    public class RecordsData
    {
        public List<LevelRecord> levelRecords = new();
        public List<TestRecord> preTestRecords = new();
        public List<TestRecord> postTestRecords = new();
        
        /// <summary> Returns the best points of a chosen level. (total score with time bonus). </summary>
        public LevelRecord GetBestLevelPoints(string levelID)
        {
            return levelRecords
                .Where(l => l.levelID == levelID)
                .OrderByDescending(l => l.levelPoints)
                .FirstOrDefault();
        }
        
        /// <summary> Returns the shortest time you ever played a level. </summary>
        public LevelRecord GetBestLevelTime(string levelID)
        {
            return levelRecords
                .Where(l => l.levelID == levelID)
                .OrderBy(l => l.time)
                .FirstOrDefault();
        }
        
        /// <summary> Returns the date time of last time you played a level. </summary>
        public LevelRecord GetLastTimePlayedLevel(string levelID)
        {
            return levelRecords
                .Where(l => l.levelID == levelID)
                .OrderByDescending(r =>
                {
                    if (DateTime.TryParseExact(r.dateTime,
                        Standard.GameData_DateTimeFormat,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime dt))
                            return dt;
                    return DateTime.MinValue;
                })
                .FirstOrDefault();
        }
        
        /// <summary> Returns the best score of a pre-test or post-test. </summary>
        public TestRecord GetBestTestScore(string testID)
        {
            TestRecord test = preTestRecords
                .Where(l => l.testID == testID)
                .OrderByDescending(l => l.score)
                .FirstOrDefault();
            if (test != null) return test;
            
            test = postTestRecords.Where(l => l.testID == testID)
                .OrderByDescending(l => l.score)
                .FirstOrDefault();
            return test;
        }
        
        /// <summary> Returns the sum of all best points from all levels (excluding pre-test and post-test). </summary>
        public int GetCampaignScore()
        {
            return levelRecords
                .GroupBy(lr => lr.levelID)             // group by level ID
                .Select(g => g.Max(lr => lr.levelPoints)) // take the best score per level
                .Sum();                                // sum them all
        }
        
        
        [Serializable]
        public class LevelRecord
        {
            /// <summary> ID of the level. (ex: LVL-01) </summary>
            public string levelID;
            /// <summary> Difficulty a level session is played with. </summary>
            public Difficulty difficulty;
            
            /// <summary> Represent quiz score, simulation points, or minigame score. </summary>
            public int score;
            /// <summary> Represent maximum quiz score, simulation points, or minigame score. </summary>
            public int maxScore;
            
            /// <summary> This is the level points with both quiz/sim/mini score and times, used on leaderboards. </summary>
            public int levelPoints;
            /// <summary> Play time taken. </summary>
            public float time;
            /// <summary> Date and time the game is finished. </summary>
            public string dateTime;
            
            public LevelRecord(string levelID, Difficulty difficulty, int score, int maxScore, int levelPoints, float time, string dateTime)
            {
                this.levelID = levelID;
                this.difficulty = difficulty;
                this.score = score;
                this.maxScore = maxScore;
                this.levelPoints = levelPoints;
                this.time = time;
                this.dateTime = dateTime;
            }
        }
        
        [Serializable]
        public class TestRecord
        {
            /// <summary> ID of the pre-test or post-test. (ex: PRE-01, POST-01) </summary>
            public string testID;
            
            /// <summary> Represent pre-test or post-test quiz score. </summary>
            public int score;
            /// <summary> Represent maximum pre-test or post-test quiz score. </summary>
            public int maxScore;
            
            /// <summary>  </summary>
            public float time;
            /// <summary>  </summary>
            public string dateTime;
            
            public TestRecord(string testID, int score, int maxScore, float time, string dateTime)
            {
                this.testID = testID;
                this.score = score;
                this.maxScore = maxScore;
                this.time = time;
                this.dateTime = dateTime;
            }
        }
    }
    
    [Serializable]
    public class StatisticsData
    {
        public double timeSpentPlaying;
        public double timeSpentInStatistics;
        public int completePlays;
        public int failedPlays;
    }
    
    [Serializable]
    public class SettingsData
    {
        public float masterVolume;
        public float musicVolume;
        public float sfxVolume;
        public float uiVolume;
    }
    
}
