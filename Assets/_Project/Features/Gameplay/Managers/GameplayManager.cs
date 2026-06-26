using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

using Eflatun.SceneReference;
using AstrotypeTools.InspectorAttributes;

using ProjectATLAS.System;
using ProjectATLAS.Architecture;
using ProjectATLAS.GameData;
using ProjectATLAS.CloudSave;

using ProjectATLAS.Dialogue;
using ProjectATLAS.Quiz;
using ProjectATLAS.Simulation;
using ProjectATLAS.Minigame;
using ProjectATLAS.Leaderboards;
using ProjectATLAS.Gameplay.UI;

namespace ProjectATLAS.Gameplay
{
    using LevelRecord = RecordsData.LevelRecord;
    using TestRecord = RecordsData.TestRecord;
    
    /// <summary> Manages the flow of a campaign level gameplay. </summary>
    public class GameplayManager : SceneSingleton<GameplayManager>
    {
        [Header("Scenes")]
        [SerializeField] private SceneReference dialogueScene;
        [SerializeField] private SceneReference quizScene;
        [SerializeField] private SceneReference simulationScene;
        [SerializeField] private SceneReference minigameScene;
        
        [Header("UI")]
        [SerializeField] private GameplayUI gameplayUI;
        
        [Header("Camera")]
        [SerializeField, AssetOnly] private Camera cameraPrefab;
        [SerializeField] private GameObject cameraInstance;
        
        private CampaignLevelData currentLevelData;
        private Difficulty currentDifficulty;
        
        private DialogueManager dialogueManager;
        private QuizManager quizManager;
        private SimulationManager simulationManager;
        private MinigameManager minigameManager;
        
        private Coroutine currentGameplayCoroutine;
        private bool isLevelRoutineCompleted;
        private bool isGameplayTerminable;
        
        public CompleteAction CompleteAction { get; private set; }
        
        
        // MONOBEHAVIOUR METHODS
        protected override void Awake()
        {
            base.Awake();
            
            gameplayUI.OnResume += GameplayUI_OnResume;
            gameplayUI.OnRestart += GameplayUI_OnRestart;
            gameplayUI.OnQuit += GameplayUI_OnQuit;
        }
        
        
        // PUBLIC METHODS
        /// <summary> Starts the gameplay session with chosen campaign level and difficulty. </summary>
        public void StartGame(CampaignLevelData campaignLevelData, Difficulty difficulty)
        {
            this.currentLevelData = campaignLevelData;
            this.currentDifficulty = difficulty;
            
            currentGameplayCoroutine = StartCoroutine(RunGameplayRoutine(campaignLevelData, difficulty));
        }
        
        /// <summary> Pauses the gameplay. </summary>
        public void PauseGame()
        {
            gameplayUI.ShowPausePage();
            
            if (dialogueManager) dialogueManager.PauseDialogue();
            if (quizManager) quizManager.PauseQuiz();
            if (simulationManager) simulationManager.PauseSimulation();
            if (minigameManager) minigameManager.PauseMinigame();
        }
        
        /// <summary> Resumes the gameplay. </summary>
        public void ResumeGame()
        {
            gameplayUI.HidePausePage();
            
            if (dialogueManager) dialogueManager.ResumeDialogue();
            if (quizManager) quizManager.ResumeQuiz();
            if (simulationManager) simulationManager.ResumeSimulation();
            if (minigameManager) minigameManager.ResumeMinigame();
        }
        
        /// <summary>
        /// Unloads the currently loaded dialogue/quiz/simulation/minigame scene. <br/>
        /// Restarts the gameplay with the current level and difficulty.
        /// </summary>
        public void RestartGame()
        {
            // Reset time scale to 1
            Time.timeScale = 1.0f;
            
            // Stop the currently running gameplay routine
            if (currentGameplayCoroutine != null)
            {
                StopCoroutine(currentGameplayCoroutine);
                currentGameplayCoroutine = null;
            }
            
            // Hide pause page
            gameplayUI.HidePausePage();
            
            // Unload scenes if loaded
            UnloadSceneIfLoaded(dialogueScene);
            UnloadSceneIfLoaded(quizScene);
            UnloadSceneIfLoaded(simulationScene);
            UnloadSceneIfLoaded(minigameScene);
            
            // Restart gameplay routine
            StartCoroutine(RunGameplayRoutine(currentLevelData, currentDifficulty));
        }
        
