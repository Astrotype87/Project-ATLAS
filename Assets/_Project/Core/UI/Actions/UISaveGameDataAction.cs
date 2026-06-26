using UnityEngine;
using ProjectATLAS.GameData;

namespace ProjectATLAS.UI
{
    public class UISaveGameDataAction : UIAction
    {
        private GameDataManager gameDataService;
        
        protected override void Awake()
        {
            base.Awake();
            
            gameDataService = GameDataManager.Instance;
        }
        
        protected override void OnClick()
        {
            if (gameDataService == null)
                gameDataService = GameDataManager.Instance;
            
            if (gameDataService)
            {
                gameDataService.SaveData();
            }
        }
    }
}
