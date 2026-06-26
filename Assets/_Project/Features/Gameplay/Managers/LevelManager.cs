using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using ProjectATLAS.Architecture;
using ProjectATLAS.GameData;

namespace ProjectATLAS.Gameplay
{
    public class LevelManager : PersistentSingletonMonoBehaviour<LevelManager>
    {
        [Header("Levels")]
        [SerializeField] private LevelData[] levels;
        [SerializeField] private PreTestData[] preTests;
        [SerializeField] private PostTestData[] postTests;
        
        [Header("References")]
        [SerializeField] private GameDataManager gameDataService;
        
        private Dictionary<int, LevelData[]> chapterLevelsMap;
        private Dictionary<int, LevelData> levelsMap;
        private Dictionary<int, PreTestData> preTestsMap;
        private Dictionary<int, PostTestData> postTestsMap;
        
        // PROPERTIES
        public string DisplayName
        {
            get => gameDataService ? gameDataService.DetailsData.displayName : "Player";
            set { if (gameDataService) gameDataService.DetailsData.displayName = value; }
        }
        public Difficulty SelectedDifficulty
        {
            get => gameDataService ? gameDataService.CampaignData.selectedDifficulty : Difficulty.Easy;
            set { if (gameDataService) gameDataService.CampaignData.selectedDifficulty = value; }
        }
        
        
        // MONOBEHAVIOUR METHODS
        protected override void Awake()
        {
            base.Awake();
            
            chapterLevelsMap = new Dictionary<int, LevelData[]>();
            var chapterKeys = levels.Where(ld => ld != null && ld.Chapter > 0).Select(ld => ld.Chapter).Distinct();
            foreach (var chapter in chapterKeys)
            {
                var chapterLevels = levels.Where(ld => ld != null && ld.Chapter == chapter).ToArray();
                chapterLevelsMap[chapter] = chapterLevels;
            }
            
            levelsMap = new Dictionary<int, LevelData>();
            for (int i = 0; i < levels.Length; i++)
            {
                var lv = levels[i];
                if (lv == null)
                {
                    Debug.LogWarning($"LevelManager: levels[{i}] is null in inspector.");
                    continue;
                }
                
                if (levelsMap.ContainsKey(lv.Number))
                    Debug.LogWarning($"LevelManager: duplicate level number {lv.Number} found (index {i}). Overwriting entry.");
                
                levelsMap[lv.Number] = lv;
            }
            
            preTestsMap = new Dictionary<int, PreTestData>();
            for (int i = 0; i < preTests.Length; i++)
            {
                var t = preTests[i];
                if (t == null)
                {
                    Debug.LogWarning($"LevelManager: preTests[{i}] is null in inspector.");
                    continue;
                }
                
                if (preTestsMap.ContainsKey(t.Chapter))
                    Debug.LogWarning($"LevelManager: duplicate preTest for chapter {t.Chapter}. Overwriting entry.");
                
                preTestsMap[t.Chapter] = t;
            }
            
            postTestsMap = new Dictionary<int, PostTestData>();
            for (int i = 0; i < postTests.Length; i++)
            {
                var t = postTests[i];
                if (t == null)
                {
                    Debug.LogWarning($"LevelManager: postTests[{i}] is null in inspector.");
                    continue;
                }
                
                if (postTestsMap.ContainsKey(t.Chapter))
                    Debug.LogWarning($"LevelManager: duplicate postTest for chapter {t.Chapter}. Overwriting entry.");
                
                postTestsMap[t.Chapter] = t;
            }
        }
        
        
        // PUBLIC METHODS
        public bool IsLevelAvailable(int levelNumber)
        {
            if (gameDataService == null || gameDataService.CampaignData == null)
            {
                Debug.LogWarning("LevelManager: gameData or CampaignData is null. Treating level as unavailable.");
                return false;
            }
            
            if (!levelsMap.TryGetValue(levelNumber, out LevelData levelData) || levelData == null)
            {
                Debug.LogWarning($"LevelManager: Level {levelNumber} not configured.");
                return false;
            }
            
            // Require that the chapter's pre-test is completed (e.g. for chapter N, pre-test N must be completed)
            int lastCompletedPreTest = LastCompletedPreTest(); // expected to return 0 if none
            bool isPreTestCompleted = lastCompletedPreTest >= levelData.Chapter;
            if (!isPreTestCompleted) return false;
            
            // Require that the level itself is not beyond lastCompletedLevel + 1
            int lastCompletedLevel = LastCompletedLevel(); // expected to return 0 if none
            bool isLevelAvailable = levelNumber == 1 || levelNumber <= (lastCompletedLevel + 1);
            return isLevelAvailable;
        }
        
