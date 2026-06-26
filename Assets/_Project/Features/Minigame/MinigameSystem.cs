using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using ProjectATLAS.UI;
using ProjectATLAS.Gameplay;
using ProjectATLAS.Gameplay.UI;

namespace ProjectATLAS.Minigame
{
    /// <summary>
    /// Abstract class for different minigame types.
    /// </summary>
    public abstract class MinigameSystem : MonoBehaviour
    {
        [Header("Minigame Settings")]
        [SerializeField] protected int scoreMultiplier = 100;
        
        [Header("Minigame UI Pages")]
        [SerializeField] protected GameStartPage gameStartPage;
        [SerializeField] protected GameResultPage gameResultPage;
        [SerializeField] protected UIPage customGameplayPage;
        
        [Header("Minigame UI Buttons")]
        [SerializeField] protected Button infoButton;
        [SerializeField] protected Button finishButton;
        
        [Header("Minigame UI Text")]
        [SerializeField] protected TMP_Text timerText;
        
        
        // PROTECTED FIELDS
        protected bool isMinigameRunning;
        protected DateTime startTime;
        protected DateTime endTime;
        
        protected bool isMinigamePaused;
        protected TimeSpan pausedDuration;  // total accumulated time already played before pause
        protected DateTime pauseStartTime;  // when pause began
        
        // PROPERTIES
        public float ScorePercentage => Score / MaxScore;
        public float Score { get; protected set; }
        public float MaxScore { get; protected set; }
        public float PlayTime { get; protected set; }
        public PointsEntry[] PointsEntries { get; protected set; }
        
        public bool IsCompleted { get; protected set; }
        public bool IsBronzeAwarded { get; protected set; }
        public bool IsGoldAwarded { get; protected set; }
        public bool IsSilverAwarded { get; protected set; }
        
        /// <summary> The minigame session can now be closed. </summary>
        public bool IsMinigameClosable { get; protected set; }
        
        
        // MONOBEHAVIOUR METHODS
        private void Update()
        {
            if (isMinigameRunning)
            {
                float time = (float)GetCurrentDuration().TotalSeconds;
                
                int totalMinutes = Mathf.FloorToInt(time / 60f);
                int seconds = Mathf.FloorToInt(time % 60f);
                // int centiseconds = Mathf.FloorToInt((time * 100f) % 100f); // two decimals (hundredths)
                
                timerText.text = $"{totalMinutes:00}:{seconds:00}";
            }
        }
        
        
        // PUBLIC METHODS
        /// <summary> Initializes the minigame. This is called by GameplayManager. </summary>
        public void InitializeMinigame(ChallengeLevelData levelData, Difficulty difficulty)
        {
            gameStartPage.StartButton.onClick.AddListener(StartButton_onClick);
            if (infoButton)
            {
                infoButton.onClick.AddListener(InfoButton_onClick);
                
                if (levelData)
                {
                    TMP_Text[] textComponents = infoButton.GetComponentsInChildren<TMP_Text>();
                    if (textComponents.Length >= 1)
                        textComponents[0].text = $"Minigame {levelData.Chapter}";
                    if (textComponents.Length >= 2)
                        textComponents[1].text = levelData.Title;
                }
            }
            if (finishButton) finishButton.onClick.AddListener(FinishButton_onClick);
            gameResultPage.QuitButton.onClick.AddListener(QuitButton_onClick);
            
            IsMinigameClosable = false;
            
            OnInitializeMinigame(difficulty);
            
            if (levelData && gameStartPage)
            {
                gameStartPage.SetTitle(levelData.Title);
                gameStartPage.SetSubtitle($"{levelData.Name} • {levelData.Type} Type • {difficulty}");
                gameStartPage.SetInstructions(levelData.Instructions);
                
                gameStartPage.SetObjectives(levelData.Objectives);
                gameStartPage.SetBronze(levelData.BronzeObjective);
                gameStartPage.SetSilver(levelData.SilverObjective);
                gameStartPage.SetGold(levelData.GoldObjective);
            }
            
            if (levelData && gameResultPage)
            {
                gameResultPage.SetTitle($"{levelData.Title} Results");
                gameResultPage.SetSubtitle($"{levelData.Name} • {levelData.Type} Type • {difficulty}");
            }
        }
        
