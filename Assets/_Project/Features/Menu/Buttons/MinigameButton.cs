using UnityEngine;
using UnityEngine.UI;
using TMPro;

using KBCore.Refs;

using ProjectATLAS.UI;
using ProjectATLAS.Gameplay;

namespace ProjectATLAS.Menu
{
    public class MinigameButton : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private new string name;
        [SerializeField] private int number;
        [SerializeField] private ChallengeLevelData challengeLevelData;
        [SerializeField] private bool isLocked;
        
        [Header("Components")]
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text numberText;
        [SerializeField, Self] private Button button;
        [SerializeField, Self] private UIToggleButton toggleButton;
        [SerializeField] private GameObject lockObject;
        
        public string LevelID => challengeLevelData.ID;
        public ChallengeLevelData ChallengeLevelData => challengeLevelData;
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
            base.name = $"Mini {number} Button";
            
            string nameString = isLocked ? $"Complete Level {challengeLevelData.Number} to unlock." : name;
            
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
