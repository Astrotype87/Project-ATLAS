using TMPro;
using UnityEngine;

namespace ProjectATLAS.Quiz.UI
{
    public class QuizProgressView : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private int total;
        [SerializeField] private int current;
        
        [Header("Components")]
        [SerializeField] private TMP_Text itemText;
        [SerializeField] private RectTransform progressFill;
        
        
        // MONOBEHAVIOUR METHODS
        private void OnValidate()
        {
            UpdateProgressDisplay(current, total);
        }
        
        // PUBLIC METHODS
        public void UpdateProgressDisplay(int current, int total)
        {
            // Update data
            this.total = Mathf.Max(total, 1);
            this.current = Mathf.Clamp(current, 0, total);
            
            // Set text display
            if (itemText) itemText.text = $"Item {current} of {total}";
            
            // Set progress bar display
            if (progressFill)
            {
                float progress = Mathf.Clamp01((float)current / total);
                if (float.IsNaN(progress) || float.IsInfinity(progress))
                    progress = 0;
                
                Vector2 anchorMax = progressFill.anchorMax;
                anchorMax.x = progress;
                progressFill.anchorMax = anchorMax;
            }
        }
    }
}
