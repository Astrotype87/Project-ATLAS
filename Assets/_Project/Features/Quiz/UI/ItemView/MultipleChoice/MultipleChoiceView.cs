using System;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ProjectATLAS.Quiz.UI
{
    public class MultipleChoiceView : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] [TextArea] private string question;
        
        [Header("Components")]
        [SerializeField] private TMP_Text questionComponent;
        [SerializeField] [FormerlySerializedAs("answerPanel")] private MultipleChoicesPanel choicesPanel;
        [SerializeField] private Image image;
        
        // PROPERTIES
        public event Action<string> OnAnswerChanged;
        
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            choicesPanel.OnAnswerButtonClicked += ChoicesPanel_OnAnswerButtonClicked;
        }
        
        private void OnValidate()
        {
            if (questionComponent) questionComponent.text = question;
        }
        
        
        // PUBLIC METHODS
        public void DisplayItem(MultipleChoiceItem item, MultipleChoiceAnswer answer)
        {
            question = item.GetQuestion();
            questionComponent.text = question;
            
            choicesPanel.SetAnswersPanelDisplay(item.GetChoices(), answer?.answer);
        }
        
        // EVENT LISTENER METHODS
        private void ChoicesPanel_OnAnswerButtonClicked(string answer)
        {
            OnAnswerChanged?.Invoke(answer);
        }
    }
}
