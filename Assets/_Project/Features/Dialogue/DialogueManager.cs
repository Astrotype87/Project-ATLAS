using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

using AstrotypeTools.InspectorAttributes;

using ProjectATLAS.TTS;
using ProjectATLAS.System;
using ProjectATLAS.GameData;
using ProjectATLAS.Avatar;

namespace ProjectATLAS.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private bool loadPrefab;
        [SerializeField, AssetOnly] private GameObject dialoguePrefab;
        [SerializeField, AssetOnly] private GameObject guidebooksPrefab;
        
        [Header("Container")]
        [SerializeField] private bool autoStart;
        [SerializeField, SceneOnly] private GameObject dialogueContainer;
        [SerializeField] private bool playFirstStep;
        
        [Header("Components")]
        [SerializeField] private DialogueSequence sequence;
        [SerializeField] private DialogueAnimator animator;
        [SerializeField] private DialogueUI userInterface;
        [SerializeField] private AndroidTTSService textToSpeech;
        
        [Header("Player Character")]
        [SerializeField] private DialogueCharacter dialogueCharacter;
        
        [Header("Debug")]
        [SerializeField] private bool enableLog;
        [SerializeField] private DialogueCondition currentCondition;
        
        // PRIVATE FIELDS
        private int _chapterStory;
        private int _currentStepIndex;
        
        // PROPERTIES
        public bool IsDialogueEnded { get; private set; }
        
        public event Action OnPauseRequested;
        public event Action OnDialogueEnded;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            textToSpeech = AndroidTTSService.Instance;
        }
        
        private void OnEnable()
        {
            if (!Application.isPlaying) return;
            
            userInterface.OnStart += UI_OnStart;
            userInterface.OnNext += UI_OnNext;
            userInterface.OnBack += UI_OnBack;
            userInterface.OnSkip += UI_OnSkip;
            userInterface.OnPause += UI_OnPause;
            
            userInterface.OnPlay += UI_OnPlay;
            userInterface.OnAudio += UI_OnAudio;
            
            if (textToSpeech)
            {
                textToSpeech.OnSpeechStart += TextToSpeech_OnSpeechStart;
                textToSpeech.OnSpeechDone += TextToSpeech_OnSpeechDone;
                textToSpeech.OnSpeechStopped += TextToSpeech_OnSpeechStopped;
            }
        }
        
        private void OnDisable()
        {
            if (!Application.isPlaying) return;
            
            userInterface.OnStart -= UI_OnStart;
            userInterface.OnBack -= UI_OnBack;
            userInterface.OnSkip -= UI_OnSkip;
            userInterface.OnPause -= UI_OnPause;
            
            userInterface.OnPlay -= UI_OnPlay;
            userInterface.OnAudio -= UI_OnAudio;
            
            textToSpeech.OnSpeechStart -= TextToSpeech_OnSpeechStart;
            textToSpeech.OnSpeechDone -= TextToSpeech_OnSpeechDone;
            textToSpeech.OnSpeechStopped -= TextToSpeech_OnSpeechStopped;
        }
        
        private IEnumerator Start()
        {
            if (loadPrefab)
            {
                yield return LoadDialogueAndGuidebooksPrefab(dialoguePrefab, guidebooksPrefab);
            }
            if (autoStart)
            {
                InitializeDialogue(true);
                if (playFirstStep) StartDialogue();
            }
        }
        
        private void Update()
        {
            if (currentCondition)
            {
                bool isConditionMet = currentCondition.IsConditionMet;
                userInterface.SetNextButtonInteractable(isConditionMet);
            }
            else
            {
                userInterface.SetNextButtonInteractable(true);
            }
        }
        
        
        // PUBLIC METHODS
        public IEnumerator LoadDialogueAndGuidebooksPrefab(GameObject dialoguePrefab, GameObject guidebooksPrefab)
        {
            if (dialoguePrefab == null)
            {
                Debug.LogWarning("[DialogueManager] Failed to load dialoguePrefab. The dialoguePrefab is null!");
                yield break;
            }
            
            // Spawn dialogue prefab
            this.dialoguePrefab = dialoguePrefab;
            this.guidebooksPrefab = guidebooksPrefab;
            
            if (dialogueContainer)
            {
                // // Save parent and sibling index information
                // Transform parent = dialogueContainer.transform.parent;
                // int siblingIndex = dialogueContainer.transform.GetSiblingIndex();
                
                // Destroy, instantiate, and wait for instantiation to finish
                Destroy(dialogueContainer);
            }
            
            var instantiateDialoguePrefab = InstantiateAsync(dialoguePrefab); //, parent);
            if (LoadingScreen.Instance)
                yield return instantiateDialoguePrefab.ReportLoadingProgress("Loading dialogue prefab", 0.6667f, 1.0f);
            else
                yield return instantiateDialoguePrefab;
            
            // Assign instantiated dialoguePrefab, and move to dialogue scene
            dialogueContainer = instantiateDialoguePrefab.Result[0];
            
            if (dialogueContainer)
            {
                // dialogueContainer.transform.SetSiblingIndex(siblingIndex);
                SceneManager.MoveGameObjectToScene(dialogueContainer, gameObject.scene);
                
                
                // Spawn guidebooks prefab
                userInterface.LoadGuidebooksPrefab(guidebooksPrefab);
                
                
                // Get reference to sequence and animation
                sequence = dialogueContainer.GetComponent<DialogueSequence>();
                animator = dialogueContainer.GetComponent<DialogueAnimator>();
            }
        }
        
        public void InitializeDialogue(bool isSkippable, int chapterStory = 0) // 0 if no chapter intro story
        {
            GameDataManager gameDataService = GameDataManager.Instance;
            AvatarManager avatarManager = AvatarManager.Instance;
            AvatarProfile avatarProfile = avatarManager.GetAvatarProfileByID(gameDataService.AvatarData.avatarIndex);
            
            dialogueCharacter.name = gameDataService.DetailsData.displayName;
            dialogueCharacter.image = avatarProfile.characterHead;
            
            // Get reference to dialogue sequence and animator
            if (!sequence && dialogueContainer) sequence = dialogueContainer.GetComponent<DialogueSequence>();
            if (!animator && dialogueContainer) animator = dialogueContainer.GetComponent<DialogueAnimator>();
            
            // Initialize states
            IsDialogueEnded = false;
            _currentStepIndex = -1;
            int stepCount = sequence ? sequence.StepCount : 1;
            
            // Initialize UI
            _chapterStory = chapterStory;
            userInterface.Initialize(0, stepCount, isSkippable, chapterStory);
        }
        
        public void StartDialogue()
        {
            userInterface.StartStoryUI();
            
            if (_chapterStory <= 0)
            {
                // Play first step if you want
                SetNextStep();
            }
        }
        
        public void PauseDialogue()
        {
            Time.timeScale = 0.0f;
        }
        
        public void ResumeDialogue()
        {
            Time.timeScale = 1.0f;
        }
        
        public IEnumerator WaitForDialogueEnd()
        {
            yield return new WaitUntil(() => IsDialogueEnded);
        }
        
        
        // PRIVATE METHODS
        private void SetNextStep()
        {
            // Update step index
            int newIndex = _currentStepIndex + 1;
            if (newIndex < 0 || newIndex >= sequence.StepCount) return;
            if (_currentStepIndex == newIndex) return;
            _currentStepIndex = newIndex;
            
            // Take data from sequence
            DialogueStep dialogueStep = sequence.GetDialogueStep(_currentStepIndex);
            
            // Push output to animator, UI, and TTS
            if (dialogueStep.UseAnimation)
            {
                animator.PlaySection(dialogueStep.StartTime, dialogueStep.EndTime);
            }
            else
            {
                float fallbackTime = FindPreviousAnimationEndTime(_currentStepIndex);
                if (fallbackTime > 0f)
                    animator.SkipToTime(fallbackTime);
                else
                    animator.ResetToStart();
            }
            
            // animator.SetDialogueStep(dialogueStep);
            // animator.SaveState();
            // animator.PlayAnimation();
            
            userInterface.DisplayDialogue(dialogueStep, _currentStepIndex + 1, sequence.StepCount);
            if (dialogueStep.UseTTS)
            {
                string playerName = GameDataManager.Instance.DetailsData.displayName;
                textToSpeech.SetPitch(dialogueStep.Pitch);
                textToSpeech.Speak(dialogueStep.Message.Replace("{player}", playerName));
            }
            
            currentCondition = dialogueStep.DialogueCondition;
        }
        
        private void SetPreviousStep()
        {
            // Update step index
            int newIndex = _currentStepIndex - 1;
            if (newIndex < 0 || newIndex >= sequence.StepCount) return;
            if (_currentStepIndex == newIndex) return;
            _currentStepIndex = newIndex;
            
            // Take data from sequence
            DialogueStep dialogueStep = sequence.GetDialogueStep(_currentStepIndex);
            
            // Push output to animator, UI, and TTS
            if (dialogueStep.UseAnimation)
            {
                animator.SkipToTime(dialogueStep.EndTime);
            }
            else
            {
                float fallbackTime = FindPreviousAnimationEndTime(_currentStepIndex);
                if (fallbackTime > 0f)
                    animator.SkipToTime(fallbackTime);
                else
                    animator.ResetToStart();
            }
            
            // animator.RestoreState();
            // animator.SetDialogueStep(dialogueStep);
            // animator.CompleteAnimation();
            
            userInterface.DisplayDialogue(dialogueStep, _currentStepIndex + 1, sequence.StepCount);
            
            currentCondition = dialogueStep.DialogueCondition;
        }
        
        private void CompleteDialogue()
        {
            IsDialogueEnded = true;
            OnDialogueEnded?.Invoke();
        }
        
        // private void RestartDialogue()
        // {
            
        // }
        
        private void LogMessage(object message)
        {
            if (enableLog) Debug.Log(message);
        }
        
        
        private float FindPreviousAnimationEndTime(int index)
        {
            for (int i = index - 1; i >= 0; i--)
            {
                DialogueStep prev = sequence.GetDialogueStep(i);
                if (prev != null && prev.UseAnimation)
                    return prev.EndTime;
            }
            return 0f; // default start of timeline
        }
        
        
        // LISTENERS
        private void UI_OnStart()
        {
            SetNextStep();
        }
        
        private void UI_OnNext()
        {
            if (!animator.IsAnimating && !userInterface.IsAnimating)
            {
                if (_currentStepIndex >= sequence.StepCount - 1)
                    CompleteDialogue();
                else
                    SetNextStep();
            }
            else
            {
                if (animator.IsAnimating) animator.SkipAnimation();
                if (userInterface.IsAnimating) userInterface.CompleteAnimation(); 
            }
            
            if (IsDialogueEnded) LogMessage($"Dialogue completed.");
            else LogMessage($"Next dialogue.");
        }
        
        private void UI_OnBack()
        {
            if (_currentStepIndex > 0)
            {
                if (animator.IsAnimating) animator.SkipAnimation();
                if (userInterface.IsAnimating) userInterface.CompleteAnimation();
                
                SetPreviousStep();
                
                if (animator.IsAnimating) animator.SkipAnimation();
                if (userInterface.IsAnimating) userInterface.CompleteAnimation();
                
                LogMessage($"Previous dialogue.");
            }
        }
        
        private void UI_OnSkip()
        {
            CompleteDialogue();
            LogMessage($"Skip dialogue.");
        }
        
        private void UI_OnPause()
        {
            OnPauseRequested?.Invoke();
            LogMessage($"Pause dialogue.");
        }
        
        private void UI_OnPlay()
        {
            if (textToSpeech.IsSpeaking)
            {
                textToSpeech.Stop();
                LogMessage($"Stop TTS");
            }
            else if (_currentStepIndex >= 0)
            {
                DialogueStep dialogueStep = sequence.GetDialogueStep(_currentStepIndex);
                if (dialogueStep.UseTTS)
                {
                    string playerName = GameDataManager.Instance.DetailsData.displayName;
                    textToSpeech.SetPitch(dialogueStep.Pitch);
                    textToSpeech.Speak(dialogueStep.Message.Replace("{player}", playerName));
                    LogMessage($"Speak TTS");
                }
            }
        }
        
        private void UI_OnAudio()
        {
            textToSpeech.SetMute(!textToSpeech.IsMuted);
            userInterface.SetAudioIcon(textToSpeech.IsMuted);
            
            LogMessage($"{(textToSpeech.IsMuted ? "Muted" : "Unmuted")} TTS");
        }
        
        private void TextToSpeech_OnSpeechStart(string message)
        {
            userInterface.SetPlayIcon(true);
        }
        
        private void TextToSpeech_OnSpeechDone(string message)
        {
            userInterface.SetPlayIcon(false);
        }
        
        private void TextToSpeech_OnSpeechStopped(string message)
        {
            userInterface.SetPlayIcon(false);
        }
        
    }
}