        /// <summary> Terminates the gameplay and returns to Main Menu > Campaign. </summary>
        public void QuitGame()
        {
            // Reset time scale to 1
            Time.timeScale = 1.0f;
            
            // Hide pause page
            gameplayUI.HidePausePage();
            
            // Terminate gameplay
            CompleteAction = CompleteAction.Campaign;
            isGameplayTerminable = true;
        }
        
        /// <summary> Wait until the gameplay can be terminated, ended and gameplay scene unloaded. </summary>
        public IEnumerator WaitUntilGameplayTerminable()
        {
            yield return new WaitUntil(() => isGameplayTerminable);
        }
        
        
        // PRIVATE METHODS
        private IEnumerator RunGameplayRoutine(CampaignLevelData campaignLevelData, Difficulty difficulty)
        {
            do
            {
                IEnumerator levelRoutine = campaignLevelData switch
                {
                    PreTestData preTestLevelData => RunPreTestRoutine(preTestLevelData),
                    LessonLevelData lessonLevelData => RunLessonLevelRoutine(lessonLevelData, difficulty),
                    SimulationLevelData simulationLevelData => RunSimulationLevelRoutine(simulationLevelData, difficulty),
                    ChallengeLevelData challengeLevelData => RunChallengeLevelRoutine(challengeLevelData, difficulty),
                    PostTestData postTestLevelData => RunPostTestRoutine(postTestLevelData),
                    _ => null
                };
                
                // Open loading screen
                LoadingScreen.Instance.ShowInfo(campaignLevelData.Name, $"{campaignLevelData.Title} • {campaignLevelData.Type} Type • {difficulty}");
                yield return LoadingScreen.Instance.Open();
                
                // Destroy dummy camera if existing
                if (cameraInstance)
                {
                    Destroy(cameraInstance);
                    cameraInstance = null;
                }
                
                // Wait for level routine to finish
                if (levelRoutine != null)
                {
                    isLevelRoutineCompleted = false;
                    yield return levelRoutine;
                    isLevelRoutineCompleted = true;
                }
            }
            while (CompleteAction == CompleteAction.Restart);
            
            // Terminate gameplay
            isGameplayTerminable = true;
        }
        