        public bool IsPreTestAvailable(int chapter)
        {
            // Pre-test for chapter 1 is always available
            if (chapter == 1) return true;
            
            if (gameDataService == null || gameDataService.CampaignData == null)
            {
                Debug.LogWarning("LevelManager: gameData or CampaignData is null. Treating pre-test as unavailable.");
                return false;
            }
            
            // Ensure a pre-test is configured for this chapter
            if (!preTestsMap.TryGetValue(chapter, out PreTestData preTestData) || preTestData == null)
            {
                Debug.LogWarning($"LevelManager: No pre-test configured for chapter {chapter}.");
                return false;
            }
            
            // Pre-test for chapter N is available only if post-test for chapter (N-1) is completed.
            int lastCompletedPostTest = LastCompletedPostTest(); // 0 if none
            return lastCompletedPostTest >= (chapter - 1);
        }
        
        public bool IsPostTestAvailable(int chapter)
        {
            if (gameDataService == null || gameDataService.CampaignData == null)
            {
                Debug.LogWarning("LevelManager: gameData or CampaignData is null. Treating post-test as unavailable.");
                return false;
            }
            
            // Ensure a post-test is configured for this chapter
            if (!postTestsMap.TryGetValue(chapter, out PostTestData postTestData) || postTestData == null)
            {
                Debug.LogWarning($"LevelManager: No post-test configured for chapter {chapter}.");
                return false;
            }
            
            // Ensure we have levels configured for this chapter
            if (!chapterLevelsMap.TryGetValue(chapter, out LevelData[] levelsData) || levelsData == null || levelsData.Length == 0)
            {
                Debug.LogWarning($"LevelManager: No levels configured for chapter {chapter} — cannot unlock post-test.");
                return false;
            }
            
            // Get completed levels (from CampaignData) and check that every level in this chapter is completed.
            LevelData[] completedLevelsDataByChapter = GetCompletedLevelsDataByChapter(chapter) ?? new LevelData[0];
            
            // Use level numbers for comparison (defensive vs reference equality)
            var completedNumbers = new HashSet<int>(completedLevelsDataByChapter.Where(l => l != null).Select(l => l.Number));
            foreach (var lvl in levelsData)
            {
                if (lvl == null || !completedNumbers.Contains(lvl.Number))
                {
                    // At least one required level not completed
                    return false;
                }
            }
            
            // All levels in the chapter are completed -> post-test becomes available
            return true;
        }
        
        public bool IsLevelCompleted(string levelID) => gameDataService.CampaignData.IsLevelCompleted(levelID);
        
        public bool IsTestCompleted(string testID) => gameDataService.CampaignData.IsTestCompleted(testID);
        
        
        public CampaignData.Level GetLevelGameData(LevelData levelData)
        {
            return gameDataService.CampaignData.levels
                .Where(l => l.levelID == levelData.ID).FirstOrDefault();
        }
        
        public CampaignData.Test GetTestGameData(TestData testData)
        {
            CampaignData.Test test = gameDataService.CampaignData.preTests
                .Where(l => l.testID == testData.ID).FirstOrDefault();
            if (test != null) return test;
            
            test = gameDataService.CampaignData.postTests
                .Where(l => l.testID == testData.ID).FirstOrDefault();
            return test;
        }
        
        
        public LevelData GetFirstLevelDataByChapter(int chapter)
        {
            return levels.Where(ld => ld.Chapter == chapter).OrderBy(ld => ld.Number).First();
        }
        
        
        
