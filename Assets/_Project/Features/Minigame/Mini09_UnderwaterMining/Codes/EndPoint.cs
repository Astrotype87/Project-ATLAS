using System;
using UnityEngine;

namespace ProjectATLAS.Minigame.Mini09_UnderwaterMining
{
    public class EndPoint : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                if (GameManager.Instance.CrystalsCollected < GameManager.Instance.TotalCrystals)
                {
                    // ❌ Not enough crystals → missed crystal
                    FindObjectOfType<GameMessageUI>().ShowMissedCrystal();
                    GameManager.Instance.DisablePlayer();
                }
                else
                {
                    // ✅ Enough crystals → win
                    FindObjectOfType<GameMessageUI>().ShowWin();
                    GameManager.Instance.DisablePlayer();
                }
            }
        }
    }
}
