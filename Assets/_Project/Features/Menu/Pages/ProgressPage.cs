using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using ProjectATLAS.GameData;
using ProjectATLAS.Gameplay;
using ProjectATLAS.UI;
using ProjectATLAS.Library.Glossary;

namespace ProjectATLAS.Menu
{
    public class ProgressPage : UIPage
    {
        [Header("Data")]
        [SerializeField, Range(0, 100)] private float gameProgress = 0.1234f;
        [SerializeField, Range(0, 50)] private float levelProgress = 0.1234f;
        [SerializeField, Range(0, 50)] private float medalProgress = 0.1234f;
        [Space]
        [SerializeField] private int currentTotalMedals = 20;
        [SerializeField] private int maxTotalMedals = 123;
        [Space]
        [SerializeField] private int currentLesson = 4;
        [SerializeField] private int totalLesson = 21;
        [SerializeField] private int currentSimulation = 10;
        [SerializeField] private int totalSimulation = 2;
        [SerializeField] private int currentChallenge = 10;
        [SerializeField] private int totalChallenge = 2;
        [SerializeField] private int currentPreTest = 3;
        [SerializeField] private int currentPostTest = 3;
        [SerializeField] private int totalTests = 10;
        [SerializeField] private int currentLevels = 1;
        [SerializeField] private int totalLevels = 41;
        [Space]
        [SerializeField] private int currentBronzeMedals = 5;
        [SerializeField] private int currentSilverMedals = 3;
        [SerializeField] private int currentGoldMedals = 2;
        [SerializeField] private int maxSingleMedals = 41;
        [Space]
        [SerializeField] private int currentGuidebooks = 4;
        [SerializeField] private int totalGuidebooks = 21;
        [Space]
        [SerializeField] private int currentGlossary = 20;
        [SerializeField] private int totalGlossary = 99;
        [SerializeField] private GlossaryData[] glossaryDatas;
        
        [Header("Components")]
        [SerializeField] private Image levelsFill;
        [SerializeField] private Image medalsFill;
        [SerializeField] private TMP_Text gameProgressText;
        [Space]
        [SerializeField] private TMP_Text preTestText;
        [SerializeField] private TMP_Text lessonText;
        [SerializeField] private TMP_Text simulationText;
        [SerializeField] private TMP_Text challengeText;
        [SerializeField] private TMP_Text postTestText;
        [SerializeField] private TMP_Text[] levelsTexts;
        [Space]
        [SerializeField] private TMP_Text[] medalsTexts;
        [SerializeField] private TMP_Text bronzeMedalsText;
        [SerializeField] private TMP_Text silverMedalsText;
        [SerializeField] private TMP_Text goldMedalsText;
        [Space]
        [SerializeField] private TMP_Text guidebooksText;
        [SerializeField] private TMP_Text glossaryText;
        
        private GameDataManager gameDataManager;
        private LevelManager levelManager;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            gameDataManager = GameDataManager.Instance;
            levelManager = LevelManager.Instance;
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();
            
            DisplayGameProgress(gameProgress, levelProgress, medalProgress);
            
            DisplayLesson(currentLesson, totalLesson);
            DisplaySimulation(currentSimulation, totalSimulation);
            DisplayChallenge(currentChallenge, totalChallenge);
            DisplayTests(currentPreTest, currentPostTest, totalTests);
            DisplayLevels(currentLevels, totalLevels);
            
            DisplayMedals(currentTotalMedals, maxTotalMedals);
            DisplayBronzeSilverGoldMedals(currentBronzeMedals, currentSilverMedals, currentGoldMedals, maxSingleMedals);
            
