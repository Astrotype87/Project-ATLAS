using System;
using UnityEngine;

namespace ProjectATLAS.Menu.Background
{
    [CreateAssetMenu(fileName = "ParallaxPreset", menuName = "Scriptable Objects/ParallaxPreset")]
    public class ParallaxPreset : ScriptableObject
    {
        public ParallaxLayer[] layers;
        
        [Serializable]
        public struct ParallaxLayer
        {
            public Sprite background;
            public float speed;
            [Range(0, 1)] public float phase;
        }
    }
}
