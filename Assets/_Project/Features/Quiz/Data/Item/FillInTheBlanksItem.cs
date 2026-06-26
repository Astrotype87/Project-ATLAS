using UnityEngine;
using AstrotypeTools.InspectorAttributes;

using ProjectATLAS.Gameplay;
using System.Linq;
using ProjectATLAS.Utility;

namespace ProjectATLAS.Quiz
{
    public class FillInTheBlanksItem : QuizItem
    {
        [SerializeField] [Indent(-0.5f)] [TextArea(1, 8)] private string[] question = {"An object must __________ for work to be considered done, and the __________ must act along the motion of the object.\nThe expression to calculate work along the x-axis is __________, with W as work, F as force, and x as __________."};
        [SerializeField] [Indent(-0.5f)] private string[] correctAnswers = {"moves", "force", "W = F x", "displacement"};
        [SerializeField] [Indent(-0.5f)] private string[] wrongAnswers = {"velocity", "speed"};
        
        private string selectedQuestion;
        private string[] selectedChoices;
        
        
        // QUIZ ITEM METHODS
        public override void Initialize(bool isParaphrased)
        {
            selectedQuestion = isParaphrased
                ? question[Random.Range(0, question.Length)]
                : question[0];
            
            selectedChoices = correctAnswers.Concat(wrongAnswers).ToArray();
            selectedChoices.Shuffle();
        }
        
        public override float CheckAnswerAndGetScore(QuizAnswer quizAnswer, Difficulty? forcedDifficulty = null)
        {
            // Convert to fill in the blanks answer
            if (quizAnswer is not FillInTheBlanksAnswer answer) return 0f;
            
            int totalScore = 0;
            for (int i = 0; i < correctAnswers.Length; i++)
            {
                if (correctAnswers[i] == answer.answers[i])
                    totalScore ++;
            }
            
            return totalScore * (forcedDifficulty == null ? difficulty : forcedDifficulty.Value).AsMultiplier();
        }
        
        public override float GetMaxScore(Difficulty? forcedDifficulty = null) => correctAnswers.Length * (forcedDifficulty == null ? difficulty : forcedDifficulty.Value).AsMultiplier();
        
        public override string GetQuestionAsString() => selectedQuestion;
        
        public override string GetAnswerAsString() => string.Join(", ", correctAnswers);
        
        // PUBLIC METHODS
        public string GetQuestion() => selectedQuestion;
        public string[] GetChoices() => selectedChoices;
        public string[] GetCorrectAnswers() => correctAnswers;
    }
}
