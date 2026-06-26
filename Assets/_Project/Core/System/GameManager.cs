using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Eflatun.SceneReference;

using ProjectATLAS.Architecture;
using ProjectATLAS.Menu;
using ProjectATLAS.Gameplay;
using ProjectATLAS.GameData;
using ProjectATLAS.Authentication;
using ProjectATLAS.Quiz;
using ProjectATLAS.Freeplay;

namespace ProjectATLAS.System
{
    public class GameManager : PersistentSingletonGameObject<GameManager>
    {
        [Header("Scenes")]
        [SerializeField] private SceneReference introScene;
        [SerializeField] private SceneReference menuScene;
        [SerializeField] private SceneReference gameplayScene;
        [SerializeField] private SceneReference freeplayScene;
        
        [Header("Loading")]
        [SerializeField] private GameObject loadingPrefab;
        [SerializeField] private LoadingScreen loadingScreen;
        
        [Header("Managers")]
        [SerializeField] private MenuManager menuManager;
        [SerializeField] private GameplayManager gameplayManager;
        [SerializeField] private FreeplayManager freeplayManager;
        
        [Header("Services")]
        [SerializeField] private GameDataManager gameDataManager;
        [SerializeField] private AuthenticationManager authenticationManager;
        
        
        // MONOBEHAVIOUR METHODS
        protected override void Awake()
        {
            base.Awake();
        }
        
        private void Start()
        {
            StartCoroutine(Initialize());
        }
        
        
        // EVENT LISTENER METHODS
        private void MenuManager_OnPlayLevel(CampaignLevelData levelData, Difficulty difficulty)
        {
            StartCoroutine(StartGameplayRoutine(levelData, difficulty));
        }
        
        private void MenuManager_OnPlayQuiz(CustomQuizSettings customQuizSettings, Difficulty difficulty)
        {
            StartCoroutine(StartFreeplayRoutine(FreeplayGameType.Quiz, customQuizSettings, difficulty));
        }
        
        private void MenuManager_OnPlaySimulation(SimulationLevelData simulationLevelData, Difficulty difficulty)
        {
            StartCoroutine(StartFreeplayRoutine(FreeplayGameType.Simulation, simulationLevelData, difficulty));
        }
        
        private void MenuManager_OnPlayMinigame(ChallengeLevelData challengeLevelData, Difficulty difficulty)
        {
            StartCoroutine(StartFreeplayRoutine(FreeplayGameType.Minigame, challengeLevelData, difficulty));
        }
        
        
        // PRIVATE METHODS
        private IEnumerator Initialize()
        {
            // Instantiate loading screen prefab
            yield return LoadLoadingScreen();
            
            // Get list of loaded scene indexes
            HashSet<int> loadedScenes = new();
            for (int i = 0; i < SceneManager.loadedSceneCount; i++)
            {
                loadedScenes.Add(SceneManager.GetSceneAt(i).buildIndex);
            }
            
            // Check which scenes are loaded on play
            if (loadedScenes.Contains(introScene.BuildIndex))
            {
                // If INTRO scene is loaded, load menu scene with loading screen
                
                LoadingScreen.Instance.OpenUnanimated();
                yield return gameDataManager.LoadDataAsync(); // LOAD GAME DATA
                yield return authenticationManager.InitializeAsync();
                
                yield return LoadMenuScene();
                yield return LoadingScreen.Instance.Close();
            }
            else if (loadedScenes.Contains(menuScene.BuildIndex))
            {
                // If MENU scene is loaded, get reference to menuManager and listen to its event
                
                yield return gameDataManager.LoadDataAsync(); // LOAD GAME DATA
                yield return authenticationManager.InitializeAsync();
                
                menuManager = FindAnyObjectByType<MenuManager>(FindObjectsInactive.Include);
                menuManager.OnPlayLevel += MenuManager_OnPlayLevel;
                menuManager.OnPlayQuiz += MenuManager_OnPlayQuiz;
                menuManager.OnPlaySimulation += MenuManager_OnPlaySimulation;
                menuManager.OnPlayMinigame += MenuManager_OnPlayMinigame;
                
                // if first time playing, display welcome page
                if (!gameDataManager.DetailsData.completedFirstTime)
                {
                    menuManager.PreOpenWelcomePage();
                }
            }
            else
            {
                yield return gameDataManager.LoadDataAsync(); // LOAD GAME DATA
                
                yield return authenticationManager.InitializeAsync();
            }
        }
        