        private IEnumerator RunPreTestRoutine(PreTestData preTestData)
        {
            // ----- DIALOGUE PHASE -----
            // Load dialogue scene
            var loadDialogueScene = SceneManager.LoadSceneAsync(dialogueScene.BuildIndex, LoadSceneMode.Additive);
            yield return loadDialogueScene.ReportLoadingProgress("Loading dialogue scene", 0.3333f, 0.6667f);
            dialogueManager = FindAnyObjectByType<DialogueManager>(FindObjectsInactive.Include);
            
            // Load dialogue prefab and initialize dialogue
            yield return dialogueManager.LoadDialogueAndGuidebooksPrefab(preTestData.DialoguePrefab, null);
            bool isSkippable = LevelManager.Instance.IsTestCompleted(preTestData.ID);
            dialogueManager.InitializeDialogue(isSkippable, preTestData.Chapter);
            
            // Close loading screen with confirmation
            yield return LoadingScreen.Instance.WaitForConfirmation();
            
            // Start dialogue and wait for dialogue scene to finish
            dialogueManager.StartDialogue();
            yield return LoadingScreen.Instance.Close();
            yield return dialogueManager.WaitForDialogueEnd();
            
            // Unload dialogue scene
            SceneManager.UnloadSceneAsync(dialogueScene.BuildIndex);
            
            
            // ----- QUIZ PHASE -----
            // Open loading screen
            yield return LoadingScreen.Instance.Open();
            
            // Load quiz scene
            var loadQuizScene = SceneManager.LoadSceneAsync(quizScene.BuildIndex, LoadSceneMode.Additive);
            yield return loadQuizScene.ReportLoadingProgress("Loading quiz scene");
            quizManager = FindAnyObjectByType<QuizManager>(FindObjectsInactive.Include);
            
            // Set quiz settings and run quiz routine
            quizManager.SetPreTestQuiz(preTestData.ChapterQuizData);
            quizManager.InitializeQuiz();
            quizManager.SetQuizInfo(preTestData.Name, preTestData.Title, preTestData.Description, preTestData.Objectives);
            
            // Close loading screen
            yield return LoadingScreen.Instance.Close();
            
            // Start quiz and wait for quiz scene to finish
            yield return quizManager.WaitForQuizEnd();
            
            // Get quiz results and gameplay score
            QuizResultData resultData = quizManager.ResultData;
            
            // Unload quiz scene
            SceneManager.UnloadSceneAsync(quizScene.BuildIndex);
            
            
            // ----- GAME DATA UPDATE PHASE -----
            var gameData = GameDataManager.Instance;
            
            // Check if previously completed
            bool isPreviouslyCompleted = gameData.CampaignData.IsTestCompleted(preTestData.ID);
            
            // Decide pre-test complete
            bool isLevelCompleted = true;
            gameData.CampaignData.UpdateTestProgress(preTestData.ID, isLevelCompleted);
            
            // Create and save test record to game data
            var preTestRecord = new TestRecord(
                preTestData.ID,
                (int)resultData.score,
                (int)resultData.maxScore,
                (float)resultData.time.TotalSeconds,
                GetCurrentDateTime()
            );
            gameData.RecordsData.preTestRecords.Add(preTestRecord);
            
            // Update complete/fail plays in statistics
            if (isLevelCompleted) gameData.StatisticsData.completePlays++;
            else gameData.StatisticsData.failedPlays++;
            
            // Save game data
            Task saveGameDataOperation = SaveGameData();
            
            
            // ----- LEVEL COMPLETE PHASE -----
            // Load dummy level complete camera.
            var instantiate = InstantiateAsync(cameraPrefab);
            yield return instantiate;
            cameraInstance = instantiate.Result[0].gameObject;
            
            // Display status in level complete/failed page
            gameplayUI.DisplayCompletePage();
            gameplayUI.DisplayCompleted(preTestData.Name, true);
            gameplayUI.SetMedalsVisible(false);
            
            // Display unlocks
            bool displayUnlocks = !isPreviouslyCompleted && isLevelCompleted;
            gameplayUI.DisplayUnlocks(displayUnlocks ? preTestData.UnlockDatas : null);
            
            // Wait for complete action (Menu, Restart, Campaign)
            yield return gameplayUI.WaitForCompleteAction();
            CompleteAction = gameplayUI.CompleteAction;
            
            // Wait for game data saving operation to finish (if player clicked too fast)
            yield return saveGameDataOperation;
        }
        
