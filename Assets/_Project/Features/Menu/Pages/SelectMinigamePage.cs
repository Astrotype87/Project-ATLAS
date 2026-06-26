using System;
using UnityEngine;
using UnityEngine.UI;
using KBCore.Refs;

using ProjectATLAS.UI;
using ProjectATLAS.Gameplay;

namespace ProjectATLAS.Menu
{
    public class SelectMinigamePage : UIPage
    {
        [SerializeField] private Difficulty difficulty;
        [SerializeField, Child] private MinigameButton[] minigameButtons;
        [SerializeField] private UIToggleButton easyToggle;
        [SerializeField] private UIToggleButton mediumToggle;
        [SerializeField] private UIToggleButton hardToggle;
        
        [SerializeField] private Button playButton;
        
        
        // PROPERTIES3
        public event Action<ChallengeLevelData, Difficulty> OnStartClicked;
        
        
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
            foreach (var minigameButton in minigameButtons)
            {
                bool isCompleted = LevelManager.Instance.IsLevelCompleted(minigameButton.LevelID);
                minigameButton.UpdateUI(!isCompleted);
            }
        }
        
        
        private void PlayButton_onClick()
        {
            foreach (var minigameButton in minigameButtons)
            {
                if (minigameButton.IsSelected)
                    OnStartClicked?.Invoke(minigameButton.ChallengeLevelData, difficulty);
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
