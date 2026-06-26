using System;
using UnityEngine;


namespace ProjectATLAS.Beta.Quiz
{
    [Serializable]
    public class SolvingItem : QuizItem
    {
        public string[] question;
        public string expression;
        public Variable[] variables;
        private string selectedQuestion;

        public struct Variable
        {
            public string name;
            public float value;
            public string unit;
        }
        public override void Initialize()
        {

        }

        public override float CheckAnswerAndGetScore(QuizAnswer answer)
        {
            SolvingAnswer solvingAnswer = answer as SolvingAnswer;
            if(solvingAnswer == null)
            
                return 0f;

            double playerAnswer = solvingAnswer.answer;

            //var expr = new Expression(expression);
            //double correctAnswer = expression.Evaluate();

            //bool isCorrect = playerAnswer == correctAnswer;
            return 0F;//daya

        }
        public string GetQuestion()
        {
            return selectedQuestion;
        }

        public Variable[] GetVariables()
        {
            return variables;
        }
    }
    
}
