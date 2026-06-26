using System;
using UnityEngine;

namespace ProjectATLAS.Minigame.Mini09_UnderwaterMining
{
    public class GameMessageUI : MonoBehaviour
    {
        [Header("Canvas References")]
        public GameObject gameOverCanvas;
        public GameObject missedCrystalCanvas;
        public GameObject winCanvas;
        
        public event Action OnGameWin;
        public event Action OnGameLose;
        

        private void Start()
        {
            // Hide all canvases at start
            if (gameOverCanvas != null) gameOverCanvas.SetActive(false);
            if (missedCrystalCanvas != null) missedCrystalCanvas.SetActive(false);
            if (winCanvas != null) winCanvas.SetActive(false);
        }

        public void ShowGameOver()
        {
            if (gameOverCanvas != null) gameOverCanvas.SetActive(true);
            OnGameLose?.Invoke();
        }

        public void ShowMissedCrystal()
        {
            if (missedCrystalCanvas != null) missedCrystalCanvas.SetActive(true);
            OnGameLose?.Invoke();
        }

        public void ShowWin()
        {
            if (winCanvas != null) winCanvas.SetActive(true);
            OnGameWin?.Invoke();
        }
    }
    
}
