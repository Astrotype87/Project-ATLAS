using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using ProjectATLAS.Utility;

namespace ProjectATLAS.Quiz.UI
{
    public class TrueOrFalseView : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] [TextArea] private string question;
        
        [Header("Components")]
        [SerializeField] private TMP_Text questionComponent;
        [SerializeField] private TrueOrFalseButton trueButton;
        [SerializeField] private TrueOrFalseButton falseButton;
        
        // PROPERTIES
        public event Action<bool> OnAnswerChanged;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            trueButton.OnClicked += TrueButton_OnClicked;
            falseButton.OnClicked += FalseButton_OnClicked;
        }
        
        private void OnValidate()
        {
            if (questionComponent) questionComponent.text = question;
        }
        
        // PUBLIC METHODS
        public void DisplayItem(TrueOrFalseItem item, TrueOrFalseAnswer trueOrFalseAnswer)
        {
            question = item.GetQuestion();
            questionComponent.text = question;
            
            if (trueOrFalseAnswer != null)
            {
                if (trueOrFalseAnswer.answer.AsBool() == true)
                {
                    trueButton.SetSelected(true);
                    falseButton.SetSelected(false);
                }
                else
                {
                    trueButton.SetSelected(false);
                    falseButton.SetSelected(true);
                }
            }
            else
            {
                trueButton.SetSelected(false);
                falseButton.SetSelected(false);
            }
        }
        
        // EVENT LISTENER METHODS
        private void TrueButton_OnClicked()
        {
            falseButton.SetSelected(false);
            OnAnswerChanged?.Invoke(true);
        }
        
        private void FalseButton_OnClicked()
        {
            trueButton.SetSelected(false);
            OnAnswerChanged?.Invoke(false);
        }
    }
}
