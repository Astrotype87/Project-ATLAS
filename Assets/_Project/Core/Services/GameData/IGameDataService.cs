using UnityEngine;

namespace ProjectATLAS.GameData
{
    public interface IGameDataService
    {
        public DetailsData DetailsData { get; }
        public AvatarData AvatarData { get; }
        public CampaignData CampaignData { get; }
        public RecordsData RecordsData { get; }
        public StatisticsData StatisticsData { get; }
        
        public void LoadData();
        public void SaveData();
    }
}
