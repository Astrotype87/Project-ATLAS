using UnityEngine;
using TMPro;

namespace ProjectATLAS.Simulation.Sim01_PartsMeasurements
{
    public class BreakdownView : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private string partName;
        [SerializeField] private float correctValue;
        [SerializeField] private float answerValue;
        [SerializeField] private float difference;
        [SerializeField] private bool isCorrect;
        [SerializeField, Range(0, 4)] private int rounding = 1;
        
        [Header("Components")]
        [SerializeField] private TMP_Text partNameText;
        [SerializeField] private TMP_Text correctValueText;
        [SerializeField] private TMP_Text answerValueText;
        [SerializeField] private TMP_Text differenceText;
        [SerializeField] private GameObject correctIcon;
        [SerializeField] private GameObject wrongIcon;
        
        
        // MONOBEHAVIOUR METHODS
        private void OnValidate()
        {
            // Update UI based on current serialized values
            SetData(partName, correctValue, answerValue);
        }
        
        
        // PUBLIC METHODS
        public void SetData(string partName, float correctValue, float answerValue)
        {
            this.partName = partName;
            this.correctValue = correctValue;
            this.answerValue = answerValue;
            
            // Calculate difference
            this.difference = answerValue - correctValue;
            this.isCorrect = Mathf.Approximately(correctValue, answerValue);
            
            // Update UI
            if (partNameText) partNameText.text = partName;
            if (correctValueText) correctValueText.text = correctValue.ToString($"F{rounding}");
            if (answerValueText) answerValueText.text = answerValue.ToString($"F{rounding}");
            if (differenceText) differenceText.text = difference.ToString($"+0.{new string('0', rounding)};-0.{new string('0', rounding)};0");
            
            if (correctIcon) correctIcon.SetActive(isCorrect);
            if (wrongIcon) wrongIcon.SetActive(!isCorrect);
        }
    }
}
