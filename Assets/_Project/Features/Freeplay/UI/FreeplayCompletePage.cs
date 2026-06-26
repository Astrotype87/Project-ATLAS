using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using ProjectATLAS.Gameplay;
using ProjectATLAS.UI;

namespace ProjectATLAS.Freeplay.UI
{
    public class FreeplayCompletePage : UIPage
    {
        [Header("Data")]
        [SerializeField] private string title;
        [SerializeField] private bool isCompleted = true;
        
        [Header("Components")]
        [SerializeField] private TMP_Text completeText;
        [SerializeField] private Button menuButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button nextButton;
        
        
        // PROPERTIES
        public event Action<CompleteAction> OnCompleteAction;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            menuButton.onClick.AddListener(MenuButton_onClick);
            restartButton.onClick.AddListener(RestartButton_onClick);
            nextButton.onClick.AddListener(NextButton_onClick);
        }
        
        // PUBLIC METHODS
        public void DisplayCompleted(string title, bool isCompleted)
        {
            this.title = title;
            this.isCompleted = isCompleted;
            
            completeText.text = $"{title} {(isCompleted ? "Complete" : "Failed")}";
        }
        
        // EVENT LISTENER METHODS
        private void MenuButton_onClick() => OnCompleteAction?.Invoke(CompleteAction.Menu);
        private void RestartButton_onClick() => OnCompleteAction?.Invoke(CompleteAction.Restart);
        private void NextButton_onClick() => OnCompleteAction?.Invoke(CompleteAction.Freeplay);
    }
}
