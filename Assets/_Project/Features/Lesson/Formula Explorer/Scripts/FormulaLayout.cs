using UnityEngine;

namespace ProjectATLAS.Lesson
{
    [ExecuteAlways]
    public class FormulaLayout : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float variablesWidth = 200f;
        [SerializeField] private float solutionWidth = 300f;
        
        [Header("Components")]
        [SerializeField] private RectTransform detailsTransform;
        [SerializeField] private RectTransform variablesTransform;
        [SerializeField] private RectTransform solutionTransform;
        
        private RectTransform rectTransform;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            UpdateLayout();
        }
        
        private void OnValidate()
        {
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();
            
            UpdateLayout();
        }
        
        // PRIVATE METHODS
        private void UpdateLayout()
        {
            if (rectTransform == null ||
                detailsTransform == null ||
                variablesTransform == null ||
                solutionTransform == null)
                return;
            
            float totalWidth = rectTransform.rect.width;
            float detailsWidth = Mathf.Max(0, totalWidth - variablesWidth - solutionWidth);
            
            // --- SOLUTION (Rightmost) ---
            solutionTransform.anchorMin = new Vector2(1, 0);
            solutionTransform.anchorMax = new Vector2(1, 1);
            solutionTransform.pivot = new Vector2(1, 0.5f);
            solutionTransform.sizeDelta = new Vector2(solutionWidth, 0);
            solutionTransform.anchoredPosition = Vector2.zero;
            
            // --- VARIABLES (just left of solution) ---
            variablesTransform.anchorMin = new Vector2(1, 0);
            variablesTransform.anchorMax = new Vector2(1, 1);
            variablesTransform.pivot = new Vector2(1, 0.5f);
            variablesTransform.sizeDelta = new Vector2(variablesWidth, 0);
            variablesTransform.anchoredPosition = new Vector2(-solutionWidth, 0);
            
            // --- DETAILS (fills remaining left space) ---
            detailsTransform.anchorMin = new Vector2(0, 0);
            detailsTransform.anchorMax = new Vector2(0, 1);
            detailsTransform.pivot = new Vector2(0, 0.5f);
            detailsTransform.sizeDelta = new Vector2(detailsWidth, 0);
            detailsTransform.anchoredPosition = Vector2.zero;
        }
    }
}
