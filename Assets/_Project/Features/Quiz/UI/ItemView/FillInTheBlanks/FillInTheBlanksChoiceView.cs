using UnityEngine;
using TMPro;

namespace ProjectATLAS.Quiz.UI
{
    public class FillInTheBlanksChoiceView : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private string choice;
        
        [Header("Components")]
        [SerializeField] private TMP_Text choiceText;
        
        public string Choice => choice;
        
        
        // MONOBEHAVIOUR METHODS
        private void OnValidate()
        {
            if (choiceText) choiceText.text = choice;
        }
        
        // PUBLIC METHODS
        public void DisplayChoice(string choice)
        {
            this.choice = choice;
            
            if (choiceText) choiceText.text = choice;
        }
    }
}
