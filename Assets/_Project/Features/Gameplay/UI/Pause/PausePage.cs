using UnityEngine;
using UnityEngine.UI;
using System;

using ProjectATLAS.UI;

namespace ProjectATLAS.Gameplay.UI
{
    public class PausePage : UIPage
    {
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button quitButton;
        
        public event Action OnResumeClicked;
        public event Action OnRestartClicked;
        public event Action OnQuitClicked;
        
        private void Awake()
        {
            resumeButton.onClick.AddListener(ResumeButton_onClick);
            restartButton.onClick.AddListener(RestartButton_onClick);
            quitButton.onClick.AddListener(QuitButton_onClick);
        }
        
        private void ResumeButton_onClick() => OnResumeClicked?.Invoke();
        private void RestartButton_onClick() => OnRestartClicked?.Invoke();
        private void QuitButton_onClick() => OnQuitClicked?.Invoke();
    }
}
