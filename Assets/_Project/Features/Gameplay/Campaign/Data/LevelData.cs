using UnityEngine;
using UnityEngine.Serialization;

namespace ProjectATLAS.Gameplay
{
    public abstract class LevelData : CampaignLevelData
    {
        [SerializeField] protected int chapter;
        [SerializeField, FormerlySerializedAs("levelNumber")] protected int number;
        [SerializeField, FormerlySerializedAs("levelName")] protected string title;
        [SerializeField] [TextArea(3, 10)] protected string description;
        [SerializeField] [TextArea(3, 10)] protected string objectives;
        [SerializeField] [TextArea(3, 10)] protected string scoring;
        
        [SerializeField] protected string bronzeObjective;
        [SerializeField] protected string silverObjective;
        [SerializeField] protected string goldObjective;
        
        [SerializeField] private TimeBonus timeBonus;
        [SerializeField] private UnlockData[] unlockDatas;
        
        // PROPERTIES
        // public virtual string ID { get; } // LVL-01, PRE-01, POST-01
        // public virtual string Name { get; } // Level 01, Pre-Test 01, Post-Test 01
        // public virtual LevelType Type { get; } // Pre-Test, Lesson, Simulation, Challenge, Post-Test
        // public virtual string Info { get; }
        
        public override int Chapter => chapter;
        public override int Number => number;
        public override string Title => title;
        public override string Description => description;
        public override string Objectives => objectives;
        public override string Scoring => scoring;
        
        public string BronzeObjective => bronzeObjective;
        public string SilverObjective => silverObjective;
        public string GoldObjective => goldObjective;
        
        public float GoldTime => TimeBonus.GetSmallestSeconds();
        
        public TimeBonus TimeBonus => timeBonus;
        public override UnlockData[] UnlockDatas => unlockDatas;
    }
    
    public enum LevelType
    {
        PreTest, Lesson, Simulation, Challenge, PostTest
    }
}
