using System;
using UnityEngine;


namespace ProjectATLAS.Beta.Quiz
{
    [Serializable]
    public class TrueOrFalseItem : QuizItem
    {
        public string[] question;
        public bool correctAnswer;
        private string selectedQuestion;
        public override void Initialize()
        {
            selectedQuestion = question[UnityEngine.Random.Range(0, question.Length)];
        }

        public override float CheckAnswerAndGetScore(QuizAnswer answer)
        {
            TrueOrFalseAnswer trueOrFalseAnswer = answer as TrueOrFalseAnswer;
            bool isCorrect = trueOrFalseAnswer.answer == correctAnswer;
            return isCorrect ? 1.0f : 0.0f;
        }

        public string GetQuestion()
        {
            return selectedQuestion;

        }
        public bool GetCorrrectAnswer()
        {
            return correctAnswer;
        }
    }
}
