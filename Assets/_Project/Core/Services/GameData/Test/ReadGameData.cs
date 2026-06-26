using UnityEngine;
using ProjectATLAS.Architecture;
using System;

namespace ProjectATLAS.GameData.Test
{
    public class ReadGameData : MonoBehaviour, IInject<IGameDataService>
    {
        private IGameDataService gameDataService;
        
        public void Inject(IGameDataService dependency) => gameDataService = dependency;


        private void Start()
        {
            Debug.Log(gameDataService.DetailsData.displayName);
        }
    }
}
