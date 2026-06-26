using System;
using UnityEngine;

namespace ProjectATLAS.Gameplay
{
    [Serializable]
    public struct UnlockData
    {
        public UnlockIcon icon;
        public string name;
        [TextArea(2, 4)] public string description;
        
        public UnlockData(UnlockIcon icon, string name, string description)
        {
            this.icon = icon;
            this.name = name;
            this.description = description;
        }
    }
    
    public enum UnlockIcon
    {
        PreTest,
        Lesson,
        Simulation,
        Challenge,
        PostTest,
        
        Guidebook,
        Glossary,
        QuizTopic,
        Chapter,
    }
}
