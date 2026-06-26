using System;
using UnityEngine;
using Random = UnityEngine.Random;

using AstrotypeTools.InspectorAttributes;

using ProjectATLAS.Gameplay;
using ProjectATLAS.Utility;

namespace ProjectATLAS.Quiz
{
    [Serializable]
    public class MultipleChoiceItem : QuizItem
    {
        [SerializeField] [Indent(-0.5f)] [TextArea(1, 4)] private string[] question = { "Enter question.", "Enter paraphrased question.", "Enter another paraphrased question." };
        [SerializeField] [Indent(-0.5f)] [TextArea(1, 4)] [HideLabel] private string[] correctAnswer = { "Enter correct answer." };
        [SerializeField] [Indent(-0.5f)] [TextArea(1, 4)] [HideLabel] private string[] wrongAnswers = { "Enter at least three wrong answers." };
        [SerializeField] [Range(2, 5)] private int choiceCount = 4;
        
        private string selectedQuestion;
        private string selectedCorrectAnswer;
        private string[] selectedChoices;
        
        
        // QUIZ ITEM METHODS
        public override void Initialize(bool isParaphrased)
        {
            // Select a random paraphrased question and correct answer
            selectedQuestion = isParaphrased
                ? question[Random.Range(0, question.Length)]
                : question[0];
            selectedCorrectAnswer = isParaphrased
                ? correctAnswer[Random.Range(0, correctAnswer.Length)]
                : correctAnswer[0];
            
            // Ensure choice count is within valid range
            choiceCount = Mathf.Clamp(choiceCount, 2, wrongAnswers.Length + 1);
            
            // Add correct answer to choices
            selectedChoices = new string[choiceCount];
            selectedChoices[0] = selectedCorrectAnswer;
            
            // Shuffle wrong answers and add to choices
            string[] selectedWrongAnswers = wrongAnswers.CloneArray().Shuffle();
            for (int i = 0; i < choiceCount - 1; i++)
            {
                selectedChoices[i + 1] = selectedWrongAnswers[i];
            }
            
            // Shuffle all choices
            selectedChoices.Shuffle();
        }
        
        public override float CheckAnswerAndGetScore(QuizAnswer quizAnswer, Difficulty? forcedDifficulty = null)
        {
            // Convert to multiple choice answer
            if (quizAnswer is not MultipleChoiceAnswer answer) return 0f;
            
            // Check if answer is correct and return score
            bool isCorrect = answer.answer == selectedCorrectAnswer;
            return isCorrect ? 1f * ((forcedDifficulty == null ? difficulty : forcedDifficulty.Value).AsMultiplier()) : 0f;
        }
        
        public override float GetMaxScore(Difficulty? forcedDifficulty = null) => 1f * (forcedDifficulty == null ? difficulty : forcedDifficulty.Value).AsMultiplier();
        
        public override string GetQuestionAsString() => selectedQuestion;
        
        public override string GetAnswerAsString() => GetCorrectAnswer();
        
        // PUBLIC METHODS
        public string GetQuestion() => selectedQuestion;
        public string[] GetChoices() => selectedChoices;
        public string GetCorrectAnswer() => selectedCorrectAnswer;
    }
}