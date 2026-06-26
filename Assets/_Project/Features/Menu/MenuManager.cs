using System;
using UnityEngine;

using ProjectATLAS.Gameplay;
using ProjectATLAS.UI;
using ProjectATLAS.Menu.Background;
using ProjectATLAS.Quiz;
using ProjectATLAS.Quiz.UI;

namespace ProjectATLAS.Menu
{
    public class MenuManager : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] private Header header;
        
        [Header("Pages")]
        [SerializeField] private WelcomePage welcomePage;
        [SerializeField] private UIPage menuPage;
        [SerializeField] private CampaignPage campaignPage;
        [SerializeField] private LevelPage levelPage;
        [SerializeField] private TestDetailsPage testPage;
        [SerializeField] private LevelPanel levelPanel;
        
        [SerializeField] private UIPage freeplayPage;
        [SerializeField] private SelectQuizPage selectQuizPage;
        [SerializeField] private SelectSimulationPage selectSimulationPage;
        [SerializeField] private SelectMinigamePage selectMinigamePage;
        
        [Header("Background")]
        [SerializeField] private ParallaxBackground parallaxBackground;
        
        
        // PROPERTIES
        public event Action<CampaignLevelData, Difficulty> OnPlayLevel;
        public event Action<CustomQuizSettings, Difficulty> OnPlayQuiz;
        public event Action<SimulationLevelData, Difficulty> OnPlaySimulation;
        public event Action<ChallengeLevelData, Difficulty> OnPlayMinigame;
        
        
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            levelPanel.OnPlayLevelClicked += LevelPanel_OnPlayLevelClicked;
            // levelPage.OnPlayLevelClicked += LevelPage_OnPlayLevelClicked;
            // testPage.OnPlayTestClicked += TestPage_OnPlayTestClicked;
            
            selectQuizPage.OnStartClicked += SelectQuizPage_OnStartClicked;
            selectSimulationPage.OnStartClicked += SelectSimulationPage_OnStartClicked;
            selectMinigamePage.OnStartClicked += SelectMinigamePage_OnStartClicked;
        }
        
        
        // PUBLIC METHODS
        public void PreOpenWelcomePage()
        {
            welcomePage.OpenPageInGroup();
        }
        
        public void PreOpenMenuPage()
        {
            menuPage.OpenPageInGroup();
        }
        
        public void PreOpenCampaignPage()
        {
            menuPage.OpenPageInGroup();
            campaignPage.OpenPageInGroup();
        }
        
        public void PreOpenQuizPage()
        {
            menuPage.OpenPageInGroup();
            freeplayPage.OpenPageInGroup();
            selectQuizPage.OpenPageInGroup();
        }
        
        public void PreOpenSimulationPage()
        {
            menuPage.OpenPageInGroup();
            freeplayPage.OpenPageInGroup();
            selectSimulationPage.OpenPageInGroup();
        }
        
        public void PreOpenMinigamePage()
        {
            menuPage.OpenPageInGroup();
            freeplayPage.OpenPageInGroup();
            selectMinigamePage.OpenPageInGroup();
        }
        
        
        
        // PRIVATE METHODS
        // private void FindPagesAndSubscribe()
        // {
        //     // Find campaign page and subscribe to events
        //     var foundCampaignPage = FindAnyObjectByType<CampaignPage>(FindObjectsInactive.Include);
        //     campaignPage = foundCampaignPage;
            
        //     // Find level page and subscribe to events
        //     var foundLevelPage = FindAnyObjectByType<LevelPage>(FindObjectsInactive.Include);
        //     if (foundLevelPage != levelPage)
        //     {
        //         if (levelPage) levelPage.OnPlayClicked -= LevelPage_OnPlayClicked;
        //         if (foundLevelPage) foundLevelPage.OnPlayClicked += LevelPage_OnPlayClicked;
        //     }
        //     levelPage = foundLevelPage;
        // }
        
        
        // EVENT LISTENER METHODS
        // private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        // {
        //     FindPagesAndSubscribe();
        // }
        
        private void LevelPanel_OnPlayLevelClicked(CampaignLevelData levelData, Difficulty difficulty)
        {
            OnPlayLevel?.Invoke(levelData, difficulty);
        }
        
        // private void LevelPage_OnPlayLevelClicked(LevelData levelData, Difficulty difficulty)
        // {
        //     OnPlayLevel?.Invoke(levelData, difficulty);
        // }
        
        // private void TestPage_OnPlayTestClicked(TestData testData)
        // {
        //     OnPlayLevel?.Invoke(testData, Difficulty.Easy);
        // }
        
        private void SelectQuizPage_OnStartClicked(CustomQuizSettings customQuizSettings, Difficulty difficulty)
        {
            Debug.Log($"SelectQuizPage_OnStartClicked = {customQuizSettings.topicName}");
            OnPlayQuiz?.Invoke(customQuizSettings, difficulty);
        }
        
        private void SelectSimulationPage_OnStartClicked(SimulationLevelData simulationLevelData, Difficulty difficulty)
        {
            OnPlaySimulation?.Invoke(simulationLevelData, difficulty);
        }
        
        private void SelectMinigamePage_OnStartClicked(ChallengeLevelData challengeLevelData, Difficulty difficulty)
        {
            OnPlayMinigame?.Invoke(challengeLevelData, difficulty);
        }
    }
}