        public int LastCompletedLevel()
        {
            List<CampaignData.Level> levelDatas = gameDataService.CampaignData.levels;
            List<int> completedLevelIDs = new();
            
            for (int i = 0; i < levelDatas.Count; i++)
            {
                // split each level id into LVL-01 into LVL and 01, take the second (index 1) token
                completedLevelIDs.Add(int.Parse(levelDatas[i].levelID.Split("-")[1]));
            }
            return completedLevelIDs.Any() ? completedLevelIDs.Max() : 0;
        }
        
        public int LastCompletedPreTest()
        {
            List<CampaignData.Test> testDatas = gameDataService.CampaignData.preTests;
            List<int> completedTestIDs = new();
            
            for (int i = 0; i < testDatas.Count; i++)
            {
                // split each test id into LVL-01 into LVL and 01, take the second (index 1) token
                completedTestIDs.Add(int.Parse(testDatas[i].testID.Split("-")[1]));
            }
            return completedTestIDs.Any() ? completedTestIDs.Max() : 0;
        }
        
        public int LastCompletedPostTest()
        {
            List<CampaignData.Test> testDatas = gameDataService.CampaignData.postTests;
            List<int> completedTestIDs = new();
            
            for (int i = 0; i < testDatas.Count; i++)
            {
                // split each test id into LVL-01 into LVL and 01, take the second (index 1) token
                if (testDatas[i].isCompleted) // If post-test is completed.
                    completedTestIDs.Add(int.Parse(testDatas[i].testID.Split("-")[1]));
            }
            return completedTestIDs.Any() ? completedTestIDs.Max() : 0;
        }
        
        
        public int TotalCompletedPreTests()
        {
            return gameDataService.CampaignData.preTests
                .Where(td => td.isCompleted)
                .Count();
        }
        
        public int TotalCompletedPostTests()
        {
            return gameDataService.CampaignData.postTests
                .Where(td => td.isCompleted)
                .Count();
        }
        
        
        
        
        
        public LevelData[] GetCompletedLevelsData()
        {
            string[] completedLevelsID = gameDataService.CampaignData.levels
                .Where(ld => ld.isCompleted)
                .Select(ld => ld.levelID)
                .ToArray();
            
            return levels
                .Where(l => completedLevelsID.Contains(l.ID))
                .ToArray();
        }
        
        public LevelData[] GetCompletedLevelsDataByChapter(int chapter)
        {
            string[] completedLevelsID = gameDataService.CampaignData.levels
                .Where(ld => ld.isCompleted)
                .Select(ld => ld.levelID)
                .ToArray();
            
            return levels
                .Where(l => completedLevelsID.Contains(l.ID))
                .Where(l => (l == null ? 0 : l.Chapter) == chapter)
                .ToArray();
        }
        
