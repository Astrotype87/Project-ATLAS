using UnityEngine;

namespace ProjectATLAS.Gameplay
{
    public abstract class TestData : CampaignLevelData
    {
        [SerializeField] protected int chapter;
        [SerializeField] protected string title;
        [SerializeField] [TextArea(3, 10)] protected string description;
        [SerializeField] [TextArea(3, 10)] protected string objectives;
        [SerializeField] [TextArea(3, 10)] protected string scoring;
        
        [SerializeField] private UnlockData[] unlockDatas;
        
        // PROPERTIES
        // public virtual string ID { get; } // LVL-01, PRE-01, POST-01
        // public virtual string Name { get; } // Level 01, Pre-Test 01, Post-Test 01
        // public virtual LevelType Type { get; } // Pre-Test, Lesson, Simulation, Challenge, Post-Test
        // public virtual string Info { get; }
        
        public override int Chapter => chapter;
        public override int Number => chapter;
        public override string Title => title;
        public override string Description => description;
        public override string Objectives => objectives;
        public override string Scoring => scoring;
        
        public override string Mechanics => Objectives;
        
        public override UnlockData[] UnlockDatas => unlockDatas;
    }
}
