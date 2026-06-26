using System;
using UnityEngine;

namespace ProjectATLAS.UI
{
    [Serializable]
    public struct Padding
    {
        public float left;
        public float right;
        public float top;
        public float bottom;
        
        public Vector2 GetOffsetMin() => new(left, bottom);
        public Vector2 GetOffsetMax() => new(-right, -top);
    }
}
