using UnityEngine;

using ProjectATLAS.UI;
using ProjectATLAS.Gameplay;
using ProjectATLAS.Freeplay;

namespace ProjectATLAS.Gameplay
{
    public class PauseAction : UIAction
    {
        protected override void OnClick()
        {
            Debug.Log("PauseAction.OnClick()");
            
            GameplayManager gameplayManager = GameplayManager.Instance;
            if (gameplayManager)
            {
                gameplayManager.PauseGame();
            }
            
            FreeplayManager freeplayManager = FreeplayManager.Instance;
            if (freeplayManager)
            {
                freeplayManager.PauseGame();
            }
        }
    }
}
