using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using ProjectATLAS.UI;
using ProjectATLAS.Gameplay;
using ProjectATLAS.Gameplay.UI;

namespace ProjectATLAS.Simulation
{
    /// <summary>
    /// Abstract class for different simulation types.
    /// </summary>
    public abstract class SimulationSystem : MonoBehaviour
    {
        [Header("Simulation Settings")]
        [SerializeField] protected int scoreMultiplier = 100;
        
        [Header("Simulation UI Pages")]
        [SerializeField] protected GameStartPage gameStartPage;
        [SerializeField] protected GameResultPage gameResultPage;
        [SerializeField] protected UIPage customGameplayPage;
        
        [Header("Simulation UI Buttons")]
        [SerializeField] protected Button infoButton;
        [SerializeField] protected Button finishButton;
        
        [Header("Simulation UI Text")]
        [SerializeField] protected TMP_Text timerText;
        
        
        // PROTECTED FIELDS
        protected bool isSimulationRunning;
        protected DateTime startTime;
        protected DateTime endTime;
        
        protected bool isSimulationPaused;
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
        public bool IsSilverAwarded { get; protected set; }
        public bool IsGoldAwarded { get; protected set; }
        
        /// <summary> The simulation session can now be closed. </summary>
        public bool IsSimulationClosable { get; protected set; }
        
        
        // MONOBEHAVIOUR METHODS
        private void Update()
        {
            if (isSimulationRunning)
            {
                float time = (float)GetCurrentDuration().TotalSeconds;
                
                int totalMinutes = Mathf.FloorToInt(time / 60f);
                int seconds = Mathf.FloorToInt(time % 60f);
                // int centiseconds = Mathf.FloorToInt((time * 100f) % 100f); // two decimals (hundredths)
                
                timerText.text = $"{totalMinutes:00}:{seconds:00}";
            }
        }
        
        
        // PUBLIC METHODS
        /// <summary> Initializes the simulation. This is called by GameplayManager. </summary>
        public void InitializeSimulation(SimulationLevelData levelData, Difficulty difficulty)
        {
            if (gameStartPage) gameStartPage.StartButton.onClick.AddListener(StartButton_onClick);
            if (infoButton)
            {
                infoButton.onClick.AddListener(InfoButton_onClick);
                
                if (levelData)
                {
                    TMP_Text[] textComponents = infoButton.GetComponentsInChildren<TMP_Text>();
                    if (textComponents.Length >= 1)
                        textComponents[0].text = $"Simulation {levelData.Chapter}";
                    if (textComponents.Length >= 2)
                        textComponents[1].text = levelData.Title;
                }
            }
            
            if (finishButton) finishButton.onClick.AddListener(FinishButton_onClick);
            if (gameResultPage) gameResultPage.QuitButton.onClick.AddListener(QuitButton_onClick);
            
            IsSimulationClosable = false;
            
            OnInitializeSimulation(difficulty);
            
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
        
        /// <summary> Starts the simulation. The timer starts running. </summary>
        public void StartSimulation()
        {
            isSimulationRunning = true;
            StartTimer();
            customGameplayPage.OpenPageInGroup();
            
            OnStartSimulation();
        }
        
        /// <summary> Pauses the simulation. </summary>
        public void PauseSimulation()
        {
            PauseTimer();
        }
        
        /// <summary> Resumes the simulation. </summary>
        public void ResumeSimulation()
        {
            ResumeTimer();
        }
        
        /// <summary> Marks the simulation as finished. The timer ends. The results are displayed. </summary>
        public void FinishSimulation()
        {
            EndTimer();
            isSimulationRunning = false;
            gameResultPage.OpenPageInGroup();
            
            OnFinishSimulation(); 
        }
        
        /// <summary> Notifies other systems that this simulation can now be closed. </summary>
        public void CloseSimulation()
        {
            IsSimulationClosable = true;
        }
        
        
        // PROTECTED ABSTRACT METHODS
        /// <summary> Custom code when simulation is initialized. </summary>
        protected abstract void OnInitializeSimulation(Difficulty difficulty);
        /// <summary> Custom code when simulation is started. </summary>
        protected abstract void OnStartSimulation();
        /// <summary> Custom code when simulation is finished. </summary>
        protected abstract void OnFinishSimulation();
        
        
        
        // EVENT LISTENER METHODS
        private void StartButton_onClick()
        {
            if (!isSimulationRunning) StartSimulation();    // Start simulation and open gameplay page
            else customGameplayPage.OpenPageInGroup();      // Open gameplay page
        }
        private void InfoButton_onClick() => gameStartPage.OpenPageInGroup();
        private void FinishButton_onClick() => FinishSimulation();
        private void QuitButton_onClick() => CloseSimulation();
        
        
        // PROTECTED METHODS
        /// <summary> Starts the timer by setting startTime = DateTime.Now. </summary>
        protected void StartTimer()
        {
            startTime = DateTime.Now;
            pausedDuration = TimeSpan.Zero; // reset pauses
            isSimulationPaused = false;
        }
        
        /// <summary> Pauses the simulation timer. </summary>
        protected void PauseTimer()
        {
            if (!isSimulationRunning || isSimulationPaused) return;
            
            isSimulationPaused = true;
            pauseStartTime = DateTime.Now;
        }
        
        /// <summary> Resumes the simulation timer. </summary>
        protected void ResumeTimer()
        {
            if (!isSimulationRunning || !isSimulationPaused) return;
            
            pausedDuration += DateTime.Now - pauseStartTime;
            isSimulationPaused = false;
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
            return isSimulationPaused
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
