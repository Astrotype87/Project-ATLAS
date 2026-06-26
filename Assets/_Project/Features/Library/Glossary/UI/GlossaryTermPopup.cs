using UnityEngine;
using TMPro;

using ProjectATLAS.UI;

namespace ProjectATLAS.Library.Glossary
{
    public class GlossaryTermPopup : UIPopup
    {
        [Header("Data")]
        [SerializeField] private TermResult termResult;
        
        [Header("Components")]
        [SerializeField] private TMP_Text termText;
        [SerializeField] private TMP_Text typeText;
        [SerializeField] private TMP_Text chapterLevelText;
        [SerializeField] private TMP_Text definitionText;
        [SerializeField] private TMP_Text explanationLabel;
        [SerializeField] private TMP_Text explanationText;
        [SerializeField] private TMP_Text formulaLabel;
        [SerializeField] private TMP_Text formulaText;
        
        
        // MONOBEHAVIOUR METHODS
        private void OnValidate()
        {
            DisplayTerm(termResult);
        }
        
        
        // PUBLIC METHODS
        public void DisplayTerm(TermResult termResult)
        {
            this.termResult = termResult;
            
            string term = termResult.term;
            if (!string.IsNullOrWhiteSpace(termResult.symbol))
                term += $" ({termResult.symbol})";
            
            if (termText) termText.text = term;
            if (typeText) typeText.text = termResult.type.ToFullString();
            if (chapterLevelText) chapterLevelText.text = $"Chapter {termResult.chapter} • Level {termResult.level}";
            if (definitionText) definitionText.text = termResult.definition;
            
            // Explanation handling
            bool hasExplanation = !string.IsNullOrWhiteSpace(termResult.explanation);
            if (explanationLabel) explanationLabel.gameObject.SetActive(hasExplanation);
            if (explanationText)
            {
                explanationText.gameObject.SetActive(hasExplanation);
                if (hasExplanation) explanationText.text = termResult.explanation;
            }
            
            // Formula handling
            bool hasFormula = !string.IsNullOrWhiteSpace(termResult.formula);
            if (formulaLabel) formulaLabel.gameObject.SetActive(hasFormula);
            if (formulaText)
            {
                formulaText.gameObject.SetActive(hasFormula);
                if (hasFormula) formulaText.text = termResult.formula;
            }
        }
    }
}
