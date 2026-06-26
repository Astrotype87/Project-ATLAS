using System;
using UnityEngine;


namespace ProjectATLAS.Beta.Quiz
{
    [Serializable]
    public abstract class QuizItem
    {
        public abstract void Initialize(); // Randomize question
        public abstract float CheckAnswerAndGetScore(QuizAnswer answer);
    }
    
}





