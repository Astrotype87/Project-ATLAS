using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AstrotypeTools.ClassSelector;
using AstrotypeTools.InspectorAttributes;

using ProjectATLAS.Gameplay;
using ProjectATLAS.Quiz.UI;

namespace ProjectATLAS.Quiz
{
    public class QuizManager : MonoBehaviour
    {
        public enum QuizMode { Normal, PreTest, PostTest, Custom }
        
        [Header("Startup")]
        [SerializeField] private bool autoStart;
        [SerializeField] private QuizData[] quizDatas;
        [SerializeField] private Difficulty difficulty;
        [SerializeField] private QuizMode mode;
        [SerializeField] private float scoreMultiplier = 100;
        [SerializeField] private TimeBonus timeBonus = new() {
            timeBonuses = new() {
                new(10, 300),
                new(15, 200),
                new(30, 100),
                new(45, 50),
                new(60, 25),
                new(90, 15)
            },
            lowestScore = 10
        };
        
        [Header("Settings")]
        [SerializeField] private QuizSettings easySettings;
        [SerializeField] private QuizSettings mediumSettings;
        [SerializeField] private QuizSettings hardSettings;
        [SerializeField] private QuizSettings preTestSettings;
        [SerializeField] private QuizSettings postTestSettings;
        [SerializeField] private QuizSettings customSettings;
        
        [Header("References")]
        [SerializeField] private QuizUI quizUI;
        
        [Header("Debug")]
        [ReadOnly] [SerializeField] private int currentItemIndex;
        [ReadOnly] [SerializeField] private QuizSettings quizSettings;
        [ReadOnly] [SerializeReference] private List<QuizItem> quizItems;
        [ReadOnly] [SerializeReference, ClassSelector] private QuizAnswer[] quizAnswers;
        
        // PRIVATE FIELDS
        private string quizName; // To be displayed in game start page and game result page
        
        private bool isQuizRunning;
        private DateTime startTime;
        private DateTime endTime;
        
        private bool isQuizPaused;
        private TimeSpan pausedDuration;  // total accumulated time already played before pause
        private DateTime pauseStartTime;  // when pause began
        
        private QuizResultData resultData;
        private PointsEntry[] pointsEntries;
        
        // PROPERTIES
        public bool IsQuizEnded { get; private set; }
        public QuizResultData ResultData => resultData;
        public PointsEntry[] PointsEntries => pointsEntries;
        
        public event Action OnQuizEnded;
        
        private bool IsTestMode => mode is QuizMode.PreTest or QuizMode.PostTest;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            quizUI.OnStartClicked += QuizUI_OnStartClicked;
            quizUI.OnPreviousClicked += QuizUI_OnPreviousClicked;
            quizUI.OnNextClicked += QuizUI_OnNextClicked;
            quizUI.OnAnswered += QuizUI_OnAnswered;
            quizUI.OnEditClicked += QuizUI_OnEditClicked;
            quizUI.OnFinishClicked += QuizUI_OnFinishQuiz;
            quizUI.OnExitClicked += QuizUI_OnExitClicked;
        }
        
        private void Start()
        {
            if (autoStart) InitializeQuiz();
        }
        
        private void Update()
        {
            if (isQuizRunning)
            {
                float time = (float)GetCurrentDuration().TotalSeconds;
                quizUI.UpdateTimer(time);
            }
        }
        
        
        // PUBLIC METHODS
        public void SetQuizSettings(QuizData quizData, Difficulty difficulty, TimeBonus timeBonus = null)
        {
            this.quizDatas = new[] {quizData};
            this.difficulty = difficulty;
            this.mode = QuizMode.Normal;
            
            if (timeBonus != null) this.timeBonus = timeBonus;
        }
        
        public void SetPreTestQuiz(QuizData[] quizDatas)
        {
            this.quizDatas = quizDatas;
            this.difficulty = Difficulty.Easy;
            this.mode = QuizMode.PreTest;
        }
        
        public void SetPostTestQuiz(QuizData[] quizDatas)
        {
            this.quizDatas = quizDatas;
            this.difficulty = Difficulty.Medium;
            this.mode = QuizMode.PostTest;
        }
        
        public void SetCustomQuiz(CustomQuizSettings customQuizSettings, Difficulty difficulty)
        {
            this.quizDatas = customQuizSettings.quizDatas;
            this.difficulty = difficulty;
            this.mode = QuizMode.Custom;
            
            this.customSettings.totalItems = customQuizSettings.itemCount;
            this.customSettings.shuffled = true;
            this.customSettings.itemTypes = customQuizSettings.itemTypes;
        }
        
