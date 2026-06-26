using ProjectATLAS.Gameplay;
using TMPro;
using UnityEngine;

namespace ProjectATLAS
{
    public class TopicDisplay : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private CampaignLevelData levelData;
        
        [Header("Components")]
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text nameText;
        
        private void OnValidate()
        {
            if (levelData)
            {
                if (levelText) levelText.text = $"Level {levelData.Number} - {levelData.Type}";
                if (nameText) nameText.text = $"{levelData.Title}";
            }
        }
    }
}