        /// <summary>
        /// Returns total campaign progress (0–100)
        /// based on levels, tests (pre + post), and medals.
        /// - 50% weight for level+test completions
        /// - 50% weight for medals earned
        /// </summary>
        public float GetCampaignProgress()
        {
            // Levels
            var (completedLevels, totalLevels) = GetCompletedAndTotalLevels();
            
            // Tests (pre + post)
            var (completedTests, totalTests) = GetCompletedAndTotalTests();
            
            // Medals
            var (obtainedMedals, totalMedals) = GetObtainedAndTotalMedals();
            
            int completedContent = completedLevels + completedTests;
            int totalContent = totalLevels + totalTests;
            
            if (totalContent == 0 || totalMedals == 0)
                return 0f;
            
            // Weight: 50% content completion, 50% medals
            float contentProgress = (float)completedContent / totalContent;
            float medalProgress = (float)obtainedMedals / totalMedals;
            
            float progress = (contentProgress * 0.5f + medalProgress * 0.5f) * 100f;
            return Mathf.Clamp(progress, 0f, 100f);
        }
        
        
        
        
        /// <summary> chapter = 0 means all </summary>
        public (int completed, int total) GetCompletedAndTotalChapters()
        {
            // Get total chapters
            int totalChapters = chapterLevelsMap.Keys.Count;
            
            // Count all completed levels, pre-test and post-test for each chapters
            int completedChapters = 0;
            
            foreach (var chapter in chapterLevelsMap.Keys)
            {
                var (completedLevels, totalLevels) = GetCompletedAndTotalLevels(chapter);
                var (completedTests, totalTests) = GetCompletedAndTotalTests(chapter);
                
                if (completedLevels == totalLevels && completedTests == totalTests)
                    completedChapters++;
            }
            return (completedChapters, totalChapters);
        }
        
        
        /// <summary> chapter = 0 means all </summary>
        public (int completed, int total) GetCompletedAndTotalLevels(int chapter = 0)
        {
            int completed = chapter <= 0
                ? GetCompletedLevelsData().Length
                : GetCompletedLevelsDataByChapter(chapter).Length;
            
            int total = chapter <= 0
                ? levels.Length
                : chapterLevelsMap[chapter].Length;
            
            return (completed, total);
        }
        
        /// <summary> chapter = 0 means all </summary>
        public (int obtained, int total) GetObtainedAndTotalMedals(int chapter = 0)
        {
            List<CampaignData.Level> levelGameDatas = chapter <= 0
                ? gameDataService.CampaignData.levels
                : gameDataService.CampaignData.levels
                    .Where(ld => levelsMap[int.Parse(ld.levelID.Split("-")[1])].Chapter == chapter).ToList();
            
            int obtained = 0;
            foreach (var levelGameData in levelGameDatas)
            {
                if (levelGameData.isBronze) obtained++;
                if (levelGameData.isSilver) obtained++;
                if (levelGameData.isGold) obtained++;
            }
            
            int totalLevels = chapter <= 0 ? levels.Length : chapterLevelsMap[chapter].Length;
            return (obtained, totalLevels * 3);
        }
        
        
        
        /// <summary>
        /// Returns the number of completed and total LESSON levels.
        /// chapter = 0 means all.
        /// </summary>
        public (int completed, int total) GetCompletedAndTotalLessonLevels(int chapter = 0)
        {
            return GetCompletedAndTotalLevelsByType(LevelType.Lesson, chapter);
        }
        
        /// <summary>
        /// Returns the number of completed and total SIMULATION levels.
        /// chapter = 0 means all.
        /// </summary>
        public (int completed, int total) GetCompletedAndTotalSimulationLevels(int chapter = 0)
        {
            return GetCompletedAndTotalLevelsByType(LevelType.Simulation, chapter);
        }
        
        /// <summary>
        /// Returns the number of completed and total CHALLENGE levels.
        /// chapter = 0 means all.
        /// </summary>
        public (int completed, int total) GetCompletedAndTotalChallengeLevels(int chapter = 0)
        {
            return GetCompletedAndTotalLevelsByType(LevelType.Challenge, chapter);
        }
        
        
        /// <summary>
        /// Returns the number of completed and total tests (pre + post).
        /// chapter = 0 means all chapters.
        /// </summary>
        public (int completed, int total) GetCompletedAndTotalTests(int chapter = 0)
        {
            if (gameDataService == null || gameDataService.CampaignData == null)
                return (0, 0);
            
            var preTestsData = gameDataService.CampaignData.preTests;
            var postTestsData = gameDataService.CampaignData.postTests;
            
            int completed = 0;
            int total = 0;
            
            if (chapter <= 0)
            {
                completed += preTestsData.Count(t => t.isCompleted);
                completed += postTestsData.Count(t => t.isCompleted);
                total = preTestsMap.Count + postTestsMap.Count;
            }
            else
            {
                if (preTestsMap.ContainsKey(chapter))
                {
                    total++;
                    if (preTestsData.Any(t => t.isCompleted && int.Parse(t.testID.Split("-")[1]) == chapter))
                        completed++;
                }
                
                if (postTestsMap.ContainsKey(chapter))
                {
                    total++;
                    if (postTestsData.Any(t => t.isCompleted && int.Parse(t.testID.Split("-")[1]) == chapter))
                        completed++;
                }
            }
            
            return (completed, total);
        }
        
        
        public (int completed, int total) GetCompletedAndTotalPreTests()
        {
            if (gameDataService == null || gameDataService.CampaignData == null)
                return (0, 0);
            
            var preTestsData = gameDataService.CampaignData.preTests;
            
            int completed = preTestsData.Count(t => t.isCompleted);
            int total = preTestsMap.Count;
            
            return (completed, total);
        }
        
