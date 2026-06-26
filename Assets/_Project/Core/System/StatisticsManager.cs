using UnityEngine;

using ProjectATLAS.GameData;
using ProjectATLAS.Architecture;

namespace ProjectATLAS.System
{
    public class StatisticsManager : PersistentSingletonMonoBehaviour<StatisticsManager>
    {
        [SerializeField] private GameDataManager gameDataService;
        
        private void Update()
        {
            gameDataService.StatisticsData.timeSpentPlaying += Time.deltaTime;
        }
    }
}
