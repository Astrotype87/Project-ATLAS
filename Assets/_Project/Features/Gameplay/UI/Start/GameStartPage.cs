using UnityEngine;
using UnityEngine.UI;
using TMPro;

using ProjectATLAS.UI;
using System;
using ProjectATLAS.GameData;

namespace ProjectATLAS.Gameplay.UI
{
    public class GameStartPage : UIPage
    {
        [Header("Details")]
        [SerializeField] private string title;
        [SerializeField] private string subtitle;
        [SerializeField, TextArea(5, 10)] private string objectives;
        [SerializeField] private string bronze;
        [SerializeField] private string silver;
        [SerializeField] private string gold;
        [SerializeField, TextArea(5, 10)] private string instructions;
        [SerializeField] private bool hideMedals;
        [SerializeField] private string buttonText;
        
        [Header("Components")]
        [SerializeField] private TMP_Text titleComponent;
        [SerializeField] private TMP_Text subtitleComponent;
        [SerializeField] private TMP_Text objectivesComponent;
        [SerializeField] private TMP_Text bronzeComponent;
        [SerializeField] private TMP_Text silverComponent;
        [SerializeField] private TMP_Text goldComponent;
        [SerializeField] private TMP_Text instructionsComponent;
        [SerializeField] private CanvasGroup medalsGroup;
        [SerializeField] private TMP_Text buttonTextComponent;
        [SerializeField] private Button startButton;
        
        // PRIVATE FIELDS
        private GameDataManager gameDataService;
        private string playerName;
        
        // PROPERTIES
        public Button StartButton => startButton;
        
        public event Action OnStartClicked;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            startButton.onClick.AddListener(StartButton_onClick);
            
            gameDataService = GameDataManager.Instance;
            
            if (!gameDataService) gameDataService = GameDataManager.Instance;
            
            playerName = gameDataService ? gameDataService.DetailsData.displayName : "Player";
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();
            
            SetTitle(title);
            SetSubtitle(subtitle);
            SetObjectives(objectives);
            SetBronze(bronze);
            SetSilver(silver);
            SetGold(gold);
            SetInstructions(instructions);
            SetHideMedals(hideMedals);
            SetButtonText(buttonText);
        }
        
        
        // PUBLIC METHODS
        public void SetTitle(string title)
        {
            this.title = title;
            if (titleComponent != null)
                titleComponent.text = title;
        }
        
        public void SetSubtitle(string subtitle)
        {
            this.subtitle = subtitle;
            if (subtitleComponent != null) subtitleComponent.text = subtitle;
        }
        
        public void SetObjectives(string objectives)
        {
            this.objectives = objectives;
            
            string objectivesString = objectives.Replace("{player}", playerName);
            
            if (objectivesComponent != null) objectivesComponent.text = objectivesString;
        }
        
        public void SetBronze(string bronze)
        {
            this.bronze = bronze;
            if (bronzeComponent != null) bronzeComponent.text = bronze;
        }
        
        public void SetSilver(string silver)
        {
            this.silver = silver;
            if (silverComponent != null) silverComponent.text = silver;
        }
        
        public void SetGold(string gold)
        {
            this.gold = gold;
            if (goldComponent != null) goldComponent.text = gold;
        }
        
        public void SetInstructions(string instructions)
        {
            this.instructions = instructions;
            
            if (!gameDataService) gameDataService = GameDataManager.Instance;
            
            string playerName = gameDataService ? gameDataService.DetailsData.displayName : "Player";
            string instructionsString = instructions.Replace("{player}", playerName);
            
            if (instructionsComponent != null) instructionsComponent.text = instructionsString;
        }
        
        public void SetHideMedals(bool hide)
        {
            this.hideMedals = hide;
            if (medalsGroup != null) medalsGroup.alpha = hide ? 0f : 1f;
        }
        
        public void SetButtonText(string buttonText)
        {
            this.buttonText = buttonText;
            if (buttonTextComponent != null) buttonTextComponent.text = buttonText;
        }
        
        
        // EVENT LISTENER METHODS
        private void StartButton_onClick()
        {
            OnStartClicked?.Invoke();
        }
        
        
    }
}
