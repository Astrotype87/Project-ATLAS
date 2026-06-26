using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

using Eflatun.SceneReference;
using AstrotypeTools.InspectorAttributes;

using ProjectATLAS.System;
using ProjectATLAS.Architecture;
using ProjectATLAS.Quiz;
using ProjectATLAS.Simulation;
using ProjectATLAS.Minigame;
using ProjectATLAS.Gameplay;
using ProjectATLAS.Freeplay.UI;

namespace ProjectATLAS.Freeplay
{
    /// <summary> Manages the flow of a freeplay gameplay. </summary>
    public class FreeplayManager : SceneSingleton<FreeplayManager>
    {
        [Header("Scenes")]
        [SerializeField] private SceneReference quizScene;
        [SerializeField] private SceneReference simulationScene;
        [SerializeField] private SceneReference minigameScene;
        
        [Header("UI")]
        [SerializeField] private FreeplayUI freeplayUI;
        
        [Header("Camera")]
        [SerializeField, AssetOnly] private Camera cameraPrefab;
        [SerializeField] private GameObject cameraInstance;
        
        private FreeplayGameType currentGameType;
        private object currentData;
        private Difficulty currentDifficulty;
        
        private QuizManager quizManager;
        private SimulationManager simulationManager;
        private MinigameManager minigameManager;
        
        private Coroutine currentFreeplayCoroutine;
        private bool isFreeplayRoutineCompleted;
        private bool isFreeplayTerminable;
        
        public CompleteAction CompleteAction { get; private set; }
        
        
        // MONOBEHAVIOUR METHODS
        protected override void Awake()
        {
            base.Awake();
            
            freeplayUI.OnResume += FreeplayUI_OnResume;
            freeplayUI.OnRestart += FreeplayUI_OnRestart;
            freeplayUI.OnQuit += FreeplayUI_OnQuit;
        }
        
        
        // PUBLIC METHODS
        /// <summary> Starts the gameplay session with chosen game type, data, and difficulty. </summary>
        public void StartGame(FreeplayGameType gameType, object data, Difficulty difficulty)
        {
            Debug.Log("StartGame Freeplay");
            
            this.currentGameType = gameType;
            this.currentData = data;
            this.currentDifficulty = difficulty;
            
            currentFreeplayCoroutine = StartCoroutine(RunFreeplayRoutine(gameType, data, difficulty));
        }
        
        /// <summary> Pauses the gameplay. </summary>
        public void PauseGame()
        {
            freeplayUI.ShowPausePage();
            
            // if (dialogueManager) dialogueManager.PauseDialogue();
            if (quizManager) quizManager.PauseQuiz();
            if (simulationManager) simulationManager.PauseSimulation();
            if (minigameManager) minigameManager.PauseMinigame();
        }
        
        /// <summary> Resumes the gameplay. </summary>
        public void ResumeGame()
        {
            freeplayUI.HidePausePage();
            
            // if (dialogueManager) dialogueManager.ResumeDialogue();
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
            if (currentFreeplayCoroutine != null)
            {
                StopCoroutine(currentFreeplayCoroutine);
                currentFreeplayCoroutine = null;
            }
            
            // Hide pause page
            freeplayUI.HidePausePage();
            
            // Unload scenes if loaded
            UnloadSceneIfLoaded(quizScene);
            UnloadSceneIfLoaded(simulationScene);
            UnloadSceneIfLoaded(minigameScene);
            
            // Restart gameplay routine
            StartCoroutine(RunFreeplayRoutine(currentGameType, currentData, currentDifficulty));
        }
        
        /// <summary> Terminates the gameplay and returns to Main Menu > Campaign. </summary>
        public void QuitGame()
        {
            // Reset time scale to 1
            Time.timeScale = 1.0f;
            
            // Hide pause page
            freeplayUI.HidePausePage();
            
            // Terminate gameplay
            CompleteAction = currentGameType switch
            {
                FreeplayGameType.Quiz => CompleteAction.Quiz,
                FreeplayGameType.Simulation => CompleteAction.Simulation,
                FreeplayGameType.Minigame => CompleteAction.Minigame,
                _ => CompleteAction.Menu
            };
            isFreeplayTerminable = true;
        }
        
        /// <summary> Wait until the gameplay can be terminated, ended and freeplay scene unloaded. </summary>
        public IEnumerator WaitUntilFreeplayTerminable()
        {
            yield return new WaitUntil(() => isFreeplayTerminable);
        }
        
        
        // PRIVATE METHODS
        private IEnumerator RunFreeplayRoutine(FreeplayGameType gameType, object data, Difficulty difficulty)
        {
            do
            {
                Debug.Log("RunFreeplayRoutine Freeplay");
                
                IEnumerator freeplayRoutine = gameType switch
                {
                    FreeplayGameType.Quiz => RunQuizRoutine(data as CustomQuizSettings, difficulty),
                    FreeplayGameType.Simulation => RunSimulationRoutine(data as SimulationLevelData, difficulty),
                    FreeplayGameType.Minigame => RunMinigameRoutine(data as ChallengeLevelData, difficulty),
                    _ => null
                };
                
                // Open loading screen
                LoadingScreen.Instance.ShowInfo($"Freeplay", $"{gameType} • {difficulty}");
                yield return LoadingScreen.Instance.Open();
                
                // Destroy dummy camera if existing
                if (cameraInstance)
                {
                    Destroy(cameraInstance);
                    cameraInstance = null;
                }
                
                // Wait for freeplay routine to finish
                if (freeplayRoutine != null)
                {
                    isFreeplayRoutineCompleted = false;
                    yield return freeplayRoutine;
                    isFreeplayRoutineCompleted = true;
                }
            }
            while (CompleteAction == CompleteAction.Restart);
            
            // Terminate freeplay
            isFreeplayTerminable = true;
        }
        
