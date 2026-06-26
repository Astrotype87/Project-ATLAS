using ProjectATLAS.Gameplay.UI;
using UnityEngine;

namespace ProjectATLAS.Gameplay
{
    public abstract class CampaignLevelData : ScriptableObject
    {
        public abstract string ID { get; } // LVL-01, PRE-01, POST-01
        public abstract string Name { get; } // Level 01, Pre-Test 01, Post-Test 01
        public abstract LevelType Type { get; } // Pre-Test, Lesson, Simulation, Challenge, Post-Test
        public abstract string Info { get; }
        
        public abstract int Chapter { get; }
        public abstract int Number { get; }
        public abstract string Title { get; }
        public abstract string Description { get; }
        public abstract string Objectives { get; }
        public abstract string Scoring { get; }
        
        public abstract string Mechanics { get; }
        
        public abstract UnlockData[] UnlockDatas { get; }
    }
}