        public void SetQuizInfo(string level, string name, string description, string objectives)
        {
            quizName = $"{level} - {name}";
            quizUI.SetQuizInfo(level, name, description, objectives, quizItems, IsTestMode);
        }
        
        public void SetMedalsInfo(string bronze, string silver, string gold)
        {
            quizUI.SetMedalsInfo(bronze, silver, gold);
        }
        
        
        // BUG: ADD TIME BONUS from GameplayManager
        public void InitializeQuiz()
        {
            // Get quiz settings
            quizSettings = mode switch
            {
                QuizMode.Normal => difficulty switch
                {
                    Difficulty.Easy => easySettings,
                    Difficulty.Medium => mediumSettings,
                    Difficulty.Hard => hardSettings,
                    _ => easySettings
                },
                QuizMode.PreTest => preTestSettings,
                QuizMode.PostTest => postTestSettings,
                QuizMode.Custom => customSettings,
                _ => quizSettings
            };
            
            // Initialize quiz items
            quizItems = QuizData.GenerateQuizItems(QuizData.CombineQuizItems(quizDatas), quizSettings, difficulty);
            
            // Initialize quiz answer data
            int totalItemsCount = quizItems.Count;
            quizAnswers = new QuizAnswer[totalItemsCount];
            quizUI.SetTotalItemsCount(totalItemsCount);
            
            // Display quiz page (second page)
            bool hideDifficultyDisplay = mode is QuizMode.PreTest or QuizMode.PostTest;
            quizUI.HideDifficultyDisplay(hideDifficultyDisplay);
            quizUI.DisplayStartPage(quizDatas[0], quizItems, IsTestMode);
        }
        
        public void StartQuiz()
        {
            // Display quiz page (second page)
            quizUI.DisplayQuizPage();
            
            // Display first quiz item
            currentItemIndex = 0;
            DisplayQuizItem(currentItemIndex);
            
            // Start timer
            isQuizRunning = true;
            StartTimer();
        }
        
        public void PauseQuiz()
        {
            PauseTimer();
        }
        
        public void ResumeQuiz()
        {
            ResumeTimer();
        }
        
        public void FinishQuiz()
        {
            // Scoring difficulty
            Difficulty scoringDifficulty = mode is QuizMode.PreTest or QuizMode.PostTest
                ? Difficulty.Easy : difficulty;
            
            
            // End quiz timer
            EndTimer();
            isQuizRunning = false;
            
            // Calculate total quiz score
            // Display results and score to results page
            
            int itemCount = quizItems.Count;
            
            // Create result data
            resultData = new();
            resultData.itemResults = new QuizResultData.ItemResult[itemCount];
            resultData.score = CalculateScore(scoringDifficulty);
            resultData.time = GetPlayTime();
            resultData.maxScore = QuizData.GetTotalScore(quizItems, scoringDifficulty);
            
            QuizItem quizItem;
            QuizAnswer quizAnswer;
            for (int i = 0; i < itemCount; i++)
            {
                quizItem = quizItems[i];
                quizAnswer = quizAnswers[i];
                
                float score = quizItem.CheckAnswerAndGetScore(quizAnswer, scoringDifficulty);
                float max = quizItem.GetMaxScore(scoringDifficulty);
                
                QuizResultData.ItemResult itemResult = new()
                {
                    isCorrect = score == quizItem.GetMaxScore(scoringDifficulty),
                    points = $"{score}/{max} PTS\n",
                    difficulty = quizItem.Difficulty.ToString().ToUpper(),
                    question = quizItem.GetQuestionAsString(),
                    answer = quizAnswer == null ? "(No Answer)" : quizAnswer.ToString(),
                    correct = quizItem.GetAnswerAsString()
                };
                
                resultData.itemResults[i] = itemResult;
            }
            
            // Create points data
            int quizPoints = (int)(resultData.score * scoreMultiplier);
            int timeBonus = (int)(CalculateTimeBonus(resultData.time));
            
            pointsEntries = new PointsEntry[] {
                new("Quiz", quizPoints),
                new("Time Bonus", timeBonus)
            };
            
            // Display quiz results
            bool HideScoreAndTimeBonusPanel = mode is QuizMode.PreTest or QuizMode.PostTest;
            quizUI.HideScoreAndTimeBonusPanel(HideScoreAndTimeBonusPanel);
            quizUI.DisplayQuizResults(quizName, resultData, pointsEntries);
        }
        
