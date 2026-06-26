using UnityEngine;
using TMPro;

namespace ProjectATLAS.Minigame.Mini09_UnderwaterMining
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("Crystal Settings")]
        public int CrystalsCollected = 0;
        public int TotalCrystals = 0;
        public TextMeshProUGUI crystalText;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            UpdateCrystalUI();
        }

        // 🔹 Called when player collects a crystal
        public void CollectCrystal()
        {
            CrystalsCollected++;
            UpdateCrystalUI();

            // ✅ Instantly win if all crystals are collected
            if (CrystalsCollected >= TotalCrystals)
            {
                FindObjectOfType<GameMessageUI>().ShowWin();
                DisablePlayer();
            }
        }

        // 🔹 Update UI text
        private void UpdateCrystalUI()
        {
            if (crystalText != null)
                crystalText.text = CrystalsCollected + "/" + TotalCrystals;
        }

        // 🔹 Disable player movement after end state
        public void DisablePlayer()
        {
            PlayerController pc = FindObjectOfType<PlayerController>();
            if (pc != null) pc.enabled = false;

            SubmarineAirSystem air = FindObjectOfType<SubmarineAirSystem>();
            if (air != null) air.autoMoveSpeed = 0f;
        }
    }
}