        private IEnumerator RunLessonLevelRoutine(LessonLevelData lessonLevelData, Difficulty difficulty)
        {
            // ----- DIALOGUE PHASE -----
            // Load dialogue scene
            var loadDialogueScene = SceneManager.LoadSceneAsync(dialogueScene.BuildIndex, LoadSceneMode.Additive);
            yield return loadDialogueScene.ReportLoadingProgress("Loading dialogue scene", 0.3333f, 0.6667f);
            dialogueManager = FindAnyObjectByType<DialogueManager>(FindObjectsInactive.Include);
            
            // Load dialogue prefab and initialize dialogue
            yield return dialogueManager.LoadDialogueAndGuidebooksPrefab(
                lessonLevelData.DialoguePrefab, lessonLevelData.GuidebookPrefab);
            bool isSkippable = LevelManager.Instance.IsLevelCompleted(lessonLevelData.ID);
            dialogueManager.InitializeDialogue(isSkippable);
            
            // Close loading screen with confirmation
            yield return LoadingScreen.Instance.WaitForConfirmation();
            
            // Start dialogue and wait for dialogue scene to finish
            dialogueManager.StartDialogue();
            yield return LoadingScreen.Instance.Close();
            yield return dialogueManager.WaitForDialogueEnd();
            
            // Unload dialogue scene
            SceneManager.UnloadSceneAsync(dialogueScene.BuildIndex);
            
            
            // ----- QUIZ PHASE -----
            // Open loading screen
            yield return LoadingScreen.Instance.Open();
            
            // Load quiz scene
            var loadQuizScene = SceneManager.LoadSceneAsync(quizScene.BuildIndex, LoadSceneMode.Additive);
            yield return loadQuizScene.ReportLoadingProgress("Loading quiz scene");
            quizManager = FindAnyObjectByType<QuizManager>(FindObjectsInactive.Include);
            
            // Set quiz settings and run quiz routine
            quizManager.SetQuizSettings(lessonLevelData.QuizData, difficulty, lessonLevelData.TimeBonus);
            quizManager.InitializeQuiz();
            quizManager.SetQuizInfo(lessonLevelData.Name, lessonLevelData.Title, lessonLevelData.Description, lessonLevelData.Objectives);
            quizManager.SetMedalsInfo(
                lessonLevelData.BronzeObjective,
                lessonLevelData.SilverObjective,
                lessonLevelData.GoldObjective.Replace("{goldTime}", lessonLevelData.GoldTime.ToString()));
            
            
            // Close loading screen
            yield return LoadingScreen.Instance.Close();
            
            // Start quiz and wait for quiz scene to finish
            yield return quizManager.WaitForQuizEnd();
            
            // Get quiz results and gameplay score
            float passingScorePercentage = quizManager.GetPassingScorePercentage();
            QuizResultData resultData = quizManager.ResultData;
            PointsEntry[] pointsEntries = quizManager.PointsEntries;
            
            // Unload quiz scene
            SceneManager.UnloadSceneAsync(quizScene.BuildIndex);
            
            
            // ----- GAME DATA UPDATE PHASE -----
            var gameData = GameDataManager.Instance;
            
            // Check if previously completed
            bool isPreviouslyCompleted = gameData.CampaignData.IsLevelCompleted(lessonLevelData.ID);
            
            // Decide if level complete or failed
            bool isLevelCompleted = resultData.ScorePercentage >= passingScorePercentage;
            bool isBronze = false, isSilver = false, isGold = false;
            if (isLevelCompleted)
            {
                isBronze = true;
                isSilver = resultData.ScorePercentage == 1.0f;
                isGold = resultData.time.TotalSeconds < lessonLevelData.GoldTime && isBronze && isSilver; 
                gameData.CampaignData.UpdateLevelProgress(lessonLevelData.ID, isLevelCompleted, isGold, isSilver, isBronze);
            }
            
            // Create level record
            var levelRecord = new LevelRecord(
                lessonLevelData.ID, difficulty,
                (int)resultData.score, (int)resultData.maxScore,
                (int)PointsEntry.GetTotalPoints(pointsEntries),
                (float)resultData.time.TotalSeconds, GetCurrentDateTime());
            gameData.RecordsData.levelRecords.Add(levelRecord);
            
            // Update complete/fail plays in statistics
            if (isLevelCompleted) gameData.StatisticsData.completePlays++;
            else gameData.StatisticsData.failedPlays++;
            
            // Save game data
            Task saveGameDataOperation = SaveGameData();
            
            
            // Save to leaderboards
            LeaderboardsManager.Instance.EnqueueLeaderboardsUpdate(lessonLevelData.Number);
            LeaderboardsManager.Instance.TryUpdateQueuedLeaderboards();
            
            
            // ----- LEVEL COMPLETE PHASE -----
            // Load dummy level complete camera.
            var instantiate = InstantiateAsync(cameraPrefab);
            yield return instantiate;
            cameraInstance = instantiate.Result[0].gameObject;
            
            // Display status in level complete/failed page and wait for complete action (Menu, Restart, Campaign)
            gameplayUI.DisplayCompletePage();
            gameplayUI.DisplayCompleted(lessonLevelData.Name, isLevelCompleted);
            gameplayUI.DisplayMedalsObtained(isBronze, isSilver, isGold);
            gameplayUI.DisplayMedalsObjectives(
                lessonLevelData.BronzeObjective, lessonLevelData.SilverObjective, lessonLevelData.GoldObjective,
                lessonLevelData.GoldTime);
            
            // Display unlocks
            bool displayUnlocks = !isPreviouslyCompleted && isLevelCompleted;
            gameplayUI.DisplayUnlocks(displayUnlocks ? lessonLevelData.UnlockDatas : null);
            
            yield return gameplayUI.WaitForCompleteAction();
            CompleteAction = gameplayUI.CompleteAction;
            
            // Wait for game data saving operation to finish (if player clicked too fast)
            yield return saveGameDataOperation;
        }
        
