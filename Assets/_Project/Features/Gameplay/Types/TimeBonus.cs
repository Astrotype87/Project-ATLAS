using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectATLAS.Gameplay
{
    [Serializable]
    public class TimeBonus
    {
        public List<TimeBonusEntry> timeBonuses = new();
        public int lowestScore = 10;
        
        public int Evaluate(float timeInSeconds)
        {
            foreach (var entry in timeBonuses)
            {
                if (timeInSeconds < entry.seconds)
                    return entry.score;
            }
            
            return lowestScore;
        }
        
        public float GetSmallestSeconds()
        {
            if (timeBonuses == null || timeBonuses.Count == 0)
                return 0f; // or throw an exception / return a default value
            
            float smallest = float.MaxValue;
            
            foreach (var entry in timeBonuses)
            {
                if (entry.seconds < smallest)
                    smallest = entry.seconds;
            }
            
            return smallest;
        }
    }
    
    [Serializable]
    public struct TimeBonusEntry
    {
        public float seconds;
        public int score;
        
        public TimeBonusEntry(float seconds, int score)
        {
            this.seconds = seconds;
            this.score = score;
        }
    }
}
