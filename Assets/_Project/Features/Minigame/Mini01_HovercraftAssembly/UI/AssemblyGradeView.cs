using UnityEngine;
using TMPro;

namespace ProjectATLAS
{
    public class AssemblyGradeView : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private string partName;
        [SerializeField] private Vector2 correctPosition;
        [SerializeField] private Vector2 yourPosition;
        [SerializeField] private Vector2 difference;
        [SerializeField] private bool isCorrect;
        
        [Header("Settings")]
        [SerializeField] private int rounding = 2; // how many decimals to display
        
        [Header("Components")]
        [SerializeField] private TMP_Text partNameText;
        [SerializeField] private TMP_Text correctPositionText;
        [SerializeField] private TMP_Text answerPositionText;
        [SerializeField] private TMP_Text differenceText;
        [SerializeField] private GameObject correctIcon;
        [SerializeField] private GameObject wrongIcon;
        
        
        // MONOBEHAVIOUR METHODS
        private void OnValidate()
        {
            // Update UI based on current serialized values
            SetData(partName, correctPosition, yourPosition);
        }
        
        
        // PUBLIC METHODS
        public void SetData(string partName, Vector2 correctPos, Vector2 answerPos)
        {
            this.partName = partName;
            this.correctPosition = correctPos;
            this.yourPosition = answerPos;
            
            // Calculate difference
            this.difference = answerPos - correctPos;
            this.isCorrect = Vector2.Distance(correctPos, answerPos) < 0.0001f;
            
            // Update UI
            if (partNameText) partNameText.text = partName;
            if (correctPositionText) correctPositionText.text = FormatVector(correctPos);
            if (answerPositionText) answerPositionText.text = FormatVector(answerPos);
            if (differenceText) differenceText.text = FormatVector(difference, true);
            
            if (correctIcon) correctIcon.SetActive(isCorrect);
            if (wrongIcon) wrongIcon.SetActive(!isCorrect);
        }
        
        // PRIVATE METHODS
        private string FormatVector(Vector2 v, bool showSign = false)
        {
            string format = "F" + rounding;
            string x = v.x.ToString(format);
            string y = v.y.ToString(format);
            
            if (showSign)
            {
                if (!x.StartsWith("-")) x = "+" + x;
                if (!y.StartsWith("-")) y = "+" + y;
            }
            
            return $"({x}, {y})";
        }
    }
}
