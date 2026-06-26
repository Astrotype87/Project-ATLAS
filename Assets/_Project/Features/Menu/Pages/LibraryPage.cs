using UnityEngine;
using TMPro;

using ProjectATLAS.GameData;
using ProjectATLAS.Gameplay;
using ProjectATLAS.UI;
using ProjectATLAS.Library.Glossary;

namespace ProjectATLAS.Menu
{
    public class LibraryPage : UIPage
    {
        [Header("Components")]
        [SerializeField] private TMP_Text guidebooksText;
        [SerializeField] private TMP_Text glossaryText;
        
        [Header("Data")]
        [SerializeField] private GlossaryData[] glossaryDatas;
        
        private LevelManager levelManager;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            levelManager = LevelManager.Instance;
        }
        
        
        // UIPage METHODS
        public override void OpenPage()
        {
            base.OpenPage();
            UpdateTexts();
        }
        
        
        // PRIVATE METHODS
        private void UpdateTexts()
        {
            if (levelManager) levelManager = LevelManager.Instance;
            if (!levelManager) return;
            
            // Get guidebooks and glossary counts
            (int currentLevels, int totalLevels) = GetCompletedAndTotalCampaignLevels();
            (int currentGuidebooks, int totalGuidebooks) = levelManager.GetCompletedAndTotalLessonLevels();
            (int currentGlossary, int totalGlossary) = (CountUnlockedGlossaryTerms(currentLevels), CountTotalGlossaryTerms());
            
            // Update texts
            guidebooksText.text = $"Guidebooks\n({currentGuidebooks}/{totalGuidebooks})";
            glossaryText.text = $"Glossary\n({currentGlossary}/{totalGlossary})";
        }
        
        private (int completed, int total) GetCompletedAndTotalCampaignLevels()
        {
            // Get pre-tests, levels, lessons, simulations, challenges, and post-tests counts
            (int currentLesson, int totalLesson) = levelManager.GetCompletedAndTotalLessonLevels();
            (int currentSimulation, int totalSimulation) = levelManager.GetCompletedAndTotalSimulationLevels();
            (int currentChallenge, int totalChallenge) = levelManager.GetCompletedAndTotalChallengeLevels();
            (int currentPreTest, int totalPreTests) = levelManager.GetCompletedAndTotalPreTests();
            (int currentPostTest, int totalPostTests) = levelManager.GetCompletedAndTotalPostTests();
            
            // Get total level count
            int completed = currentLesson + currentSimulation + currentChallenge + currentPreTest + currentPostTest;
            int total = totalLesson + totalSimulation + totalChallenge + totalPreTests + totalPostTests;
            
            return (completed, total);
        }
        
        
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
