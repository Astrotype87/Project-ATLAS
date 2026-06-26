using TMPro;
using UnityEngine;

namespace ProjectATLAS
{
    public class SequenceAnswerView : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] [TextArea] private string answer;
        
        [Header("Components")]
        [SerializeField] private TMP_Text answerText;
        
        public string Answer => answer;
        
        
        // MONOBEHAVIOUR METHODS
        private void OnValidate()
        {
            if (answerText) answerText.text = answer;
        }
        
        // PUBLIC METHODS
        public void DisplayAnswer(string answer)
        {
            this.answer = answer;
            
            if (answerText) answerText.text = answer;
        }
    }
}