        private IEnumerator RunSimulationLevelRoutine(SimulationLevelData simulationLevelData, Difficulty difficulty)
        {
            // ----- DIALOGUE PHASE -----
            // // Load dialogue scene
            // var loadDialogueScene = SceneManager.LoadSceneAsync(dialogueScene.BuildIndex, LoadSceneMode.Additive);
            // yield return loadDialogueScene.ReportLoadingProgress("Loading dialogue scene", 0.3333f, 0.6667f);
            // dialogueManager = FindAnyObjectByType<DialogueManager>(FindObjectsInactive.Include);
            
            // // Load dialogue prefab and initialize dialogue
            // yield return dialogueManager.LoadDialoguePrefab(simulationLevelData.DialoguePrefab);
            // dialogueManager.InitializeDialogue();
            
            // // Close loading screen with confirmation
            // yield return LoadingScreen.Instance.WaitForConfirmation();
            // yield return LoadingScreen.Instance.Close();
            
            // // Start dialogue and wait for dialogue scene to finish
            // dialogueManager.StartDialogue();
            // yield return dialogueManager.WaitForDialogueEnd();
            
            // // Unload dialogue scene
            // SceneManager.UnloadSceneAsync(dialogueScene.BuildIndex);
            
            
            // ----- SIMULATION PHASE -----
            // Open loading screen
            yield return LoadingScreen.Instance.Open();
            
            // Load simulation scene
            var loadSimulationScene = SceneManager.LoadSceneAsync(simulationScene.BuildIndex, LoadSceneMode.Additive);
            yield return loadSimulationScene.ReportLoadingProgress("Loading simulation scene", 0.3333f, 1f);
            
            // Set simulation settings and initialize simulation
            simulationManager = FindAnyObjectByType<SimulationManager>(FindObjectsInactive.Include);
            yield return simulationManager.LoadSimulationPrefab(simulationLevelData.SimulationPrefab);
            simulationManager.InitializeSimulation(simulationLevelData, difficulty);
            
            // Close loading screen
            yield return LoadingScreen.Instance.Close();
            
            // Start simulation and wait for simulation scene to finish
            yield return simulationManager.WaitForSimulationEnd();
            
            // Get simulation results and gameplay score
            float playTime = simulationManager.PlayTime;
            float score = simulationManager.Score;
            float maxScore = simulationManager.MaxScore;
            PointsEntry[] pointsEntries = simulationManager.PointsEntries;
            
            bool isLevelCompleted = true; // Simulation levels are automatically completed
            bool isBronze = simulationManager.IsBronzeAwarded;
            bool isSilver = simulationManager.IsSilverAwarded;
            bool isGold = simulationManager.IsGoldAwarded;
            
            // Unload simulation scene
            SceneManager.UnloadSceneAsync(simulationScene.BuildIndex);
            
            
            // ----- GAME DATA UPDATE PHASE -----
            var gameData = GameDataManager.Instance;
            
            // Check if previously completed
            bool isPreviouslyCompleted = gameData.CampaignData.IsLevelCompleted(simulationLevelData.ID);
            
            // Update level progress
            gameData.CampaignData.UpdateLevelProgress(simulationLevelData.ID, isLevelCompleted, isGold, isSilver, isBronze);
            
            // Create level record
            var levelRecord = new LevelRecord(
                simulationLevelData.ID, difficulty,
                (int)score, (int)maxScore, (int)PointsEntry.GetTotalPoints(pointsEntries),
                playTime, GetCurrentDateTime()
            );
            gameData.RecordsData.levelRecords.Add(levelRecord);
            
            // Update complete/fail plays in statistics
            if (isLevelCompleted) gameData.StatisticsData.completePlays++;
            else gameData.StatisticsData.failedPlays++;
            
            // Save game data
            Task saveGameDataOperation = SaveGameData();
            
            
            // Save to leaderboards
            LeaderboardsManager.Instance.EnqueueLeaderboardsUpdate(simulationLevelData.Number);
            LeaderboardsManager.Instance.TryUpdateQueuedLeaderboards();
            
            
            // ----- LEVEL COMPLETE PHASE -----
            // Load dummy level complete camera.
            var instantiate = InstantiateAsync(cameraPrefab);
            yield return instantiate;
            cameraInstance = instantiate.Result[0].gameObject;
            
            // Display status in level complete/failed page
            gameplayUI.DisplayCompletePage();
            gameplayUI.DisplayCompleted(simulationLevelData.Name, isLevelCompleted);
            gameplayUI.DisplayMedalsObtained(isBronze, isSilver, isGold);
            gameplayUI.DisplayMedalsObjectives(
                simulationLevelData.BronzeObjective, simulationLevelData.SilverObjective, simulationLevelData.GoldObjective,
                simulationLevelData.GoldTime);
            
            // Display unlocks
            bool displayUnlocks = !isPreviouslyCompleted && isLevelCompleted;
            gameplayUI.DisplayUnlocks(displayUnlocks ? simulationLevelData.UnlockDatas : null);
            
            // Wait for complete action (Menu, Restart, Campaign)
            yield return gameplayUI.WaitForCompleteAction();
            CompleteAction = gameplayUI.CompleteAction;
            
            // Wait for game data saving operation to finish (if player clicked too fast)
            yield return saveGameDataOperation;
        }
        
