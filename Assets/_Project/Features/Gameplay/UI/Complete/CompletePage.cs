using System;
using UnityEngine;
using UnityEngine.UI;
using KBCore.Refs;

using ProjectATLAS.UI;
using System.Collections.Generic;
using TMPro;
using FewClicksDev.Core;

namespace ProjectATLAS.Gameplay.UI
{
    public class CompletePage : UIPage
    {
        [Header("Data")]
        [SerializeField] private string levelName;
        [SerializeField] private bool isCompleted = true;
        [SerializeField] private bool bronze;
        [SerializeField] private bool silver;
        [SerializeField] private bool gold;
        [SerializeField] private string bronzeObjective;
        [SerializeField] private string silverObjective;
        [SerializeField] private string goldObjective;
        [SerializeField] private float goldTime;
        
        [Header("Style")]
        [SerializeField] private Color normalBgColor;
        [SerializeField] private Color normalTextColor;
        [SerializeField] private Color highlightedBgColor;
        [SerializeField] private Color highlightedTextColor;
        
        [Header("Components")]
        [SerializeField] private TMP_Text completeText;
        [SerializeField] private RectTransform unlockItemViewContainer;
        [SerializeField, Child] private List<UnlockedItemView> unlockedItemViews;
        [SerializeField] private RectTransform medalsPanel;
        [SerializeField] private Image bronzeImage;
        [SerializeField] private Image silverImage;
        [SerializeField] private Image goldImage;
        [SerializeField] private TMP_Text bronzeText;
        [SerializeField] private TMP_Text silverText;
        [SerializeField] private TMP_Text goldText;
        [SerializeField] private Button menuButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button campaignButton;
        
        [Header("Reward Icons")]
        [SerializeField] private Sprite preTestIcon;
        [SerializeField] private Sprite lessonIcon;
        [SerializeField] private Sprite simulationIcon;
        [SerializeField] private Sprite challengeIcon;
        [SerializeField] private Sprite postTestIcon;
        [SerializeField] private Sprite guidebookIcon;
        [SerializeField] private Sprite glossaryIcon;
        [SerializeField] private Sprite quizTopicIcon;
        [SerializeField] private Sprite chapterIcon;
        
        
        // PROPERTIES
        public event Action<CompleteAction> OnCompleteAction;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            menuButton.onClick.AddListener(MenuButton_onClick);
            restartButton.onClick.AddListener(RestartButton_onClick);
            campaignButton.onClick.AddListener(CampaignButton_onClick);
        }
        
        protected override void OnValidate()
        {
            DisplayCompleted(levelName, isCompleted);
            DisplayMedalsObtained(bronze, silver, gold);
            DisplayMedalsObjectives(bronzeObjective, silverObjective, goldObjective, goldTime);
        }
        
        
        // PUBLIC METHODS
        public void DisplayCompleted(string levelName, bool isCompleted)
        {
            this.levelName = levelName;
            this.isCompleted = isCompleted;
            
            completeText.text = $"{levelName} {(isCompleted ? "Complete" : "Failed")}";
            
            if (isCompleted)
            {
                SetButtonStyle(restartButton, normalBgColor, normalTextColor);
                SetButtonStyle(campaignButton, highlightedBgColor, highlightedTextColor);
            }
            else
            {
                SetButtonStyle(restartButton, highlightedBgColor, highlightedTextColor);
                SetButtonStyle(campaignButton, normalBgColor, normalTextColor);
            }
            
            static void SetButtonStyle(Button button, Color bgColor, Color textColor)
            {
                if (button.TryGetComponent(out Image image))
                    image.color = bgColor;

                TMP_Text text = button.GetComponentInChildren<TMP_Text>();
                if (text) text.color = textColor;
            }
        }
        
