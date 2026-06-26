using System;
using UnityEngine;

namespace ProjectATLAS.Beta.Quiz
{
    [Serializable]
    public class FillInTheBlanksItem : QuizItem
    {
        public string question;
        public string[] correctAnswers;
        public string[] wrongAnswers;

        public override void Initialize()
        {
            
        }

        public override float CheckAnswerAndGetScore(QuizAnswer answer)
        {
            FillInTheBlanksAnswer fillInTheBlanksAnswer = answer as FillInTheBlanksAnswer;

            if (fillInTheBlanksAnswer == null || fillInTheBlanksAnswer.answers == null)
            
                return 0f;

                int correctCount = 0;

            for (int i = 0; i < Mathf.Min(correctAnswers.Length, fillInTheBlanksAnswer.answers.Length); i++)
            {
                if (string.Equals(fillInTheBlanksAnswer.answers[i], correctAnswers[i], StringComparison.OrdinalIgnoreCase))
                {
                    correctCount++;
                }
            }

            return (float)correctCount;
        }

        public string GetQuestion()
        {
            return question;
        }

        public string[] GetCorrectAnswer()
        {
            return correctAnswers;
        }
    }
    
}