        public void CloseQuiz()
        {
            IsQuizEnded = true;
            OnQuizEnded?.Invoke();
        }
        
        
        
        /// <summary>
        /// Get passing score percentage from QuizManager quiz setting
        /// based on current mode (PreTest or PostTest) or difficulty.
        /// </summary>
        public float GetPassingScorePercentage()
        {
            return quizSettings.passingScorePercentage;
        }
        
        public IEnumerator WaitForQuizEnd()
        {
            yield return new WaitUntil(() => IsQuizEnded);
        }
        
        
        
        // EVENT LISTENER METHODS
        private void QuizUI_OnStartClicked()
        {
            StartQuiz();
        }
        
        private void QuizUI_OnPreviousClicked()
        {
            if (currentItemIndex <= 0) return;
            
            currentItemIndex--;
            DisplayQuizItem(currentItemIndex);
        }
        
        private void QuizUI_OnNextClicked()
        {
            if (currentItemIndex >= quizItems.Count - 1)
            {
                quizUI.DisplayReviewPage(quizItems.ToArray(), quizAnswers, difficulty);
            }
            else
            {
                currentItemIndex++;
                DisplayQuizItem(currentItemIndex);
            }
        }
        
        private void QuizUI_OnAnswered(int itemIndex, QuizAnswer quizAnswer)
        {
            quizAnswers[itemIndex] = quizAnswer;
        }
        
        private void QuizUI_OnEditClicked(int itemIndex)
        {
            quizUI.DisplayQuizPage();
            
            currentItemIndex = itemIndex;
            DisplayQuizItem(currentItemIndex);
        }
        
        private void QuizUI_OnFinishQuiz()
        {
            FinishQuiz();
        }
        
        private void QuizUI_OnExitClicked()
        {
            CloseQuiz();
        }
        
        
        // PRIVATE METHODS
        private void DisplayQuizItem(int itemIndex)
        {
            QuizItem quizItem = quizItems[itemIndex];
            QuizAnswer quizAnswer = quizAnswers[itemIndex];
            quizUI.DisplayQuizItem(itemIndex, quizItem, quizAnswer, difficulty);
        }
        
        private float CalculateScore(Difficulty? forcedDifficulty = null)
        {
            float totalScore = 0f;
            
            for (int i = 0; i < quizAnswers.Length; i++)
            {
                QuizItem quizItem = quizItems[i];
                QuizAnswer quizAnswer = quizAnswers[i];
                
                float score = quizItem.CheckAnswerAndGetScore(quizAnswer, forcedDifficulty);
                totalScore += score;
            }
            
            return totalScore;
        }
        
        private float CalculateTimeBonus(TimeSpan timeSpan)
        {
            return timeBonus.Evaluate((float)timeSpan.TotalSeconds);
            
            // float timeBonus = timeSpan.TotalSeconds switch
            // {
            //     < 10 => 300,
            //     < 15 => 200,
            //     < 30 => 100,
            //     < 45 => 50,
            //     < 60 => 25,
            //     < 90 => 15,
            //     _ => 10
            // };
            
            // return timeBonus;
        }
        
        
        // PRIVATE METHODS
        /// <summary> Starts the timer by setting startTime = DateTime.Now. </summary>
        private void StartTimer()
        {
            startTime = DateTime.Now;
            pausedDuration = TimeSpan.Zero; // reset pauses
            isQuizPaused = false;
        }
        
        /// <summary> Pauses the quiz timer. </summary>
        private void PauseTimer()
        {
            if (!isQuizRunning || isQuizPaused) return;
            
            isQuizPaused = true;
            pauseStartTime = DateTime.Now;
        }
        
        /// <summary> Resumes the quiz timer. </summary>
        private void ResumeTimer()
        {
            if (!isQuizRunning || !isQuizPaused) return;
            
            pausedDuration += DateTime.Now - pauseStartTime;
            isQuizPaused = false;
        }
        
        /// <summary> Ends the timer by setting endTime = DateTime.Now. </summary>
        private void EndTimer()
        {
            endTime = DateTime.Now;
        }
        
        /// <summary> Get current passed time based. </summary>
        private TimeSpan GetCurrentDuration()
        {
            return isQuizPaused
                ? pauseStartTime - startTime - pausedDuration
                : DateTime.Now - startTime - pausedDuration;
        }
        
        /// <summary> Get the playtime from startTime to endTime (excluding pauses). </summary>
        private TimeSpan GetPlayTime() => endTime - startTime - pausedDuration;
        
    }
}
