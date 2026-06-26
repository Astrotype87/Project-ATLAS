using TMPro;
using UnityEngine;

namespace ProjectATLAS.Menu
{
    public class BestLevelScoreView : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private int number;
        [SerializeField] private float score;
        [SerializeField] private bool disabled;
        
        [Header("Components")]
        [SerializeField] private TMP_Text numberText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private CanvasGroup canvasGroup;
        
        
        private void OnValidate()
        {
            DisplayScore(number, score);
            SetDisable(disabled);
        }
        
        public void DisplayScore(int number, float score)
        {
            gameObject.name = $"Score {number}";
            
            this.number = number;
            this.score = score;
            
            if (numberText) numberText.text = $"{number:00}";
            if (scoreText) scoreText.text = $"{score:N0}";
        }
        
        public void SetDisable(bool disabled)
        {
            this.disabled = disabled;
            if (canvasGroup) canvasGroup.alpha = disabled ? 0.5f : 1.0f;
        }
    }
}
