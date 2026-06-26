using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KBCore.Refs;

namespace ProjectATLAS.Menu
{
    public class ChapterTestScoreView : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private int chapter;
        [SerializeField] private float preTestScore;
        [SerializeField] private float preTestMax;
        [SerializeField] private float postTestScore;
        [SerializeField] private float postTestMax;
        [SerializeField] private string result;
        [SerializeField] private float difference;
        [SerializeField] private bool takenPreTest;
        [SerializeField] private bool takenPostTest;
        
        [Header("Components")]
        [SerializeField] private TMP_Text chapterText;
        [SerializeField] private TMP_Text preTestText;
        [SerializeField] private TMP_Text postTestText;
        [SerializeField] private TMP_Text resultText;
        [SerializeField] private TMP_Text differenceText;
        [SerializeField] private Image resultImage;
        [SerializeField, Self] private CanvasGroup canvasGroup;
        
        [Header("Images")]
        [SerializeField] private Sprite improvedIcon;
        [SerializeField] private Sprite declinedIcon;
        [SerializeField] private Sprite retainedIcon;
        
        
        // MONOBEHAVIOUR METHODS
        private void OnValidate()
        {
            DisplayChapterTestScore(chapter, preTestScore, preTestMax, postTestScore, postTestMax, takenPreTest, takenPostTest);
        }
        
        // PUBLIC METHODS
        public void DisplayChapterTestScore(
            int chapter,
            float preTestScore, float preTestMax, float postTestScore, float postTestMax,
            bool takenPreTest, bool takenPostTest)
        {
            this.chapter = chapter;
            this.preTestScore = preTestScore;
            this.preTestMax = preTestMax;
            this.postTestScore = postTestScore;
            this.postTestMax = postTestMax;
            this.result =
                postTestScore == postTestMax ? "Perfect"
                : preTestScore == postTestMax ? "Retained"
                : (postTestScore / postTestMax) >= (preTestScore / preTestMax) ? "Improved"
                : "Declined";
            this.difference = postTestScore / postTestMax - preTestScore / preTestMax;
            
            if (chapterText) chapterText.text = $"{chapter:00}";
            if (preTestText) preTestText.text = takenPreTest ? $"{preTestScore}/{preTestMax}" : "---";
            if (postTestText) postTestText.text = takenPostTest ? $"{postTestScore}/{postTestMax}" : "---";
            if (resultText) resultText.text = takenPreTest && takenPostTest ? result : "---";
            if (resultImage)
            {
                float preTestPercent = preTestScore / preTestMax;
                float postTestPercent = postTestScore / postTestMax;
                
                resultImage.sprite =
                    !(takenPreTest && takenPostTest) ? null
                    : preTestPercent == postTestPercent ? retainedIcon
                    : postTestPercent >= preTestPercent ? improvedIcon
                    : declinedIcon;
                resultImage.color = takenPreTest && takenPostTest ? Color.white : Color.clear;
            }
            if (differenceText) differenceText.text = takenPreTest && takenPostTest ? $"{difference:+0%;-0%;0%}" : "---";
            
            if (canvasGroup) canvasGroup.alpha = takenPreTest || takenPostTest ? 1f : 0.5f;
        }
    }
}   
