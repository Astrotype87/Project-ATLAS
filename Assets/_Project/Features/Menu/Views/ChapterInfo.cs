using UnityEngine;
using TMPro;

namespace ProjectATLAS.Menu
{
    public class ChapterInfo : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private int chapter;
        [SerializeField, TextArea] private string title;
        [SerializeField] private int completedLevels;
        [SerializeField] private int totalLevels;
        [SerializeField] private int obtainedMedals;
        [SerializeField] private int totalMedals;
        
        [Header("Components")]
        [SerializeField] private TMP_Text chapterText;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text levelsText;
        [SerializeField] private TMP_Text medalsText;
        
        // PROPERTIES
        public int Chapter => chapter;
        
        
        // MONOBEHAVIOUR METHODS
        private void OnValidate()
        {
            name = $"Chapter {chapter} Info";
            
            SetChapter(chapter);
            SetTitle(title);
            SetLevels(completedLevels, totalLevels);
            SetMedals(obtainedMedals, totalMedals);
        }
        
        
        // PUBLIC METHODS
        public void SetChapter(int chapter)
        {
            this.chapter = chapter;
            if (chapterText) chapterText.text = $"Chapter {chapter}";
        }
        
        public void SetTitle(string title)
        {
            this.title = title;
            if (titleText) titleText.text = title;
        }
        
        public void SetLevels(int completed, int total)
        {
            completedLevels = completed;
            totalLevels = total;
            
            if (levelsText) levelsText.text = $"{completed}/{total}";
        }
        
        public void SetMedals(int obtained, int total)
        {
            obtainedMedals = obtained;
            totalMedals = total;
            
            if (medalsText) medalsText.text = $"{obtained}/{total}";
        }
    }
}
