using TMPro;
using UnityEngine;

namespace ProjectATLAS
{
    public class TimeBonusRow : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private float time;
        [SerializeField] private int score;
        [SerializeField] private bool lowest;
        
        [Header("Components")]
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private TMP_Text scoreText;
        
        private void OnValidate()
        {
            if (lowest) SetTimeBonusLowest(score);
            else SetTimeBonus(time, score);
        }
        
        public void SetTimeBonus(float time, int score)
        {
            this.time = time;
            this.score = score;
            this.lowest = false;
            
            if (timeText) timeText.text = $"{time} s";
            if (scoreText) scoreText.text = $"{score}";
        }
        
        public void SetTimeBonusLowest(int score)
        {
            this.lowest = true;
            this.score = score;
            
            if (timeText) timeText.text = $"MIN";
            if (scoreText) scoreText.text = $"{score}";
        }
    }
}
