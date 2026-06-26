using System;
using UnityEngine;
using UnityEngine.UI;
using KBCore.Refs;

using ProjectATLAS.UI;
using ProjectATLAS.Gameplay;
using ProjectATLAS.Quiz;
using Range = ProjectATLAS.Types.Range;
using System.Collections.Generic;
using System.Linq;
using TMPro;

namespace ProjectATLAS.Menu
{
    public class SelectQuizPage : UIPage
    {
        [Header("Quiz Data")]
        [SerializeField] private PostTestData[] quizTopics;
        
        [Header("Selections")]
        [SerializeField] private int selectedTopicIndex = -1; // -1 = All
        [SerializeField] private int numberOfItems = 5;
        [SerializeField] private int minItems = 1;
        [SerializeField] private int maxItems = 10;
        [SerializeField] private ItemTypes selectedItemTypes;
        [SerializeField] private Difficulty difficulty;
        
        [Header("Components")]
        [SerializeField] private Button lastTopicButton;
        [SerializeField] private Button nextTopicButton;
        [SerializeField] private TMP_Text topicText;
        [SerializeField] private Button decreaseItemsButton;
        [SerializeField] private Button increaseItemsButton;
        [SerializeField] private TMP_Text itemsText;
        
        [Header("Toggles")]
        [SerializeField] private UIToggleButton multipleChoiceToggle;
        [SerializeField] private UIToggleButton trueOrFalseToggle;
        [SerializeField] private UIToggleButton matchingToggle;
        [SerializeField] private UIToggleButton solvingToggle;
        [SerializeField] private UIToggleButton fillInTheBlanksToggle;
        [SerializeField] private UIToggleButton sequenceToggle;
        [SerializeField] private UIToggleButton categorizationToggle;
        
        [Header("Difficulty")]
        [SerializeField] private UIToggleButton easyToggle;
        [SerializeField] private UIToggleButton mediumToggle;
        [SerializeField] private UIToggleButton hardToggle;
        
        [SerializeField] private Button playButton;
        
        
        // PROPERTIES3
        public event Action<CustomQuizSettings, Difficulty> OnStartClicked;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            lastTopicButton.onClick.AddListener(LastTopicButton_onClick);
            nextTopicButton.onClick.AddListener(NextTopicButton_onClick);
            decreaseItemsButton.onClick.AddListener(DecreaseItemsButton_onClick);
            increaseItemsButton.onClick.AddListener(IncreaseItemsButton_onClick);
            
            multipleChoiceToggle.OnValueChanged += MultipleChoiceToggle_OnValueChanged;
            trueOrFalseToggle.OnValueChanged += TrueOrFalseToggle_OnValueChanged;
            matchingToggle.OnValueChanged += MatchingToggle_OnValueChanged;
            solvingToggle.OnValueChanged += SolvingToggle_OnValueChanged;
            fillInTheBlanksToggle.OnValueChanged += FillInTheBlanksToggle_OnValueChanged;
            sequenceToggle.OnValueChanged += SequenceToggle_OnValueChanged;
            categorizationToggle.OnValueChanged += CategorizationToggle_OnValueChanged;
            
            easyToggle.OnValueChanged += EasyToggle_OnValueChanged;
            mediumToggle.OnValueChanged += MediumToggle_OnValueChanged;
            hardToggle.OnValueChanged += HardToggle_OnValueChanged;
            
            playButton.onClick.AddListener(PlayButton_onClick);
        }
        
        // UIPage METHODS
        public override void OpenPage()
        {
            base.OpenPage();
            
            UpdateSelectedTopic(selectedTopicIndex);
            UpdateNumberOfItems(numberOfItems);
        }
        
        
        // PRIVATE METHODS
        private void UpdateSelectedTopic(int topicIndex)
        {
            int maxTopicIndex = quizTopics.Length == 0 ? -1 : quizTopics.Length;
            selectedTopicIndex = Mathf.Clamp(topicIndex, -1, maxTopicIndex - 1);
            
            if (topicText) topicText.text =
                selectedTopicIndex < 0
                    ? "All"
                    : quizTopics[selectedTopicIndex].Title;
        }
        
        private void UpdateNumberOfItems(int itemCount)
        {
            numberOfItems = Mathf.Clamp(itemCount, minItems, maxItems);
            
            if (itemsText) itemsText.text = numberOfItems.ToString();
        }
        
        
        
        // EVENT LISTENER METHODS
        private void LastTopicButton_onClick()
            => UpdateSelectedTopic(selectedTopicIndex - 1);
        private void NextTopicButton_onClick()
            => UpdateSelectedTopic(selectedTopicIndex + 1);
        
        private void DecreaseItemsButton_onClick()
            => UpdateNumberOfItems(numberOfItems - 1);
        private void IncreaseItemsButton_onClick()
            => UpdateNumberOfItems(numberOfItems + 1);
        
        private void MultipleChoiceToggle_OnValueChanged(UIToggleButton toggleButton, bool value)
            => UpdateFlag(ItemTypes.MultipleChoice, value);
        private void TrueOrFalseToggle_OnValueChanged(UIToggleButton toggleButton, bool value)
            => UpdateFlag(ItemTypes.TrueOrFalse, value);
        private void MatchingToggle_OnValueChanged(UIToggleButton toggleButton, bool value)
            => UpdateFlag(ItemTypes.Matching, value);
        private void SolvingToggle_OnValueChanged(UIToggleButton toggleButton, bool value)
            => UpdateFlag(ItemTypes.Solving, value);
        private void FillInTheBlanksToggle_OnValueChanged(UIToggleButton toggleButton, bool value)
            => UpdateFlag(ItemTypes.FillInTheBlanks, value);
        private void SequenceToggle_OnValueChanged(UIToggleButton toggleButton, bool value)
            => UpdateFlag(ItemTypes.Sequence, value);
        private void CategorizationToggle_OnValueChanged(UIToggleButton toggleButton, bool value)
            => UpdateFlag(ItemTypes.Categorization, value);
        
        private void UpdateFlag(ItemTypes flag, bool add)
        {
            selectedItemTypes = add ? selectedItemTypes |= flag : selectedItemTypes &= ~flag;
        }
        
        private void EasyToggle_OnValueChanged(UIToggleButton toggleButton, bool value)
            => difficulty = Difficulty.Easy;
        private void MediumToggle_OnValueChanged(UIToggleButton toggleButton, bool value)
            => difficulty = Difficulty.Medium;
        private void HardToggle_OnValueChanged(UIToggleButton toggleButton, bool value)
            => difficulty = Difficulty.Hard;
        
        
        private void PlayButton_onClick()
        {
            List<QuizData> selectedQuizDatas = new();
            
            // Get references to selected QuizData from selected topics
            if (selectedTopicIndex < 0)
            {
                foreach (var quizTopic in quizTopics)
                {
                    foreach (var quizData in quizTopic.ChapterQuizData)
                    {
                        selectedQuizDatas.Add(quizData);
                    }
                }
            }
            else
            {
                foreach (var quizData in quizTopics[selectedTopicIndex].ChapterQuizData)
                {
                    selectedQuizDatas.Add(quizData);
                }
            }
            
            CustomQuizSettings customQuizSettings = new()
            {
                quizDatas = selectedQuizDatas.ToArray(),
                itemCount = numberOfItems,
                itemTypes = selectedItemTypes,
                topicName = selectedTopicIndex < 0 ? "All" : quizTopics[selectedTopicIndex].Title
            };
            
            OnStartClicked?.Invoke(customQuizSettings, difficulty);
        }
    }
}