            DisplayGuidebooks(currentGuidebooks, totalGuidebooks);
            DisplayGlossary(currentGlossary, totalGlossary);
        }
        
        
        // UIPage METHODS
        public override void OpenPage()
        {
            base.OpenPage();
            UpdateProgressUI();
        }
        
        
        // PUBLIC METHODS
        public void UpdateProgressUI()
        {
            // Ensure services are ready
            if (gameDataManager) gameDataManager = GameDataManager.Instance;
            if (!gameDataManager) return;
            
            if (levelManager) levelManager = LevelManager.Instance;
            if (!levelManager) return;
            
            // Get pre-tests, levels, lessons, simulations, challenges, and post-tests counts
            (currentLesson, totalLesson) = levelManager.GetCompletedAndTotalLessonLevels();
            (currentSimulation, totalSimulation) = levelManager.GetCompletedAndTotalSimulationLevels();
            (currentChallenge, totalChallenge) = levelManager.GetCompletedAndTotalChallengeLevels();
            (currentPreTest, totalTests) = levelManager.GetCompletedAndTotalPreTests();
            (currentPostTest, totalTests) = levelManager.GetCompletedAndTotalPostTests();
            
            // Get total level count
            currentLevels = currentLesson + currentSimulation + currentChallenge + currentPreTest + currentPostTest;
            totalLevels = totalLesson + totalSimulation + totalChallenge + totalTests + totalTests;
            
            // Get medal counts
            (currentTotalMedals, maxTotalMedals) = levelManager.GetObtainedAndTotalMedals();
            currentBronzeMedals = gameDataManager.CampaignData.levels.Count(l => l.isBronze);
            currentSilverMedals = gameDataManager.CampaignData.levels.Count(l => l.isSilver);
            currentGoldMedals = gameDataManager.CampaignData.levels.Count(l => l.isGold);
            
            
            // Get game progress
            gameProgress = levelManager ? levelManager.GetCampaignProgress() : 0f;
            levelProgress = currentLevels / (float)totalLevels * 100f;
            medalProgress = currentTotalMedals / (float)maxTotalMedals * 100f;
            
            
            // Get guidebooks and glossary counts
            (currentGuidebooks, totalGuidebooks) = (currentLesson, totalLesson);
            (currentGlossary, totalGlossary) = (CountUnlockedGlossaryTerms(currentLevels), CountTotalGlossaryTerms());
            
            
            // Display all data to UI
            DisplayGameProgress(gameProgress, levelProgress, medalProgress);
            
            DisplayLesson(currentLesson, totalLesson);
            DisplaySimulation(currentSimulation, totalSimulation);
            DisplayChallenge(currentChallenge, totalChallenge);
            DisplayTests(currentPreTest, currentPostTest, totalTests);
            DisplayLevels(currentLevels, totalLevels);
            
            DisplayMedals(currentTotalMedals, maxTotalMedals);
            DisplayBronzeSilverGoldMedals(currentBronzeMedals, currentSilverMedals, currentGoldMedals, maxSingleMedals);
            
            DisplayGuidebooks(currentGuidebooks, totalGuidebooks);
            DisplayGlossary(currentGlossary, totalGlossary);
        }
        
        
        public void DisplayGameProgress(float gameProgress, float levelProgress, float medalProgress)
        {
            this.gameProgress = gameProgress;
            this.levelProgress = levelProgress;
            this.medalProgress = medalProgress;
            
            if (levelsFill) levelsFill.fillAmount = levelProgress / 100 / 2;
            if (medalsFill)
            {
                medalsFill.fillAmount = medalProgress / 100 / 2;
                // Rotate by fill amount of levelsFill to start from the end of levelsFill
                medalsFill.transform.rotation = Quaternion.Euler(0, 0, -levelsFill.fillAmount * 360);
            }
            
            if (gameProgressText) gameProgressText.text = $"{gameProgress:F2}%";
        }
        
        
        public void DisplayLesson(int currentLesson, int totalLesson)
        {
            this.currentLesson = currentLesson;
            this.totalLesson = totalLesson;
            
            if (lessonText) lessonText.text = $"{currentLesson}/{totalLesson}";
        }
        
        public void DisplaySimulation(int currentSimulation, int totalSimulation)
        {
            this.currentSimulation = currentSimulation;
            this.totalSimulation = totalSimulation;
            
            if (simulationText) simulationText.text = $"{currentSimulation}/{totalSimulation}";
        }
        