        private IEnumerator RunChallengeLevelRoutine(ChallengeLevelData challengeLevelData, Difficulty difficulty)
        {
            // ----- DIALOGUE PHASE -----
            // // Load dialogue scene
            // var loadDialogueScene = SceneManager.LoadSceneAsync(dialogueScene.BuildIndex, LoadSceneMode.Additive);
            // yield return loadDialogueScene.ReportLoadingProgress("Loading dialogue scene", 0.3333f, 0.6667f);
            // dialogueManager = FindAnyObjectByType<DialogueManager>(FindObjectsInactive.Include);
            
            // // Load dialogue prefab and initialize dialogue
            // yield return dialogueManager.LoadDialoguePrefab(challengeLevelData.DialoguePrefab);
            // dialogueManager.InitializeDialogue();
            
            // // Close loading screen with confirmation
            // yield return LoadingScreen.Instance.WaitForConfirmation();
            // yield return LoadingScreen.Instance.Close();
            
            // // Start dialogue and wait for dialogue scene to finish
            // dialogueManager.StartDialogue();
            // yield return dialogueManager.WaitForDialogueEnd();
            
            // // Unload dialogue scene
            // SceneManager.UnloadSceneAsync(dialogueScene.BuildIndex);
            
            
            // ----- MINIGAME PHASE -----
            // Open loading screen
            yield return LoadingScreen.Instance.Open();
            
            // Load minigame scene
            var loadMinigameScene = SceneManager.LoadSceneAsync(minigameScene.BuildIndex, LoadSceneMode.Additive);
            yield return loadMinigameScene.ReportLoadingProgress("Loading minigame scene", 0.3333f, 1f);
            
            // Set minigame settings and run minigame routine
            minigameManager = FindAnyObjectByType<MinigameManager>(FindObjectsInactive.Include);
            yield return minigameManager.LoadMinigamePrefab(challengeLevelData.MinigamePrefab);
            minigameManager.InitializeMinigame(challengeLevelData, difficulty);
            
            // Close loading screen
            yield return LoadingScreen.Instance.Close();
            
            // Start minigame and wait for minigame scene to finish
            yield return minigameManager.WaitForMinigameEnd();
            
            // Get minigame results and gameplay score
            float playTime = minigameManager.PlayTime;
            float score = minigameManager.Score;
            float maxScore = minigameManager.MaxScore;
            PointsEntry[] pointsEntries = minigameManager.PointsEntries;
            
            bool isLevelCompleted = minigameManager.IsCompleted;
            bool isBronze = minigameManager.IsBronzeAwarded;
            bool isSilver = minigameManager.IsSilverAwarded;
            bool isGold = minigameManager.IsGoldAwarded;
            
            // Unload minigame scene
            SceneManager.UnloadSceneAsync(minigameScene.BuildIndex);
            
            
            // ----- GAME DATA UPDATE PHASE -----
            var gameData = GameDataManager.Instance;
            
            // Check if previously completed
            bool isPreviouslyCompleted = gameData.CampaignData.IsTestCompleted(challengeLevelData.ID);
            
            // Update level progress
            gameData.CampaignData.UpdateLevelProgress(challengeLevelData.ID, isLevelCompleted, isGold, isSilver, isBronze);
            
            // Create level record
            var levelRecord = new LevelRecord(
                challengeLevelData.ID, difficulty,
                (int)score, (int)maxScore, (int)PointsEntry.GetTotalPoints(pointsEntries),
                playTime, GetCurrentDateTime()
            );
            gameData.RecordsData.levelRecords.Add(levelRecord);
            
            // Update complete/fail plays in statistics
            if (isLevelCompleted) gameData.StatisticsData.completePlays++;
            else gameData.StatisticsData.failedPlays++;
            
            // Save game data
            Task saveGameDataOperation = SaveGameData();
            
            
            // Save to leaderboards
            LeaderboardsManager.Instance.EnqueueLeaderboardsUpdate(challengeLevelData.Number);
            LeaderboardsManager.Instance.TryUpdateQueuedLeaderboards();
            
            
            // ----- LEVEL COMPLETE PHASE -----
            // Load dummy level complete camera.
            var instantiate = InstantiateAsync(cameraPrefab);
            yield return instantiate;
            cameraInstance = instantiate.Result[0].gameObject;
            
            // Display status in level complete/failed page
            gameplayUI.DisplayCompletePage();
            gameplayUI.DisplayCompleted(challengeLevelData.Name, isLevelCompleted);
            gameplayUI.DisplayMedalsObtained(isBronze, isSilver, isGold);
            gameplayUI.DisplayMedalsObjectives(
                challengeLevelData.BronzeObjective, challengeLevelData.SilverObjective, challengeLevelData.GoldObjective,
                challengeLevelData.GoldTime);
            
            // Display unlocks
            bool displayUnlocks = !isPreviouslyCompleted && isLevelCompleted;
            gameplayUI.DisplayUnlocks(displayUnlocks ? challengeLevelData.UnlockDatas : null);
            
            // Wait for complete action (Menu, Restart, Campaign)
            yield return gameplayUI.WaitForCompleteAction();
            CompleteAction = gameplayUI.CompleteAction;
            
            // Wait for game data saving operation to finish (if player clicked too fast)
            yield return saveGameDataOperation;
        }
        
