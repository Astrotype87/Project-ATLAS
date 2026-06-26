using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using ProjectATLAS.UI;
using ProjectATLAS.Utility;
using static ProjectATLAS.Quiz.CategorizationItem;
using ProjectATLAS.Gameplay;

namespace ProjectATLAS.Quiz.UI
{
    public class QuizPage : UIPage
    {
        [Header("Data")]
        [SerializeField] private string title;
        [SerializeField] private string difficulty;
        
        [Header("Components")]
        [SerializeField] private Button previousButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text difficultyText;
        [SerializeField] private QuizProgressView quizProgressView;
        
        [Header("Item Views")]
        [SerializeField] private MultipleChoiceView multipleChoiceView;
        [SerializeField] private TrueOrFalseView trueOrFalseView;
        [SerializeField] private MatchingView matchingView;
        [SerializeField] private FillInTheBlanksView fillInTheBlanksView;
        [SerializeField] private SequenceView sequenceView;
        [SerializeField] private CategorizationView categorizationView;
        [SerializeField] private SolvingView solvingView;
        
        private int totalItemsCount;
        private int currentItemIndex;
        
        // PROPERTIES
        public event Action OnPreviousClicked;
        public event Action OnNextClicked;
        public event Action<int, QuizAnswer> OnAnswered;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            previousButton.onClick.AddListener(PreviousButton_OnClick);
            nextButton.onClick.AddListener(NextButton_OnClick);
            
            multipleChoiceView.OnAnswerChanged += MultipleChoiceView_OnAnswerChanged;
            trueOrFalseView.OnAnswerChanged += TrueOrFalseView_OnAnswerChanged;
            matchingView.OnAnswerUpdated += MatchingView_OnAnswerChanged;
            fillInTheBlanksView.OnAnswerUpdated += FillInTheBlanksView_OnAnswerChanged;
            sequenceView.OnAnswerUpdated += SequenceView_OnAnswerChanged;
            categorizationView.OnAnswerUpdated += CategorizationView_OnAnswerChanged;
            solvingView.OnAnswerUpdated += SolvingView_OnAnswerChanged;
        }
        
        protected override void OnValidate()
        {
            if (titleText) titleText.text = title;
            if (difficultyText) difficultyText.text = difficulty;
        }
        
        
        // PUBLIC METHODS
        /// <summary> Called once on quiz start. The totalItemsCount is only used by progress bar. </summary>
        public void SetTotalItemsCount(int totalItems)
        {
            totalItemsCount = totalItems;
        }
        
        public void DisplayQuizItem(int itemIndex, QuizItem quizItem, QuizAnswer quizAnswer,
            Difficulty? forcedDifficulty = null, bool hideDifficultyDisplay = false)
        {
            // Update current item index
            currentItemIndex = itemIndex;
            
            // Update progress bar display
            UpdateProgressDisplay();
            
            
            // Hide all quiz views
            HideAllQuizItemViews();
            
            // Update quiz view display
            string itemType = "";
            if (quizItem is MultipleChoiceItem multipleChoiceItem)
            {
                itemType = "Multiple Choice";
                multipleChoiceView.gameObject.SetActive(true);
                multipleChoiceView.DisplayItem(multipleChoiceItem, quizAnswer as MultipleChoiceAnswer);
            }
            else if (quizItem is TrueOrFalseItem trueOrFalseItem)
            {
                itemType = "True Or False";
                trueOrFalseView.gameObject.SetActive(true);
                trueOrFalseView.DisplayItem(trueOrFalseItem, quizAnswer as TrueOrFalseAnswer);
            }
            else if (quizItem is MatchingItem matchingItem)
            {
                itemType = "Matching";
                matchingView.gameObject.SetActive(true);
                matchingView.DisplayItem(matchingItem, quizAnswer as MatchingAnswer);
            }
            else if (quizItem is FillInTheBlanksItem fillInTheBlanksItem)
            {
                itemType = "Fill In The Blanks";
                fillInTheBlanksView.gameObject.SetActive(true);
                fillInTheBlanksView.DisplayItem(fillInTheBlanksItem, quizAnswer as FillInTheBlanksAnswer);
            }
            else if (quizItem is SequenceItem sequenceItem)
            {
                itemType = "Sequence";
                sequenceView.gameObject.SetActive(true);
                sequenceView.DisplayItem(sequenceItem, quizAnswer as SequenceAnswer);
            }
            else if (quizItem is CategorizationItem categorizationItem)
            {
                itemType = "Categorization";
                categorizationView.gameObject.SetActive(true);
                categorizationView.DisplayItem(categorizationItem, quizAnswer as CategorizationAnswer);
            }
            else if (quizItem is SolvingItem solvingItem)
            {
                itemType = "Solving";
                solvingView.gameObject.SetActive(true);
                solvingView.DisplayItem(solvingItem, quizAnswer as SolvingAnswer);
            }
            
            
            // Update title and difficulty text
            titleText.text = $"ITEM {itemIndex + 1} - {itemType}";
            
            string difficultyString = (forcedDifficulty == null ? quizItem.Difficulty : forcedDifficulty.Value).ToString().ToUpper();
            
            difficultyText.text = hideDifficultyDisplay
                ? ""
                : $"{quizItem.GetMaxScore(forcedDifficulty)} pts - {difficultyString}";
        }
        
        
        
        // PRIVATE METHODS
        private void HideAllQuizItemViews()
        {
            multipleChoiceView.gameObject.SetActive(false);
            trueOrFalseView.gameObject.SetActive(false);
            matchingView.gameObject.SetActive(false);
            fillInTheBlanksView.gameObject.SetActive(false);
            sequenceView.gameObject.SetActive(false);
            categorizationView.gameObject.SetActive(false);
            solvingView.gameObject.SetActive(false);
        }
        
        private void UpdateProgressDisplay()
        {
            quizProgressView.UpdateProgressDisplay(currentItemIndex + 1, totalItemsCount);
        }
        
        
        // EVENT LISTENER METHODS
        private void PreviousButton_OnClick() => OnPreviousClicked?.Invoke();
        private void NextButton_OnClick() => OnNextClicked?.Invoke();
        
        private void MultipleChoiceView_OnAnswerChanged(string answer)
        {
            OnAnswered?.Invoke(currentItemIndex, new MultipleChoiceAnswer(answer));
        }
        
        private void TrueOrFalseView_OnAnswerChanged(bool answer)
        {
            OnAnswered?.Invoke(currentItemIndex, new TrueOrFalseAnswer(answer.AsBoolEnum()));
        }
        
        private void MatchingView_OnAnswerChanged(string[] answer)
        {
            OnAnswered?.Invoke(currentItemIndex, new MatchingAnswer(answer));
        }
        
        private void FillInTheBlanksView_OnAnswerChanged(string[] answer)
        {
            OnAnswered?.Invoke(currentItemIndex, new FillInTheBlanksAnswer(answer));
        }
        
        private void SequenceView_OnAnswerChanged(string[] answer)
        {
            OnAnswered?.Invoke(currentItemIndex, new SequenceAnswer(answer));
        }
        
        private void CategorizationView_OnAnswerChanged(CategoryItems[] answer)
        {
            OnAnswered?.Invoke(currentItemIndex, new CategorizationAnswer(answer));
        }
        
        private void SolvingView_OnAnswerChanged(double answer)
        {
            OnAnswered?.Invoke(currentItemIndex, new SolvingAnswer(answer));
        }
    }
}