        public void DisplayChallenge(int currentChallenge, int totalChallenge)
        {
            this.currentChallenge = currentChallenge;
            this.totalChallenge = totalChallenge;
            
            if (challengeText) challengeText.text = $"{currentChallenge}/{totalChallenge}";
        }
        
        public void DisplayTests(int currentPreTest, int currentPostTest, int totalTests)
        {
            this.currentPreTest = currentPreTest;
            this.currentPostTest = currentPostTest;
            this.totalTests = totalTests;
            
            if (preTestText) preTestText.text = $"{currentPreTest}/{totalTests}";
            if (postTestText) postTestText.text = $"{currentPostTest}/{totalTests}";
        }
        
        public void DisplayLevels(int currentLevels, int totalLevels)
        {
            this.currentLevels = currentLevels;
            this.totalLevels = totalLevels;
            
            foreach (var text in levelsTexts)
            {
                if (text) text.text = $"{currentLevels}/{totalLevels}";
            }
        }
        
        
        public void DisplayMedals(int currentTotalMedals, int maxTotalMedals)
        {
            this.currentTotalMedals = currentTotalMedals;
            this.maxTotalMedals = maxTotalMedals;
            
            foreach (var text in medalsTexts)
            {
                if (text) text.text = $"{currentTotalMedals}/{maxTotalMedals}";
            }
        }
        
        public void DisplayBronzeSilverGoldMedals(
            int currentBronzeMedals, int currentSilverMedals,
            int currentGoldMedals, int maxSingleMedals)
        {
            this.currentBronzeMedals = currentBronzeMedals;
            this.currentSilverMedals = currentSilverMedals;
            this.currentGoldMedals = currentGoldMedals;
            this.maxSingleMedals = maxSingleMedals;
            
            if (bronzeMedalsText) bronzeMedalsText.text = $"{currentBronzeMedals}/{maxSingleMedals}";
            if (silverMedalsText) silverMedalsText.text = $"{currentSilverMedals}/{maxSingleMedals}";
            if (goldMedalsText) goldMedalsText.text = $"{currentGoldMedals}/{maxSingleMedals}";
        }
        
        
        public void DisplayGuidebooks(int currentGuidebooks, int totalGuidebooks)
        {
            this.currentGuidebooks = currentGuidebooks;
            this.totalGuidebooks = totalGuidebooks;
            
            if (guidebooksText) guidebooksText.text = $"{currentGuidebooks}/{totalGuidebooks}";
        }
        
        public void DisplayGlossary(int currentGlossary, int totalGlossary)
        {
            this.currentGlossary = currentGlossary;
            this.totalGlossary = totalGlossary;
            
            if (glossaryText) glossaryText.text = $"{currentGlossary}/{totalGlossary}";
        }
        
        
        
        // PRIVATE METHODS
        private int CountUnlockedGlossaryTerms(int level)
        {
            if (glossaryDatas == null || glossaryDatas.Length == 0)
                return 0;
            
            int totalCount = 0;
            
            foreach (var glossary in glossaryDatas)
            {
                if (glossary == null || glossary.levelTerms == null)
                    continue;
                
                foreach (var levelTerms in glossary.levelTerms)
                {
                    if (levelTerms == null || levelTerms.terms == null)
                        continue;
                    
                    // Count terms from all levels <= the input level
                    if (levelTerms.level <= level)
                        totalCount += levelTerms.terms.Length;
                }
            }
            
            return totalCount;
        }
        
        private int CountTotalGlossaryTerms()
        {
            if (glossaryDatas == null || glossaryDatas.Length == 0)
                return 0;
            
            int total = 0;
            
            foreach (var glossary in glossaryDatas)
            {
                if (glossary == null || glossary.levelTerms == null)
                    continue;
                
                foreach (var levelTerm in glossary.levelTerms)
                {
                    if (levelTerm == null || levelTerm.terms == null)
                        continue;
                    
                    total += levelTerm.terms.Length;
                }
            }
        
            return total;
        }
        
    }
}