        private IEnumerator RunQuizRoutine(CustomQuizSettings customQuizSettings, Difficulty difficulty)
        {
            Debug.Log("RunQuizRoutine Freeplay");
            
            // Open loading screen
            yield return LoadingScreen.Instance.Open();
            
            // Load quiz scene
            var loadQuizScene = SceneManager.LoadSceneAsync(quizScene.BuildIndex, LoadSceneMode.Additive);
            yield return loadQuizScene.ReportLoadingProgress("Loading quiz scene");
            quizManager = FindAnyObjectByType<QuizManager>(FindObjectsInactive.Include);
            
            // Set quiz settings and run quiz routine
            quizManager.SetCustomQuiz(customQuizSettings, difficulty);
            quizManager.InitializeQuiz();
            quizManager.SetQuizInfo("Freeplay Quiz", customQuizSettings.topicName,
                "Custom freeplay quiz.", "Complete the quiz.");
            
            // Close loading screen
            yield return LoadingScreen.Instance.Close();
            
            // Start quiz and wait for quiz scene to finish
            yield return quizManager.WaitForQuizEnd();
            
            // Get quiz results and gameplay score
            QuizResultData resultData = quizManager.ResultData;
            
            // Unload quiz scene
            SceneManager.UnloadSceneAsync(quizScene.BuildIndex);
            
            
            // ----- FREEPLAY COMPLETE PHASE -----
            // Load dummy level complete camera.
            var instantiate = InstantiateAsync(cameraPrefab);
            yield return instantiate;
            cameraInstance = instantiate.Result[0].gameObject;
            
            // Display status in level complete/failed page
            freeplayUI.DisplayCompletePage();
            freeplayUI.DisplayCompleted("Freeplay Quiz", true);
            
            // Wait for complete action (Menu, Restart, Campaign)
            yield return freeplayUI.WaitForCompleteAction();
            CompleteAction = freeplayUI.CompleteAction == CompleteAction.Freeplay
                ? CompleteAction.Quiz
                : freeplayUI.CompleteAction;
        }
        
        private IEnumerator RunSimulationRoutine(SimulationLevelData simulationLevelData, Difficulty difficulty)
        {
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
            
            bool isLevelCompleted = true; // Simulation levels are automatically completed
            
            
            // ----- LEVEL COMPLETE PHASE -----
            // Load dummy level complete camera.
            var instantiate = InstantiateAsync(cameraPrefab);
            yield return instantiate;
            cameraInstance = instantiate.Result[0].gameObject;
            
            // Display status in level complete/failed page
            freeplayUI.DisplayCompletePage();
            freeplayUI.DisplayCompleted(simulationLevelData.Name, isLevelCompleted);
            
            // Wait for complete action (Menu, Restart, Campaign)
            yield return freeplayUI.WaitForCompleteAction();
            CompleteAction = freeplayUI.CompleteAction == CompleteAction.Freeplay
                ? CompleteAction.Simulation
                : freeplayUI.CompleteAction;
        }
        
        private IEnumerator RunMinigameRoutine(ChallengeLevelData challengeLevelData, Difficulty difficulty)
        {
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
            
            bool isLevelCompleted = minigameManager.IsCompleted;
            
            
            // ----- LEVEL COMPLETE PHASE -----
            // Load dummy level complete camera.
            var instantiate = InstantiateAsync(cameraPrefab);
            yield return instantiate;
            cameraInstance = instantiate.Result[0].gameObject;
            
            // Display status in level complete/failed page
            freeplayUI.DisplayCompletePage();
            freeplayUI.DisplayCompleted(challengeLevelData.Name, isLevelCompleted);
            
            // Wait for complete action (Menu, Restart, Campaign)
            yield return freeplayUI.WaitForCompleteAction();
            CompleteAction = freeplayUI.CompleteAction == CompleteAction.Freeplay
                ? CompleteAction.Minigame
                : freeplayUI.CompleteAction;
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
        private void FreeplayUI_OnResume() => ResumeGame();
        
        private void FreeplayUI_OnRestart() => RestartGame();
        
        private void FreeplayUI_OnQuit() => QuitGame();
        
        
        // STATIC METHODS
        private static string GetCurrentDateTime()
        {
            return DateTime.Now.ToString();
        }
    }
    
    public enum FreeplayGameType
    {
        Quiz, Simulation, Minigame
    }
}
