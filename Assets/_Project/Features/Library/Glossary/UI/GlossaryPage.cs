using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using ProjectATLAS.UI;
using ProjectATLAS.Gameplay;
using ProjectATLAS.Cheats;

namespace ProjectATLAS.Library.Glossary
{
    public class GlossaryPage : UIPage
    {
        [Header("Data")]
        [SerializeField] private GlossaryData[] glossaryDatas;
        [SerializeField] private int pageNumber = 1;
        [SerializeField] private int itemsPerPage = 20;
        [SerializeField] private int chapterFilter = 1;
        [SerializeField] private bool sortAlphabetically = false;
        
        [Header("Buttons")]
        [SerializeField] private Button lastButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private UIToggleButton[] chapterButtons;
        [SerializeField] private UIToggleButton normalSortButton;
        [SerializeField] private UIToggleButton aToZSortButton;
        
        [Header("Panel & Text")]
        [SerializeField] private GlossaryTermsPanel glossaryTermsPanel;
        [SerializeField] private GlossaryTermPopup glossaryTermPopup;
        [SerializeField] private TMP_Text labelText;
        [SerializeField] private TMP_Text pageText;
        
        private TermResult[] termResults;
        private LevelManager levelManager;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            lastButton.onClick.AddListener(LastButton_onClick);
            nextButton.onClick.AddListener(NextButton_onClick);
            
            for (int i = 0; i < chapterButtons.Length; i++)
            {
                chapterButtons[i].OnValueChanged += ChapterButtons_OnValueChanged;
            }
            
            normalSortButton.OnValueChanged += NormalSortButton_OnValueChanged;
            aToZSortButton.OnValueChanged += AToZSortButton_OnValueChanged;
            
            glossaryTermsPanel.OnTermClicked += GlossaryTermsPanel_OnTermClicked;
            
            levelManager = LevelManager.Instance;
        }
        
        
        // UIPage METHODS
        public override void OpenPage()
        {
            base.OpenPage();
            Debug.Log($"OpenPage() > ReloadGlossaryTermResults()");
            
            // Update filter and sort buttons
            chapterButtons[chapterFilter].SetValue(true);
            
            if (sortAlphabetically) aToZSortButton.SetValue(true);
            else normalSortButton.SetValue(true);
            
            // Reload glossary term results
            ReloadGlossaryTermResults();
        }
        
        
        // PUBLIC METHODS
        public void ReloadGlossaryTermResults()
        {
            // Filter by chapter, sort by level/alphabetically with locked status
            if (levelManager == null) levelManager = LevelManager.Instance;
            int lastCompletedLevel = levelManager.LastCompletedLevel();
            
            if (CheatsManager.UnlockAllGlossary) lastCompletedLevel = 10000;
            termResults = GetGlossaryTermResults(glossaryDatas, chapterFilter, sortAlphabetically, lastCompletedLevel);
            
            // Update chapter text
            int total = termResults.Length;
            int unlocked = termResults.Count(t => !t.isLocked);
            labelText.text = (chapterFilter <= 0 ? "All Chapters" : $"Chapter {chapterFilter}") + $" ({unlocked}/{total})";
            
            // Reset page and display results
            pageNumber = 1;
            DisplayPageResults(pageNumber);
        }
        
        public void DisplayPageResults(int pageNumber)
        {
            // Get total pages and clamp page number
            int totalPages = Mathf.CeilToInt(termResults.Length / (float)itemsPerPage);
            this.pageNumber = pageNumber = Mathf.Clamp(pageNumber, 1, totalPages);
            
            // Get term results per page
            TermResult[] displayedTermResults = GetTermResultPagination(termResults, pageNumber, itemsPerPage);
            
            // Update page text
            pageText.text = $"Page {pageNumber}/{totalPages}";
            
            // Disable button when first or last page
            lastButton.interactable = pageNumber > 1;
            nextButton.interactable = pageNumber < totalPages;
            
            // Display results
            glossaryTermsPanel.DisplayTermResults(displayedTermResults);
        }
        
        
        // EVENT LISTENER METHODS
        private void ChapterButtons_OnValueChanged(UIToggleButton toggleButton, bool value)
        {
            for (int i = 0; i < chapterButtons.Length; i++)
            {
                if (chapterButtons[i] == toggleButton)
                {
                    chapterFilter = i;
                    ReloadGlossaryTermResults();
                    break;
                }
            }
        }
        