        public void DisplayUnlocks(UnlockData[] unlockDatas)
        {
            if (unlockDatas == null)
            {
                // Hide all unlock item rows
                for (int i = 0; i < unlockedItemViews.Count; i++)
                {
                    unlockedItemViews[i].gameObject.SetActive(false);
                }
                
                return;
            }
            
            // Handle review item view duplication logic
            int targetChildCount = Mathf.FloorToInt(unlockDatas.Length);
            int interval = targetChildCount - unlockedItemViews.Count;
            
            if (interval > 0)
            {
                for (int i = 0; i < interval; i++)
                {
                    AddUnlockedItemView();
                }
            }
            else if (interval < 0)
            {
                interval = -interval; // flip the sign
                for (int i = 0; i < interval; i++)
                {
                    RemoveUnlockedItemView();
                }
            }
            
            // Update each review item view
            for (int i = 0; i < unlockDatas.Length; i++)
            {
                Sprite icon = unlockDatas[i].icon switch
                {
                    UnlockIcon.PreTest => preTestIcon,
                    UnlockIcon.Lesson => lessonIcon,
                    UnlockIcon.Simulation => simulationIcon,
                    UnlockIcon.Challenge => challengeIcon,
                    UnlockIcon.PostTest => postTestIcon,
                    UnlockIcon.Guidebook => guidebookIcon,
                    UnlockIcon.Glossary => glossaryIcon,
                    UnlockIcon.QuizTopic => quizTopicIcon,
                    UnlockIcon.Chapter => chapterIcon,
                    _ => chapterIcon
                };
                
                unlockedItemViews[i].DisplayUnlockedItem(icon, unlockDatas[i].name, unlockDatas[i].description);
            }
            
            // If 0, hide the only one
            if (unlockDatas.Length <= 0)
            {
                // Hide all unlock item rows
                for (int i = 0; i < unlockedItemViews.Count; i++)
                {
                    unlockedItemViews[i].gameObject.SetActive(false);
                }
                
            }
        }
        
        public void SetMedalsVisible(bool visible)
        {
            medalsPanel.gameObject.SetActive(visible);
        }
        
        public void DisplayMedalsObtained(bool bronze, bool silver, bool gold)
        {
            this.bronze = bronze;
            this.silver = silver;
            this.gold = gold;
            
            if (bronzeImage) bronzeImage.gameObject.SetActive(bronze);
            if (silverImage) silverImage.gameObject.SetActive(silver);
            if (goldImage) goldImage.gameObject.SetActive(gold);
            
            if (bronzeText) SetAlpha(bronzeText, bronze ? 0.8f : 0.2f);
            if (silverText) SetAlpha(silverText, silver ? 0.8f : 0.2f);
            if (goldText) SetAlpha(goldText, gold ? 0.8f : 0.2f);
            
            static void SetAlpha(TMP_Text textComponent, float alpha)
            {
                Color color = textComponent.color;
                color.a = alpha;
                textComponent.color = color;
            }
        }
        
        public void DisplayMedalsObjectives(string bronzeObjective, string silverObjective, string goldObjective, float goldTime)
        {
            this.goldTime = goldTime;
            this.bronzeObjective = bronzeObjective;
            this.silverObjective = silverObjective;
            this.goldObjective = goldObjective;
            
            if (bronzeText) bronzeText.text = bronzeObjective.Replace("{goldTime}", goldTime.ToString());
            if (silverText) silverText.text = silverObjective.Replace("{goldTime}", goldTime.ToString());
            if (goldText) goldText.text = goldObjective.Replace("{goldTime}", goldTime.ToString());
        }
        
        // PRIVATE METHODS
        private void AddUnlockedItemView()
        {
            UnlockedItemView template = unlockedItemViews[0];
            GameObject newGameObject = Instantiate(template.gameObject, unlockItemViewContainer);
            
            if (newGameObject.TryGetComponent(out UnlockedItemView unlockedItemView))
            {
                unlockedItemViews.Add(unlockedItemView);
            }
        }
        
        private void RemoveUnlockedItemView()
        {
            int lastIndex = unlockedItemViews.Count - 1;
            if (lastIndex <= 0) return;
            
            UnlockedItemView viewToRemove = unlockedItemViews[lastIndex];
            
            unlockedItemViews.Remove(viewToRemove);
            
            Destroy(viewToRemove.gameObject);
        }
        
        
        // EVENT LISTENER METHODS
        private void MenuButton_onClick() => OnCompleteAction?.Invoke(CompleteAction.Menu);
        private void RestartButton_onClick() => OnCompleteAction?.Invoke(CompleteAction.Restart);
        private void CampaignButton_onClick() => OnCompleteAction?.Invoke(CompleteAction.Campaign);
    }
}
