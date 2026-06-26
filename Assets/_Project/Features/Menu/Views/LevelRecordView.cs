using System;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace ProjectATLAS
{
    public class LevelRecordView : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private int number;
        [SerializeField] private int score;
        [SerializeField] private float time;
        [SerializeField] private string dateTime;
        
        [Header("Components")]
        [SerializeField] private TMP_Text numberText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private TMP_Text dateTimeText;
        
        // MONOBEHAVIOUR METHODS
        private void OnValidate()
        {
            DisplayLevelRecord(number, score, time, dateTime);
        }
        
        
        // PUBLIC METHODS
        public void DisplayLevelRecord(int number, int score, float time, string dateTime)
        {
            this.number = number;
            this.score = score;
            this.time = time;
            this.dateTime = dateTime;
            
            if (numberText) numberText.text = number.ToString();
            if (scoreText) scoreText.text = score.ToString("N0");
            if (timeText)
            {
                int minutes = (int)(time / 60f);
                int seconds = (int)(time % 60f);
                int centiseconds = (int)((time - Mathf.Floor(time)) * 100f);
                string formatted = $"{minutes:00}:{seconds:00}:{centiseconds:00}";
                
                timeText.text = formatted;
            }
            if (dateTimeText)
            {
                if (DateTime.TryParseExact(dateTime,
                        Standard.GameData_DateTimeFormat,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime dt))
                {
                    dateTimeText.text = dt.ToString("yyyy/MM/dd  hh:mm:ss tt");
                }
            }
        }
    }
}
