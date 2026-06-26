using System;
using System.Collections;
using UnityEngine;

namespace ProjectATLAS.Gameplay.UI
{
    public class GameplayUI : MonoBehaviour
    {
        [Header("Pages")]
        [SerializeField] private PausePage pausePage;
        [SerializeField] private CompletePage completePage;
        
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
        
        public void DisplayUnlocks(UnlockData[] unlockDatas)
        {
            completePage.DisplayUnlocks(unlockDatas);
        }
        
        public void SetMedalsVisible(bool visible)
        {
            completePage.SetMedalsVisible(visible);
        }
        
        public void DisplayMedalsObtained(bool bronze, bool silver, bool gold)
        {
            completePage.DisplayMedalsObtained(bronze, silver, gold);
        }
        
        public void DisplayMedalsObjectives(string bronzeObjective, string silverObjective, string goldObjective, float goldTime)
        {
            completePage.DisplayMedalsObjectives(bronzeObjective, silverObjective, goldObjective, goldTime);
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