        private IEnumerator LoadLoadingScreen()
        {
            var instantiateLoadingScreen = InstantiateAsync(loadingPrefab);
            yield return instantiateLoadingScreen;
            
            GameObject loadingScreen = instantiateLoadingScreen.Result[0];
            loadingScreen.name = "Loading UI";
        }
        
        private IEnumerator StartGameplayRoutine(CampaignLevelData campaignLevelData, Difficulty difficulty)
        {
            // Open loading screen
            LoadingScreen.Instance.ShowInfo(campaignLevelData.Name, $"{campaignLevelData.Title} • {campaignLevelData.Type} Type • {difficulty}");
            yield return LoadingScreen.Instance.Open();
            
            // Load gameplay scene
            var loadGameplayScene = SceneManager.LoadSceneAsync(gameplayScene.BuildIndex);
            yield return loadGameplayScene.ReportLoadingProgress("Loading gameplay scene", 0.0f, 0.3333f);
            
            // Run gameplay routine (loading screen will be closed inside gameplayManager.RunGameplayRoutine())
            gameplayManager = FindAnyObjectByType<GameplayManager>(FindObjectsInactive.Include);
            gameplayManager.StartGame(campaignLevelData, difficulty);
            yield return gameplayManager.WaitUntilGameplayTerminable();
            var completeAction = gameplayManager.CompleteAction;
            
            
            // Open loading screen
            yield return LoadingScreen.Instance.Open();
            
            // Load menu scene and open page
            yield return LoadMenuScene();
            if (completeAction == CompleteAction.Campaign) menuManager.PreOpenCampaignPage();
            else if (completeAction == CompleteAction.Menu) menuManager.PreOpenMenuPage();
            
            // Close loading screen
            yield return LoadingScreen.Instance.Close();
        }
        
        private IEnumerator StartFreeplayRoutine(FreeplayGameType gameType, object data, Difficulty difficulty)
        {
            // Open loading screen
            LoadingScreen.Instance.ShowInfo($"Freeplay", $"{gameType} • {difficulty}");
            yield return LoadingScreen.Instance.Open();
            
            // Load freeplay scene
            var loadFreeplayScene = SceneManager.LoadSceneAsync(freeplayScene.BuildIndex);
            yield return loadFreeplayScene.ReportLoadingProgress("Loading freeplay scene", 0.0f, 0.3333f);
            
            // Run freeplay routine (loading screen will be closed inside freeplayManager.RunFreeplayRoutine())
            freeplayManager = FindAnyObjectByType<FreeplayManager>(FindObjectsInactive.Include);
            freeplayManager.StartGame(gameType, data, difficulty);
            yield return freeplayManager.WaitUntilFreeplayTerminable();
            var completeAction = freeplayManager.CompleteAction;
            
            
            // Open loading screen
            yield return LoadingScreen.Instance.Open();
            
            // Load menu scene and open page
            yield return LoadMenuScene();
            if (completeAction == CompleteAction.Quiz) menuManager.PreOpenQuizPage();
            if (completeAction == CompleteAction.Simulation) menuManager.PreOpenSimulationPage();
            if (completeAction == CompleteAction.Minigame) menuManager.PreOpenMinigamePage();
            else if (completeAction == CompleteAction.Menu) menuManager.PreOpenMenuPage();
            
            // Close loading screen
            yield return LoadingScreen.Instance.Close();
        }
        
        private IEnumerator LoadMenuScene()
        {
            var loadMenuScene = SceneManager.LoadSceneAsync(menuScene.BuildIndex);
            yield return loadMenuScene.ReportLoadingProgress("Loading menu scene");
            
            menuManager = FindAnyObjectByType<MenuManager>(FindObjectsInactive.Include);
            menuManager.OnPlayLevel += MenuManager_OnPlayLevel;
            menuManager.OnPlayQuiz += MenuManager_OnPlayQuiz;
            menuManager.OnPlaySimulation += MenuManager_OnPlaySimulation;
            menuManager.OnPlayMinigame += MenuManager_OnPlayMinigame;
            
            
            Debug.Log($"gameDataService.DetailsData.completedFirstTime: {gameDataManager.DetailsData.completedFirstTime}");
            
            // if first time playing, display welcome page
            if (!gameDataManager.DetailsData.completedFirstTime)
            {
                gameDataManager.DetailsData.completedFirstTime = true;
                
                menuManager.PreOpenWelcomePage();
            }
        }
    }
}
