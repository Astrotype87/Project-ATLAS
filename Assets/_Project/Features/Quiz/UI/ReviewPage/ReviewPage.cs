using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProjectATLAS.UI;
using KBCore.Refs;

namespace ProjectATLAS.Quiz.UI
{
    public class ReviewPage : UIPage
    {
        [Header("Components")]
        [SerializeField] private ReviewListPanel reviewListPanel;
        [SerializeField] private Button finishButton;
        
        // PROPERTIES
        public event Action<int> OnEditClicked;
        public event Action OnFinishClicked;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            reviewListPanel.OnEditClicked += ReviewListPanel_OnEditClicked;
            finishButton.onClick.AddListener(FinishButton_onClick);
        }
        
        
        // PUBLIC METHODS
        public void UpdateReviewItemsList((string, string, string)[] items, bool hideDifficulty)
        {
            reviewListPanel.DisplayReviewItems(items, hideDifficulty);
        }
        
        
        // EVENT LISTENER METHODS
        private void ReviewListPanel_OnEditClicked(int index)
        {
            OnEditClicked?.Invoke(index);
        }
        
        private void FinishButton_onClick()
        {
            OnFinishClicked?.Invoke();
        }
    }
}
