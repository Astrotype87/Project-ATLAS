using System;
using UnityEngine;
using UnityEngine.UI;
using KBCore.Refs;

using ProjectATLAS.UI;
using ProjectATLAS.Gameplay;

namespace ProjectATLAS.Menu
{
    public class SelectSimulationPage : UIPage
    {
        [SerializeField] private Difficulty difficulty;
        [SerializeField, Child] private SimulationButton[] simulationButtons;
        [SerializeField] private UIToggleButton easyToggle;
        [SerializeField] private UIToggleButton mediumToggle;
        [SerializeField] private UIToggleButton hardToggle;
        
        [SerializeField] private Button playButton;
        
        
        // PROPERTIES3
        public event Action<SimulationLevelData, Difficulty> OnStartClicked;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            easyToggle.OnValueChanged += EasyToggle_OnValueChanged;
            mediumToggle.OnValueChanged += MediumToggle_OnValueChanged;
            hardToggle.OnValueChanged += HardToggle_OnValueChanged;
            
            playButton.onClick.AddListener(PlayButton_onClick);
        }
        
        public override void OpenPage()
        {
            base.OpenPage();
            
            // check lock unlock logic
            foreach (var simulationButton in simulationButtons)
            {
                bool isCompleted = LevelManager.Instance.IsLevelCompleted(simulationButton.LevelID);
                simulationButton.UpdateUI(!isCompleted);
            }
        }
        
        
        private void PlayButton_onClick()
        {
            foreach (var simulationButton in simulationButtons)
            {
                if (simulationButton.IsSelected)
                {
                    OnStartClicked?.Invoke(simulationButton.SimulationLevelData, difficulty);
                    return;
                }
            }
        }
        
        private void EasyToggle_OnValueChanged(UIToggleButton toggleButton, bool value)
            => difficulty = Difficulty.Easy;
        private void MediumToggle_OnValueChanged(UIToggleButton toggleButton, bool value)
            => difficulty = Difficulty.Medium;
        private void HardToggle_OnValueChanged(UIToggleButton toggleButton, bool value)
            => difficulty = Difficulty.Hard;
        
        
    }
}
