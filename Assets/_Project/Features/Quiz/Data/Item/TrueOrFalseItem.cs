using System;
using UnityEngine;
using Random = UnityEngine.Random;

using AstrotypeTools.InspectorAttributes;

using ProjectATLAS.Gameplay;
using ProjectATLAS.Types;
using ProjectATLAS.Utility;

namespace ProjectATLAS.Quiz
{
    [Serializable]
    public class TrueOrFalseItem : QuizItem
    {
        [SerializeField] [Indent(-0.5f)] [TextArea(1, 4)] private string[] question = { "Enter question.", "Enter paraphrased question.", "Enter another paraphrased question." };
        [SerializeField] private BoolEnum correctAnswer;
        
        private string selectedQuestion;
        
        
        // QUIZ ITEM METHODS
        public override void Initialize(bool isParaphrased)
        {
            // Select a random paraphrased question and correct answer
            selectedQuestion = isParaphrased
                ? question[Random.Range(0, question.Length)]
                : question[0];
        }
        
        public override float CheckAnswerAndGetScore(QuizAnswer quizAnswer, Difficulty? forcedDifficulty = null)
        {
            // Convert to true or false answer
            if (quizAnswer is not TrueOrFalseAnswer answer) return 0f;
            
            // Check if answer is correct and return score
            bool isCorrect = answer.answer == correctAnswer;
            return isCorrect ? 1f * (forcedDifficulty == null ? difficulty : forcedDifficulty.Value).AsMultiplier() : 0f;
        }
        
        public override float GetMaxScore(Difficulty? forcedDifficulty = null) => 1f * (forcedDifficulty == null ? difficulty : forcedDifficulty.Value).AsMultiplier();
        
        public override string GetQuestionAsString() => selectedQuestion;
        
        public override string GetAnswerAsString() => GetCorrectAnswer().ToString();
        
        // PUBLIC METHODS
        public string GetQuestion() => selectedQuestion;
        public bool GetCorrectAnswer() => correctAnswer.AsBool();
    }
}