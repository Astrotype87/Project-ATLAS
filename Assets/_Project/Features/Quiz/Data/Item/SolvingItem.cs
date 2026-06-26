using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

using MathEvaluation;
using MathEvaluation.Context;
using MathEvaluation.Parameters;

using AstrotypeTools.InspectorAttributes;
using Range = ProjectATLAS.Types.Range;

using ProjectATLAS.Gameplay;

namespace ProjectATLAS.Quiz
{
    public class SolvingItem : QuizItem
    {
        [SerializeField] [Indent(-0.5f)] [TextArea(1, 4)] private string[] question = { "The car has traveled {d} meters for {t} seconds. What is the speed of the car?" };
        [SerializeField, TextArea(1, 4)] private string formula = "d * t";
        [SerializeField] private List<SolvingVariable> variables = new() { new("d", "Distance", "m", 10, 20), new("t", "Time", "s", 1, 5), };
        
        [Header("Answer")]
        [SerializeField] private string variable = "s";
        [SerializeField] private string name = "Speed";
        [SerializeField] private string unit = "m/s";
        [SerializeField, Range(0, 4)] private int rounding = 2;
        
        private string selectedQuestion;
        private double selectedAnswer;
        
        // PROPERTIES
        public string Variable => variable;
        public string Name => name;
        public string Unit => unit;
        public int Rounding => rounding;
        
        
        // QUIZ ITEM METHODS
        public override void Initialize(bool isParaphrased)
        {
            // Generate variable values from defined range
            for (int i = 0; i < variables.Count; i++)
            {
                variables[i].GenerateValue(rounding);
            }
            
            // Select a random paraphrased question and correct answer
            selectedQuestion = isParaphrased
                ? question[UnityEngine.Random.Range(0, question.Length)]
                : question[0];
            
            for (int i = 0; i < variables.Count; i++)
            {
                string idText = "{" + variables[i].Variable + "}";
                string valueText = variables[i].Value.ToString($"F{rounding}");
                
                selectedQuestion = selectedQuestion.Replace(idText, valueText);
            }
            
            // Evaluate the formula and store answer
            MathParameters parameters = new();
            for (int i = 0; i < variables.Count; i++)
            {
                SolvingVariable variable = variables[i];
                parameters.BindVariable(variable.Value, variable.Variable);
            }
            
            formula = formula.Trim(); // Trim whitespace, known to cause error
            MathExpression expression = new(formula, new ScientificMathContext());
            selectedAnswer = Math.Round(expression.Evaluate(parameters), rounding);
            
            // LOGS
            Debug.Log($"Question: {selectedQuestion}");
            Debug.Log($"Formula: {formula}  Answer: {selectedAnswer}");
        }
        
        public override float CheckAnswerAndGetScore(QuizAnswer quizAnswer, Difficulty? forcedDifficulty = null)
        {
            // Convert to true or false answer
            if (quizAnswer is not SolvingAnswer answer) return 0f;
            
            // Check if answer is correct and return score
            float playerAnswer = (float)Math.Round(answer.answer, rounding);
            float correctAnswer = (float)Math.Round(selectedAnswer, rounding);
            
            bool isCorrect = Mathf.Approximately(playerAnswer, correctAnswer);
            return isCorrect ? 1f * (forcedDifficulty == null ? difficulty : forcedDifficulty.Value).AsMultiplier() : 0f;
        }
        
        public override float GetMaxScore(Difficulty? forcedDifficulty = null) => 1f * (forcedDifficulty == null ? difficulty : forcedDifficulty.Value).AsMultiplier();
        
        public override string GetQuestionAsString() => selectedQuestion;
        
        public override string GetAnswerAsString() => selectedAnswer.ToString($"F{rounding}");
        
        
        // PUBLIC METHODS
        public string GetQuestion() => selectedQuestion;
        public List<SolvingVariable> GetVariables() => variables;
        public double GetCorrectAnswer() => selectedAnswer;
        
        
        
        [Serializable]
        public class SolvingVariable
        {
            [FormerlySerializedAs("identifier")]
            [SerializeField] [Indent(-1)] private string variable;
            [SerializeField] [Indent(-1)] private string name;
            [SerializeField] [Indent(-1)] private string unit;
            [SerializeField] [Indent(-1)] private Range range = new(1, 10);
            
            public SolvingVariable(string variable, string name, string unit, float min, float max)
            {
                this.variable = variable;
                this.name = name;
                this.unit = unit;
                this.range = new(min, max);
            }
            
            public string Variable => variable;
            public string Name => name;
            public string Unit => unit;
            public double Value { get; private set; }
            
            public void GenerateValue()
            {
                Value = range.GetRandom();
            }
            
            public void GenerateValue(int rounding)
            {
                Value = Math.Round(range.GetRandom(), rounding);
            }
        }
    }
}
