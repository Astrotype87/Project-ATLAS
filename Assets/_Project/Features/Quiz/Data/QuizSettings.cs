using System;
using UnityEngine;

namespace ProjectATLAS.Quiz
{
    [CreateAssetMenu(fileName = "QuizSettings", menuName = "Scriptable Objects/QuizSettings")]
    public class QuizSettings : ScriptableObject
    {
        public int totalItems = 4;
        public bool shuffled = false;
        [Range(0, 1)] public float paraphraseChance = 0.0f;
        [Range(0, 1)] public float passingScorePercentage = 0.5f;
        public bool includeLowerDifficulty = true;
        public ItemTypes itemTypes = ItemTypes.All;
    }
    
    [Flags]
    public enum ItemTypes
    {
        None            = 0,
        MultipleChoice  = 1 << 0,
        TrueOrFalse     = 1 << 1,
        FillInTheBlanks = 1 << 2,
        Matching        = 1 << 3,
        Sequence        = 1 << 4,
        Categorization  = 1 << 5,
        Solving         = 1 << 6,
        All             = MultipleChoice | TrueOrFalse | FillInTheBlanks | Matching | Sequence | Categorization | Solving
    }
}
