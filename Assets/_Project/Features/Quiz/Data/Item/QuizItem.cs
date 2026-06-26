using System;
using UnityEngine;

using ProjectATLAS.Gameplay;

namespace ProjectATLAS.Quiz
{
    [Serializable]
    public abstract class QuizItem
    {
        [SerializeField] protected Difficulty difficulty;
        
        public Difficulty Difficulty => difficulty;
        
        public abstract void Initialize(bool isParaphrased);
        public abstract float CheckAnswerAndGetScore(QuizAnswer quizAnswer, Difficulty? forcedDifficulty = null);
        public abstract float GetMaxScore(Difficulty? forcedDifficulty = null);
        public abstract string GetQuestionAsString();
        public abstract string GetAnswerAsString();
    }
}