        public (int completed, int total) GetCompletedAndTotalPostTests()
        {
            if (gameDataService == null || gameDataService.CampaignData == null)
                return (0, 0);
            
            var postTestsData = gameDataService.CampaignData.postTests;
            
            int completed = postTestsData.Count(t => t.isCompleted);
            int total = postTestsMap.Count;
            
            return (completed, total);
        }
        
        
        /// <summary>
        /// Internal helper to calculate completion stats by level type.
        /// </summary>
        private (int completed, int total) GetCompletedAndTotalLevelsByType(LevelType type, int chapter = 0)
        {
            if (gameDataService == null || gameDataService.CampaignData == null)
                return (0, 0);
            
            // All completed level IDs in campaign data
            string[] completedLevelIDs = gameDataService.CampaignData.levels
                .Where(ld => ld.isCompleted)
                .Select(ld => ld.levelID)
                .ToArray();
            
            // Filter all level data by type and chapter
            var filteredLevels = levels
                .Where(l => l != null && l.Type == type)
                .Where(l => chapter <= 0 || l.Chapter == chapter)
                .ToArray();
            
            // Count how many of those filtered levels are completed
            int completed = filteredLevels.Count(l => completedLevelIDs.Contains(l.ID));
            int total = filteredLevels.Length;
            
            return (completed, total);
        }
        
        
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(LevelManager))]
    public class LevelManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            LevelManager manager = (LevelManager)target;
            
            if (GUILayout.Button("Find & Order Assets"))
            {
                Undo.RecordObject(manager, "Find & Order Assets");
                
                // Find and sort LevelData
                string[] levelGuids = AssetDatabase.FindAssets("t:LevelData");
                var levels = levelGuids
                    .Select(g => AssetDatabase.LoadAssetAtPath<LevelData>(AssetDatabase.GUIDToAssetPath(g)))
                    .OrderBy(l => l.name) // sort by name
                    .ToArray();
                
                manager.GetType().GetField("levels", BindingFlags.NonPublic | BindingFlags.Instance)
                    .SetValue(manager, levels);
                
                // Find and sort PreTestData
                string[] preGuids = AssetDatabase.FindAssets("t:PreTestData");
                var preTests = preGuids
                    .Select(g => AssetDatabase.LoadAssetAtPath<PreTestData>(AssetDatabase.GUIDToAssetPath(g)))
                    .OrderBy(p => p.name)
                    .ToArray();
                
                manager.GetType().GetField("preTests", BindingFlags.NonPublic | BindingFlags.Instance)
                    .SetValue(manager, preTests);
                
                // Find and sort PostTestData
                string[] postGuids = AssetDatabase.FindAssets("t:PostTestData");
                var postTests = postGuids
                    .Select(g => AssetDatabase.LoadAssetAtPath<PostTestData>(AssetDatabase.GUIDToAssetPath(g)))
                    .OrderBy(p => p.name)
                    .ToArray();
                
                manager.GetType().GetField("postTests", BindingFlags.NonPublic | BindingFlags.Instance)
                    .SetValue(manager, postTests);
                
                EditorUtility.SetDirty(manager);
                Debug.Log("Assets populated and ordered in LevelManager!");
            }
        }
    }
#endif
}