        private IEnumerator RunPostTestRoutine(PostTestData postTestData)
        {
            // ----- QUIZ PHASE -----
            // Open loading screen
            yield return LoadingScreen.Instance.Open();
            
            // Load quiz scene
            var loadQuizScene = SceneManager.LoadSceneAsync(quizScene.BuildIndex, LoadSceneMode.Additive);
            yield return loadQuizScene.ReportLoadingProgress("Loading quiz scene");
            quizManager = FindAnyObjectByType<QuizManager>(FindObjectsInactive.Include);
            
            // Set quiz settings and run quiz routine
            quizManager.SetPostTestQuiz(postTestData.ChapterQuizData);
            quizManager.InitializeQuiz();
            quizManager.SetQuizInfo(postTestData.Name, postTestData.Title, postTestData.Description, postTestData.Objectives);
            
            // Close loading screen with confirmation
            yield return LoadingScreen.Instance.WaitForConfirmation();
            yield return LoadingScreen.Instance.Close();
            
            // Start quiz and wait for quiz scene to finish
            yield return quizManager.WaitForQuizEnd();
            
            // Get quiz results and gameplay score
            QuizResultData resultData = quizManager.ResultData;
            
            // Unload quiz scene
            SceneManager.UnloadSceneAsync(quizScene.BuildIndex);
            
            
            // ----- GAME DATA UPDATE PHASE -----
            var gameData = GameDataManager.Instance;
            
            // Check if previously completed
            bool isPreviouslyCompleted = gameData.CampaignData.IsTestCompleted(postTestData.ID);
            
            // Decide post-test complete
            bool isLevelCompleted = resultData.ScorePercentage >= 0.5f;
            gameData.CampaignData.UpdateTestProgress(postTestData.ID, isLevelCompleted);
            
            // Create and save test record to game data
            var postTestRecord = new TestRecord(
                postTestData.ID,
                (int)resultData.score,
                (int)resultData.maxScore,
                (float)resultData.time.TotalSeconds,
                GetCurrentDateTime()
            );
            gameData.RecordsData.postTestRecords.Add(postTestRecord);
            
            // Update complete/fail plays in statistics
            if (isLevelCompleted) gameData.StatisticsData.completePlays++;
            else gameData.StatisticsData.failedPlays++;
            
            // Save game data
            Task saveGameDataOperation = SaveGameData();
            
            
            
            // ----- DIALOGUE PHASE IF PASSED POST-TEST -----
            if (isLevelCompleted)
            {
                // Load dialogue scene
                var loadDialogueScene = SceneManager.LoadSceneAsync(dialogueScene.BuildIndex, LoadSceneMode.Additive);
                yield return loadDialogueScene.ReportLoadingProgress("Loading dialogue scene");
                dialogueManager = FindAnyObjectByType<DialogueManager>(FindObjectsInactive.Include);
                
                // Load dialogue prefab and initialize dialogue
                yield return dialogueManager.LoadDialogueAndGuidebooksPrefab(postTestData.DialoguePrefab, null);
                bool isSkippable = LevelManager.Instance.IsTestCompleted(postTestData.ID);
                dialogueManager.InitializeDialogue(isSkippable);
                
                // Close loading screen without confirmation
                // yield return LoadingScreen.Instance.WaitForConfirmation();
                
                // Start dialogue and wait for dialogue scene to finish
                dialogueManager.StartDialogue();
                yield return LoadingScreen.Instance.Close();
                yield return dialogueManager.WaitForDialogueEnd();
                
                // Unload dialogue scene
                SceneManager.UnloadSceneAsync(dialogueScene.BuildIndex);
            }
            
            
            // ----- LEVEL COMPLETE PHASE -----
            // Load dummy level complete camera.
            var instantiate = InstantiateAsync(cameraPrefab);
            yield return instantiate;
            cameraInstance = instantiate.Result[0].gameObject;
            
            // Display status in level complete/failed page
            gameplayUI.DisplayCompletePage();
            gameplayUI.SetMedalsVisible(false);
            gameplayUI.DisplayCompleted(postTestData.Name, isLevelCompleted);
            
            // Display unlocks
            bool displayUnlocks = !isPreviouslyCompleted && isLevelCompleted;
            gameplayUI.DisplayUnlocks(displayUnlocks ? postTestData.UnlockDatas : null);
            
            // Wait for complete action (Menu, Restart, Campaign)
            yield return gameplayUI.WaitForCompleteAction();
            CompleteAction = gameplayUI.CompleteAction;
            
            // Wait for game data saving operation to finish (if player clicked too fast)
            yield return saveGameDataOperation;
        }
        
        
        // PRIVATE METHODS
        private async Task SaveGameData()
        {
            // Save GameData
            GameDataManager gameDataManager = GameDataManager.Instance;
            await gameDataManager.SaveDataAsync();
            
            // Save to cloud save
            var result = await CloudSaveManager.Instance.SaveCloudDataUnderAutoSave();
            Debug.Log("Auto Cloud Save Result: " + result.Status + " - " + result.Message);
        }
        
        
        private void UnloadSceneIfLoaded(SceneReference sceneRef)
        {
            if (sceneRef.UnsafeReason == SceneReferenceUnsafeReason.None)
            {
                var scene = SceneManager.GetSceneByBuildIndex(sceneRef.BuildIndex);
                if (scene.isLoaded)
                {
                    SceneManager.UnloadSceneAsync(scene);
                }
            }
        }
        
        
        // EVENT LISTENER METHODS
        private void GameplayUI_OnResume() => ResumeGame();
        
        private void GameplayUI_OnRestart() => RestartGame();
        
        private void GameplayUI_OnQuit() => QuitGame();
        
        
        // STATIC METHODS
        /// <summary> Consistently formatted as "dd/MM/yyyy HH:mm:ss" for saving date time </summary>
        private static string GetCurrentDateTime()
        {
            return DateTime.Now.ToString(Standard.GameData_DateTimeFormat);
        }
        
    }
    
    /// <summary> Next action after finishing a level. </summary>
    public enum CompleteAction
    {
        /// <summary> Returns to menu scene and opens menu page. </summary>
        Menu,
        /// <summary> Restarts the current level. </summary>
        Restart,
        /// <summary> Returns to menu scene and opens campaign page. </summary>
        Campaign,
        /// <summary> Returns to menu scene and opens freeplay page. </summary>
        Freeplay,
        /// <summary> Returns to menu scene and opens quiz page. </summary>
        Quiz,
        /// <summary> Returns to menu scene and opens simulation page. </summary>
        Simulation,
        /// <summary> Returns to menu scene and opens minigame page. </summary>
        Minigame
    }
}
