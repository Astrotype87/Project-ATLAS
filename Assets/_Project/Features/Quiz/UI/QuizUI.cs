using System;
using System.Collections.Generic;
using UnityEngine;

using ProjectATLAS.Gameplay;
using TMPro;
using ProjectATLAS.Gameplay.UI;

namespace ProjectATLAS.Quiz.UI
{
    public class QuizUI : MonoBehaviour
    {
        [SerializeField] private GameStartPage gameStartPage;
        [SerializeField] private QuizPage quizPage;
        [SerializeField] private ReviewPage reviewPage;
        [SerializeField] private GameResultPage gameResultPage;
        [SerializeField] private ResultListPanel resultListPanel;
        [SerializeField] private TMP_Text[] timerTexts;
        
        
        // PROPERTIES
        public event Action OnStartClicked;
        public event Action OnPreviousClicked;
        public event Action OnNextClicked;
        public event Action<int, QuizAnswer> OnAnswered;
        public event Action<int> OnEditClicked;
        public event Action OnFinishClicked;
        public event Action OnExitClicked;
        
        private bool hideDifficultyDisplay;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            gameStartPage.OnStartClicked += OnStartClicked;
            
            quizPage.OnPreviousClicked += OnPreviousClicked;
            quizPage.OnNextClicked += OnNextClicked;
            quizPage.OnAnswered += OnAnswered;
            
            reviewPage.OnEditClicked += OnEditClicked;
            reviewPage.OnFinishClicked += OnFinishClicked;
            
            // Result page "Next" button will be treated as "quiz exit"
            gameResultPage.OnNextClicked += OnExitClicked;
        }
        
        
        // PUBLIC METHODS
        public void HideDifficultyDisplay(bool hideDifficultyDisplay)
        {
            this.hideDifficultyDisplay = hideDifficultyDisplay;
        }
        
        
        public void DisplayStartPage(QuizData quizData, List<QuizItem> quizItems, bool isTestMode)
        {
            gameStartPage.OpenPageInGroup();
            
            gameStartPage.SetTitle(quizData.Level);
            gameStartPage.SetSubtitle(quizData.Name);
            gameStartPage.SetObjectives(quizData.Description);
            
            gameStartPage.SetHideMedals(isTestMode);
            
            string instructions =
                $"Complete the quiz within a passing score of 50%."
                + $"\n\n{quizItems.Count} items"
                + $"\n{QuizData.GetQuizDetails(quizItems)}";
            gameStartPage.SetInstructions(instructions);
            
            // gameStartPage.DisplayQuizInfo(quizData, quizItems);
        }
        
        public void SetQuizInfo(string level, string name, string description, string objectives, List<QuizItem> quizItems, bool isTestMode)
        {
            gameStartPage.SetTitle(level);
            gameStartPage.SetSubtitle(name);
            gameStartPage.SetObjectives(description);
            
            gameStartPage.SetHideMedals(isTestMode);
            
            string instructions =
                $"{objectives}"
                + $"\n\n{quizItems.Count} items"
                + $"\n{QuizData.GetQuizDetails(quizItems)}";
            gameStartPage.SetInstructions(instructions);
            
            // gameStartPage.DisplayQuizInfo(level, name, description, quizItems);
        }
        
        public void SetMedalsInfo(string bronze, string silver, string gold)
        {
            gameStartPage.SetBronze(bronze);
            gameStartPage.SetSilver(silver);
            gameStartPage.SetGold(gold);
        }
        
        
        public void DisplayQuizPage()
        {
            quizPage.OpenPageInGroup();
        }
        
        public void SetTotalItemsCount(int totalItems)
        {
            quizPage.SetTotalItemsCount(totalItems);
        }
        
        
        public void DisplayQuizItem(int itemIndex, QuizItem quizItem, QuizAnswer quizAnswer, Difficulty? forcedDifficulty = null)
        {
            quizPage.DisplayQuizItem(itemIndex, quizItem, quizAnswer, forcedDifficulty, hideDifficultyDisplay);
        }
        
        
        public void DisplayReviewPage(QuizItem[] quizItems, QuizAnswer[] quizAnswers, Difficulty? forcedDifficulty = null)
        {
            var items = new (string, string, string)[quizItems.Length];
            for (int i = 0; i < items.Length; i++)
            {
                items[i].Item1 = quizItems[i].GetQuestionAsString();
                
                float score = quizItems[i].GetMaxScore(forcedDifficulty);
                string pointText = score > 1f ? "POINTS" : "POINT";
                string difficultyText = (forcedDifficulty != null ? forcedDifficulty.Value : quizItems[i].Difficulty).ToString().ToUpper();
                
                items[i].Item2 = hideDifficultyDisplay
                    ? $"{score} {pointText}"
                    : $"{score} {pointText}\n{difficultyText}";
                
                items[i].Item3 = quizAnswers[i] == null ? "(No Answer)" : quizAnswers[i].ToString();
            }
            
            reviewPage.OpenPageInGroup();
            reviewPage.UpdateReviewItemsList(items, hideDifficultyDisplay);
        }
        
        
        public void DisplayQuizResults(string quizName, QuizResultData resultData, PointsEntry[] pointsEntries)
        {
            // resultPage.OpenPageInGroup();
            // resultPage.DisplayResults(resultData, pointsEntries);
            
            gameResultPage.OpenPageInGroup();
            
            gameResultPage.SetSubtitle(quizName);
            
            gameResultPage.SetScore((int)resultData.score, (int)resultData.maxScore);
            gameResultPage.SetPlayTime((float)resultData.time.TotalSeconds);
            gameResultPage.SetPointsEntries(pointsEntries);
            
            resultListPanel.DisplayResultData(resultData);
            
            // DETAILS TO DISPLAY IN RESULTS PAGE
            // - total quiz points
            // - quiz time
            // - result quiz item answer list??
            //     - isCorrect, question, points, answer, correct
            
            // 
            // - score items list
            //     - name, score
            // - total score
        }
        
        public void HideScoreAndTimeBonusPanel(bool hide)
        {
            gameResultPage.HideScoreAndTimeBonusPanel(hide);
        }
        
        
        public void UpdateTimer(float time)
        {
            foreach (var timerText in timerTexts)
            {
                if (timerText)
                {
                    int totalMinutes = Mathf.FloorToInt(time / 60f);
                    int seconds = Mathf.FloorToInt(time % 60f);
                    timerText.text = $"{totalMinutes:00}:{seconds:00}";
                }
            }
        }
        
        
        // EVENT LISTENER METHODS
        public void StartPage_OnStartClicked() => OnStartClicked?.Invoke();
        public void QuizPage_OnPreviousClicked() => OnPreviousClicked?.Invoke();
        public void QuizPage_OnNextClicked() => OnNextClicked?.Invoke();
        public void QuizPage_OnAnswered(int itemIndex, QuizAnswer answer) => OnAnswered?.Invoke(itemIndex, answer);
        public void ReviewPage_OnEditClicked(int itemIndex) => OnEditClicked?.Invoke(itemIndex);
        public void ReviewPage_OnFinishClicked() => OnFinishClicked?.Invoke();
    }
}
