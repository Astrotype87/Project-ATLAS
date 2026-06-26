using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ProjectATLAS.Types
{
    [Serializable]
    public struct Range
    {
        public float min;
        public float max;
        
        public Range(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
        
        /// <summary> Returns a random float between min and max. </summary>
        public float GetRandom()
        {
            return Random.Range(min, max);
        }
    }
}
