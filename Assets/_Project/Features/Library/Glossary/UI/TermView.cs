using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectATLAS.Library.Glossary
{
    public class TermView : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private TermResult termResult;
        
        [Header("Components")]
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text termText;
        [SerializeField] private TMP_Text typeText;
        [SerializeField] private TMP_Text definitionText;
        [SerializeField] private GameObject[] lockedDisplay;
        [SerializeField] private TMP_Text lockedText;
        [SerializeField] private CanvasGroup canvasGroup;
        
        public event Action<TermResult> OnClicked;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            button.onClick.AddListener(Button_onClick);
        }
        
        private void OnValidate()
        {
            DisplayTerm(termResult);
        }
        
        
        // PUBLIC METHODS
        public void DisplayTerm(TermResult termResult)
        {
            this.termResult = termResult;
            
            if (button) button.interactable = !termResult.isLocked;
            
            foreach (var go in lockedDisplay)
            {
                if (go) go.SetActive(termResult.isLocked);
                if (termResult.isLocked && lockedText) lockedText.text = $"Complete Level {termResult.level} to unlock";
            }
            
            if (termResult.isLocked)
            {
                if (termText) termText.text = "";
                if (typeText) typeText.text = "";
                if (definitionText) definitionText.text = "";
            }
            else
            {
                string unitText = string.IsNullOrWhiteSpace(termResult.symbol) ? "" : $" ({termResult.symbol})";
                
                if (termText) termText.text = termResult.term.ToUpper() + unitText;
                if (typeText) typeText.text = termResult.type.ToShortString();
                if (definitionText) definitionText.text = termResult.definition;
            }
        }
        
        public void SetVisible(bool isVisible)
        {
            canvasGroup.alpha = isVisible ? 1f : 0f;
            canvasGroup.interactable = isVisible;
            canvasGroup.blocksRaycasts = isVisible;
        }
        
        
        // EVENT LISTENER METHODS
        private void Button_onClick()
        {
            OnClicked?.Invoke(termResult);
        }
    }
}
