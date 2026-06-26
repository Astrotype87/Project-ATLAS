using System;
using System.Collections;
using UnityEngine;

using ProjectATLAS.Gameplay;
using ProjectATLAS.Gameplay.UI;

namespace ProjectATLAS.Freeplay.UI
{
    public class FreeplayUI : MonoBehaviour
    {
        [Header("Pages")]
        [SerializeField] private PausePage pausePage;
        [SerializeField] private FreeplayCompletePage completePage;
        
        private bool _isCompleteActionRequested;
        
        // PROPERTIES
        public event Action OnResume;
        public event Action OnRestart;
        public event Action OnQuit;
        
        public CompleteAction CompleteAction { get; private set; }
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            completePage.OnCompleteAction += CompletePage_OnCompleteAction;
            
            pausePage.OnResumeClicked += PausePage_OnResumeClicked;
            pausePage.OnRestartClicked += PausePage_OnRestartClicked;
            pausePage.OnQuitClicked += PausePage_OnQuitClicked;
            
            pausePage.ClosePage();
            completePage.ClosePage();
        }
        
        
        // PUBLIC METHODS: Pause
        public void ShowPausePage()
        {
            pausePage.OpenPage();
        }
        
        public void HidePausePage()
        {
            pausePage.ClosePage();
        }
        
        
        // PUBLIC METHODS: Complete
        public void DisplayCompletePage()
        {
            completePage.OpenPageInGroup();
        }
        
        public void DisplayCompleted(string levelName, bool isCompleted)
        {
            completePage.DisplayCompleted(levelName, isCompleted);
        }
        
        public IEnumerator WaitForCompleteAction()
        {
            yield return new WaitUntil(() => _isCompleteActionRequested);
            _isCompleteActionRequested = false;
        }
        
        
        // EVENT LISTENER METHODS
        private void PausePage_OnResumeClicked() => OnResume?.Invoke();
        private void PausePage_OnRestartClicked() => OnRestart?.Invoke();
        private void PausePage_OnQuitClicked() => OnQuit?.Invoke();
        
        private void CompletePage_OnCompleteAction(CompleteAction completeAction)
        {
            completePage.ClosePage();
            
            _isCompleteActionRequested = true;
            CompleteAction = completeAction;
        }
        
    }
}
