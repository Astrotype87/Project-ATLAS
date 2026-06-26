using UnityEngine;
using UnityEngine.UI;
using TMPro;

using KBCore.Refs;

using ProjectATLAS.UI;
using ProjectATLAS.Gameplay;

namespace ProjectATLAS.Menu
{
    public class SimulationButton : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private new string name;
        [SerializeField] private int number;
        [SerializeField] private SimulationLevelData simulationLevelData;
        [SerializeField] private bool isLocked;
        
        [Header("Components")]
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text numberText;
        [SerializeField, Self] private Button button;
        [SerializeField, Self] private UIToggleButton toggleButton;
        [SerializeField] private GameObject lockObject;
        
        public string LevelID => simulationLevelData.ID;
        public SimulationLevelData SimulationLevelData => simulationLevelData;
        public bool IsSelected => toggleButton.isOn;
        
        
        // MONOBEHAVIOUR METHODS
        private void OnValidate()
        {
            UpdateUI(isLocked);
        }
        
        // PUBLIC METHODS
        public void UpdateUI(bool isLocked)
        {
            this.isLocked = isLocked;
            base.name = $"Sim {number} Button";
            
            string nameString = isLocked ? $"Complete Level {simulationLevelData.Number} to unlock." : name;
            
            if (nameText) nameText.text = nameString;
            if (numberText) numberText.text = $"{number}";
            
            if (toggleButton)
            {
                toggleButton.offStyle.text = nameString;
                toggleButton.onStyle.text = nameString;
            }
            
            // Lock & unlock UI logic
            if (button) button.interactable = !isLocked;
            if (lockObject) lockObject.SetActive(isLocked);
        }
    }
}
