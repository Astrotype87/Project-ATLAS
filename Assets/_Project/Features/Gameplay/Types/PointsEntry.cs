using System;
using UnityEngine;

namespace ProjectATLAS.Gameplay
{
    [Serializable]
    public struct PointsEntry
    {
        public string name;
        public int points;
        
        public PointsEntry(string name, int points)
        {
            this.name = name;
            this.points = points;
        }
        
        /// <summary> Returns total points by int. </summary>
        public static int GetTotalPoints(PointsEntry[] pointsEntries)
        {
            int total = 0;
            for (int i = 0; i < pointsEntries.Length; i++)
                total += pointsEntries[i].points;
            
            return total;
        }
    }
}