        /// <summary> Starts the minigame. The timer starts running. </summary>
        public void StartMinigame()
        {
            isMinigameRunning = true;
            StartTimer();
            customGameplayPage.OpenPageInGroup();
            
            OnStartMinigame();
        }
        
        /// <summary> Pauses the minigame. </summary>
        public void PauseMinigame()
        {
            PauseTimer();
        }
        
        /// <summary> Resumes the minigame. </summary>
        public void ResumeMinigame()
        {
            ResumeTimer();
        }
        
        /// <summary> Marks the minigame as finished. The timer ends. The results are displayed. </summary>
        public void FinishMinigame()
        {
            Debug.Log($"Finish minigame");
            EndTimer();
            isMinigameRunning = false;
            gameResultPage.OpenPageInGroup();
            
            OnFinishMinigame(); 
        }
        
        /// <summary> Notifies other systems that this minigame can now be closed. </summary>
        public void CloseMinigame()
        {
            IsMinigameClosable = true;
        }
        
        
        // PROTECTED ABSTRACT METHODS
        /// <summary> Custom code when minigame is initialized. </summary>
        protected abstract void OnInitializeMinigame(Difficulty difficulty);
        /// <summary> Custom code when minigame is started. </summary>
        protected abstract void OnStartMinigame();
        /// <summary> Custom code when minigame is finished. </summary>
        protected abstract void OnFinishMinigame();
        
        
        
        // EVENT LISTENER METHODS
        private void StartButton_onClick()
        {
            if (!isMinigameRunning) StartMinigame();        // Start minigame and open gameplay page
            else customGameplayPage.OpenPageInGroup();      // Open gameplay page
        }
        private void InfoButton_onClick() => gameStartPage.OpenPageInGroup();
        private void FinishButton_onClick() => FinishMinigame();
        private void QuitButton_onClick() => CloseMinigame();
        
        
        // PROTECTED METHODS
        /// <summary> Starts the timer by setting startTime = DateTime.Now. </summary>
        protected void StartTimer()
        {
            startTime = DateTime.Now;
            pausedDuration = TimeSpan.Zero; // reset pauses
            isMinigamePaused = false;
        }
        
        /// <summary> Pauses the minigame timer. </summary>
        protected void PauseTimer()
        {
            if (!isMinigameRunning || isMinigamePaused) return;
            
            isMinigamePaused = true;
            pauseStartTime = DateTime.Now;
        }
        
        /// <summary> Resumes the minigame timer. </summary>
        protected void ResumeTimer()
        {
            if (!isMinigameRunning || !isMinigamePaused) return;
            
            pausedDuration += DateTime.Now - pauseStartTime;
            isMinigamePaused = false;
        }
        
        /// <summary> Ends the timer by setting endTime = DateTime.Now. </summary>
        protected void EndTimer()
        {
            endTime = DateTime.Now;
            PlayTime = (float)(endTime - startTime - pausedDuration).TotalSeconds;
        }
        
        /// <summary> Get current passed time based. </summary>
        protected TimeSpan GetCurrentDuration()
        {
            return isMinigamePaused
                ? pauseStartTime - startTime - pausedDuration
                : DateTime.Now - startTime - pausedDuration;
        }
        
        /// <summary> Get the playtime from startTime to endTime (excluding pauses). </summary>
        protected TimeSpan GetPlayTime() => endTime - startTime - pausedDuration;
        
        
        // PUBLIC STATIC METHODS
        public static float CalculateTimeBonus(float playTime) => playTime switch
        {
            < 10 => 300,
            < 15 => 200,
            < 30 => 100,
            < 45 => 50,
            < 60 => 25,
            < 90 => 15,
            _ => 10
        };
    }
}