        private void NormalSortButton_OnValueChanged(UIToggleButton toggleButton, bool value)
        {
            sortAlphabetically = false;
            ReloadGlossaryTermResults();
        }
        
        private void AToZSortButton_OnValueChanged(UIToggleButton toggleButton, bool value)
        {
            sortAlphabetically = true;
            ReloadGlossaryTermResults();
        }
        
        
        private void GlossaryTermsPanel_OnTermClicked(TermResult termResult)
        {
            // Open glossary popup
            glossaryTermPopup.DisplayTerm(termResult);
            glossaryTermPopup.OpenPopup();
        }
        
        
        private void LastButton_onClick()
        {
            DisplayPageResults(pageNumber - 1);
        }
        
        private void NextButton_onClick()
        {
            DisplayPageResults(pageNumber + 1);
        }
        
        
        
        // PRIVATE STATIC METHODS
        public static TermResult[] GetGlossaryTermResults(GlossaryData[] glossaryDatas, int chapterFilter, bool sortAlphabetically, int lastCompletedLevel)
        {
            // 1. Filter chapters
            IEnumerable<GlossaryData> filteredChapters = chapterFilter <= 0
                ? glossaryDatas
                : glossaryDatas.Where(c => c.chapter == chapterFilter);
            
            // 2. Flatten into TermResult[]
            var termResults = filteredChapters
                .SelectMany(
                    c => c.levelTerms,
                    (chapter, level) => new { chapter.chapter, level.level, level.terms })
                .SelectMany(
                    cl => cl.terms,
                    (cl, term) =>
                    {
                        // Example lock logic: lock if level is greater than lastCompletedLevel
                        bool isLocked = cl.level > lastCompletedLevel;
                        return new TermResult(isLocked, cl.chapter, cl.level, term);
                    })
                .ToList();
            
            // 3. Sort
            if (sortAlphabetically)
            {
                // Split into unlocked and locked
                var unlocked = termResults
                    .Where(t => !t.isLocked)
                    .OrderBy(t => t.term, StringComparer.OrdinalIgnoreCase);
                
                var locked = termResults
                    .Where(t => t.isLocked); // keep natural order
                
                return unlocked.Concat(locked).ToArray();
            }
            
            // Natural order
            return termResults.ToArray();
        }
        
        
        public static TermResult[] GetTermResultPagination(TermResult[] termResults, int pageNumber, int itemsPerPage)
        {
            if (pageNumber < 1 || itemsPerPage < 1)
                return new TermResult[0];
            
            return termResults
                .Skip((pageNumber - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToArray();
        }
        
    }
    
    [Serializable]
    public struct TermResult
    {
        public bool isLocked;
        
        public int chapter;
        public int level;
        public string term;
        public string symbol;
        public TermType type;
        [TextArea(2, 5)] public string definition;
        [TextArea(4, 8)] public string explanation;
        [TextArea(4, 8)] public string formula;
        
        public TermResult(bool isLocked, int chapter, int level, GlossaryTerm glossaryTerm)
        {
            this.isLocked = isLocked;
            this.chapter = chapter;
            this.level = level;
            this.term = glossaryTerm.term;
            this.symbol = glossaryTerm.symbol;
            this.type = glossaryTerm.type;
            this.definition = glossaryTerm.definition;
            this.explanation = glossaryTerm.explanation;
            this.formula = glossaryTerm.formula;
        }
        
        public TermResult(bool isLocked, int chapter, int level,
            string term, string unit, TermType type,
            string description, string explanation, string formula)
        {
            this.isLocked = isLocked;
            this.chapter = chapter;
            this.level = level;
            this.term = term;
            this.symbol = unit;
            this.type = type;
            this.definition = description;
            this.explanation = explanation;
            this.formula = formula;
        }
    }
    
}
