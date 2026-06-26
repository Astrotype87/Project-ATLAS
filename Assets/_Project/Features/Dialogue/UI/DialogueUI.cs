using System;
using ProjectATLAS.GameData;
using ProjectATLAS.Library.Guidebooks;
using ProjectATLAS.UI;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectATLAS.Dialogue
{
    public class DialogueUI : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private DialoguePanelView panelView;
        [SerializeField] private DialogueProgressView progressView;
        [Space]
        [SerializeField] private DialogueStoryPage dialogueStoryPage;
        [SerializeField] private UIPage dialoguePage;
        [SerializeField] private UIPage dialogueGuidebookPage;
        [SerializeField] private RectTransform contentTransform; // Where guidebook is placed into it the COntent game object in scroll view
        [SerializeField] private GuidebookDataPage guidebookDataPage;
        [Space]
        [SerializeField] private Button nextButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Button skipButton;
        [SerializeField] private UIOpenPageAction openGuidebookButton;
        [SerializeField] private Button pauseButton;
        
        private int chapter;
        
        public bool IsAnimating => panelView.IsAnimating;
        
        public event Action OnStart;
        public event Action OnNext;
        public event Action OnBack;
        public event Action OnSkip;
        public event Action OnPause;
        
        public event Action OnPlay;
        public event Action OnAudio;
        
        
        private void OnEnable()
        {
            if (!Application.isPlaying) return;
            
            if (dialogueStoryPage) dialogueStoryPage.OnStart += DialogueStoryPage_OnStart;
            
            if (nextButton) nextButton.onClick.AddListener(NextButton_onClick);
            if (backButton) backButton.onClick.AddListener(BackButton_onClick);
            if (skipButton) skipButton.onClick.AddListener(SkipButton_onClick);
            if (pauseButton) pauseButton.onClick.AddListener(PauseButton_onClick);
            
            if (panelView)
            {
                panelView.OnPlayClicked += PanelView_OnPlayClicked;
                panelView.OnAudioClicked += PanelView_OnAudioClicked;
            }
        }
        
        private void OnDisable()
        {
            if (!Application.isPlaying) return;
            
            if (dialogueStoryPage) dialogueStoryPage.OnStart -= DialogueStoryPage_OnStart;
            
            if (nextButton) nextButton.onClick.RemoveListener(NextButton_onClick);
            if (backButton) backButton.onClick.RemoveListener(BackButton_onClick);
            if (skipButton) skipButton.onClick.RemoveListener(SkipButton_onClick);
            if (pauseButton) pauseButton.onClick.RemoveListener(PauseButton_onClick);
            
            if (panelView)
            {
                panelView.OnPlayClicked -= PanelView_OnPlayClicked;
                panelView.OnAudioClicked -= PanelView_OnAudioClicked;
            }
        }
        
        
        // PUBLIC METHODS
        public void Initialize(int currentStep = 0, int totalSteps = 1, bool isSkippable = false, int chapter = 0) // chapter = 0 if no chapter
        {
            panelView.SetIcon(null);
            panelView.SetName(string.Empty);
            panelView.SetMessage(string.Empty);
            
            progressView.SetTitle(string.Empty);
            progressView.SetProgress(currentStep, totalSteps);
            
            skipButton.gameObject.SetActive(isSkippable);
            
            this.chapter = chapter;
            
            if (chapter > 0)
            {
                dialogueStoryPage.OpenPageInGroup();
            }
            else
            {
                dialoguePage.OpenPageInGroup();
            }
        }
        
        public void StartStoryUI()
        {
            dialogueStoryPage.DisplayStoryText(chapter);
        }
        
        public void LoadGuidebooksPrefab(GameObject guidebooksPrefab)
        {
            if (guidebooksPrefab == null)
            {
                dialogueGuidebookPage.ClosePage();
                openGuidebookButton.gameObject.SetActive(false);
                
                return;
            }
            
            // Instantiate guidebook, get reference to GuidebookDataPage, and assign it to the guide button
            GameObject newGameObject = Instantiate(guidebooksPrefab, contentTransform);
            guidebookDataPage = newGameObject.GetComponent<GuidebookDataPage>();
            
            openGuidebookButton.SetPage(guidebookDataPage);
        }
        
        public void DisplayDialogue(DialogueStep dialogueStep, int currentStep, int totalSteps)
        {
            string playerName = GameDataManager.Instance.DetailsData.displayName;
            
            panelView.SetIcon(dialogueStep.Image);
            panelView.SetName(dialogueStep.Name);
            panelView.SetMessageAnimated(dialogueStep.Message.Replace("{player}", playerName));
            
            progressView.SetTitle("Dialogue Scene");
            progressView.SetProgress(currentStep, totalSteps);
        }
        
        
        
        public void SetPlayIcon(bool isPlaying)
        {
            panelView.SetPlayIcon(isPlaying);
        }
        
        public void SetAudioIcon(bool isMuted)
        {
            panelView.SetAudioIcon(isMuted);
        }
        
        public void SetNextButtonInteractable(bool interactable)
        {
            nextButton.interactable = interactable;
        }
        
        public void CompleteAnimation()
        {
            panelView.CompleteAnimation();
        }
        
        
        // LISTENERS
        private void DialogueStoryPage_OnStart()
        {
            dialoguePage.OpenPageInGroup();
            OnStart?.Invoke();
        }
        private void NextButton_onClick() => OnNext?.Invoke();
        private void BackButton_onClick() => OnBack?.Invoke();
        private void SkipButton_onClick() => OnSkip?.Invoke();
        private void PauseButton_onClick() => OnPause?.Invoke();
        private void PanelView_OnPlayClicked() => OnPlay?.Invoke();
        private void PanelView_OnAudioClicked() => OnAudio?.Invoke();
    }
}
