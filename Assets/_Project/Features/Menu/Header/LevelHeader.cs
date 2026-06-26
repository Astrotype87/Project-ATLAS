using UnityEngine;

using ProjectATLAS.Gameplay;
using TMPro;

namespace ProjectATLAS.Menu
{
    public class LevelHeader : CustomHeader
    {
        [Header("Data")]
        [SerializeField] private LevelType levelType;
        [SerializeField] private string info;
        
        [Header("Components")]
        [SerializeField] private TMP_Text levelTypeText;
        [SerializeField] private TMP_Text infoText;
        
        // MONOBEHAVIOUR METHODS
        private void OnValidate()
        {
            SetLevelType(levelType);
            SetInfo(info);
        }
        
        // PUBLIC METHODS
        public void SetLevelType(LevelType levelType)
        {
            this.levelType = levelType;
            if (levelTypeText) levelTypeText.text = levelType.ToString();
        }
        
        public void SetInfo(string info)
        {
            this.info = info;
            if (infoText) infoText.text = info;
        }
    }
}
