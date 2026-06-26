using UnityEngine;
using KBCore.Refs;

using ProjectATLAS.UI;
using ProjectATLAS.Library.Guidebooks;
using ProjectATLAS.Gameplay;
using ProjectATLAS.Cheats;

namespace ProjectATLAS.Menu
{
    public class GuidebooksPage : UIPage
    {
        [Header("Components")]
        [SerializeField, Child] private GuidebookButton[] guidebookButtons;
        
        private LevelManager levelManager;
        
        private void Awake()
        {
            levelManager = LevelManager.Instance;
        }
        
        public override void OpenPage()
        {
            base.OpenPage();
            
            if (!levelManager) levelManager = LevelManager.Instance;
            if (levelManager)
            {
                int lastCompletedLevel = levelManager.LastCompletedLevel();
                
                foreach (var guidebookButton in guidebookButtons)
                {
                    bool isLocked = !CheatsManager.UnlockAllGlossary && guidebookButton.Level > lastCompletedLevel;
                    guidebookButton.SetLocked(isLocked);
                }
            }
        }
    }
}
