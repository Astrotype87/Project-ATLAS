using UnityEngine;
using AstrotypeTools.InspectorAttributes;

using ProjectATLAS.Gameplay;
using ProjectATLAS.Utility;

namespace ProjectATLAS.Quiz
{
    public class SequenceItem : QuizItem
    {
        [Indent(-0.5f)] [TextArea(2, 4)]
        [SerializeField] private string[] question = {"Arrange the step-by-step process of solving projectile motion."};
        [Indent(-0.5f)] [TextArea(2, 4)]
        [SerializeField] private string[] correctOrderedAnswer = {"Draw & list knowns.", "Resolve the initial velocity into components.", "Write separate kinematic equations for x and y.", "Use the right formula for the quantity you want."};
        
        private string selectedQuestion;
        private string[] selectedShuffledAnswer;
        
        
        // QUIZ ITEM METHODS
        public override void Initialize(bool isParaphrased)
        {
            selectedQuestion = isParaphrased
                ? question[Random.Range(0, question.Length)]
                : question[0];
            
            selectedShuffledAnswer = correctOrderedAnswer.CloneArray().Shuffle();
            
            Debug.Log($"selectedShuffledAnswer: {string.Join(", ", selectedShuffledAnswer)}");
        }
        
        public override float CheckAnswerAndGetScore(QuizAnswer quizAnswer, Difficulty? forcedDifficulty = null)
        {
            // Convert to sequence answer
            if (quizAnswer is not SequenceAnswer answer) return 0f;
            
            int totalScore = 0;
            for (int i = 0; i < correctOrderedAnswer.Length; i++)
            {
                if (correctOrderedAnswer[i] == answer.answers[i])
                    totalScore++;
            }
            
            return totalScore * (forcedDifficulty == null ? difficulty : forcedDifficulty.Value).AsMultiplier();
        }
        
        public override float GetMaxScore(Difficulty? forcedDifficulty = null) => correctOrderedAnswer.Length * (forcedDifficulty == null ? difficulty : forcedDifficulty.Value).AsMultiplier();
        public override string GetQuestionAsString() => selectedQuestion;
        public override string GetAnswerAsString() => string.Join(", ", correctOrderedAnswer);
        
        
        // PUBLIC METHODS
        public string GetQuestion() => selectedQuestion;
        public string[] GetShuffledAnswers() => selectedShuffledAnswer;
        public string[] GetCorrectOrderAnswers() => correctOrderedAnswer;
    }
}
