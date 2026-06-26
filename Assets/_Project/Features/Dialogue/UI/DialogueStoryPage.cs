using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using ProjectATLAS.UI;
using ProjectATLAS.TTS;

namespace ProjectATLAS.Dialogue
{
    public class DialogueStoryPage : UIPage
    {
        [Header("Story Data")]
        [SerializeField] private StoryData[] storyDatas;
        
        [Header("Typing Animation")]
        [SerializeField] private float typingRate = 0.02f;
        
        [Header("Components")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text storyText;
        [SerializeField] private TMP_Text buttonText;
        [SerializeField] private Button startButton;
        
        private AndroidTTSService textToSpeech;
        
        private string[] currentParagraphs;
        private int paragraphIndex;
        private Coroutine typingCoroutine;
        private bool isTyping;
        
        
        // PROPERTIES
        public event Action OnStart;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            textToSpeech = AndroidTTSService.Instance;
            startButton.onClick.AddListener(StartButton_onClick);
        }
        
        
        // PUBLIC METHODS
        public void DisplayStoryText(int chapter)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }
            if (textToSpeech && textToSpeech.IsSpeaking) textToSpeech.Stop();
            storyText.text = "";
            buttonText.text = "Next";
            
            
            if (chapter < 1 || chapter > storyDatas.Length)
            {
                Debug.LogWarning("Chapter out of bounds!");
                return;
            }
            
            StoryData storyData = storyDatas[chapter - 1];
            
            if (titleText) titleText.text = $"Chapter {storyData.Chapter}\n{storyData.Title}";
            
            currentParagraphs = Array.FindAll(
                storyData.Story.Split(new[] { "\n\n" },
                StringSplitOptions.None),
                p => !string.IsNullOrWhiteSpace(p));
            
            paragraphIndex = -1;
            
            ShowNextParagraph();
        }
        
        
        // PRIVATE METHODS
        private void ShowNextParagraph()
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);
            
            if (paragraphIndex + 1 >= currentParagraphs.Length)
            {
                CompleteStory();
                return;
            }
            
            paragraphIndex++;
            typingCoroutine = StartCoroutine(TypeText(currentParagraphs[paragraphIndex]));
        }
        
        
        private IEnumerator TypeText(string text)
        {
            isTyping = true;
            if (storyText) storyText.text = "";
            
            // Start TTS
            if (textToSpeech) textToSpeech.Speak(text);
            
            foreach (char c in text)
            {
                if (storyText) storyText.text += c;
                yield return new WaitForSeconds(typingRate); // Adjust typing speed
            }
            
            // Append (current/total) after the paragraph
            if (storyText) storyText.text += $"\n({paragraphIndex + 1}/{currentParagraphs.Length})\n";
            
            isTyping = false;
            typingCoroutine = null;
            
            // Update button label after typing finishes
            if (paragraphIndex == currentParagraphs.Length - 1)
                buttonText.text = "Start";
            else
                buttonText.text = "Next";
        }
        
        private void CompleteStory()
        {
            OnStart?.Invoke();
            // Optional: clear text or hide page if needed
        }
        
        
        
        // EVENT LISTENER METHODS
        private void StartButton_onClick()
        {
            if (isTyping)
            {
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                    typingCoroutine = null;
                }
                
                if (storyText)
                    storyText.text = currentParagraphs[paragraphIndex] + 
                                    $"\n({paragraphIndex + 1}/{currentParagraphs.Length})";
                
                isTyping = false;
            }
            else
            {
                if (paragraphIndex >= currentParagraphs.Length)
                {
                    // Story complete, invoke event
                    CompleteStory();
                    return;
                }

                ShowNextParagraph();
            }
        }
    }
}
