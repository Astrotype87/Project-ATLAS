using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using ProjectATLAS.Gameplay;
using ProjectATLAS.Utility;
using AstrotypeTools.InspectorAttributes;

namespace ProjectATLAS.Quiz
{
    [Serializable]
    public class MatchingItem : QuizItem
    {
        [SerializeField] private List<MatchingPair> pairs = new() { new MatchingPair() };
        [SerializeField] [HideLabel] [Indent(-0.5f)] private string[] wrongAnswers = { "Enter zero or more extra wrong choices." };
        
        private string[] selectedQuestions;
        private string[] selectedCorrectAnswers;
        private string[] selectedChoices;
        
        // PROPERTIES
        public int PairsCount => pairs.Count;
        
        
        // QUIZ ITEM METHODS
        public override void Initialize(bool isParaphrased)
        {
            // Randomize and set selected questions
            for (int i = 0; i < pairs.Count; i++)
            {
                pairs[i].Initialize(isParaphrased);
            }
            
            // Shuffle matching pairs
            MatchingPair[] matchingPairs = pairs.ToArray().Shuffle();
            
            // Select questions and correct answers
            selectedQuestions = new string[matchingPairs.Length];
            selectedCorrectAnswers = new string[matchingPairs.Length];
            
            for (int i = 0; i < matchingPairs.Length; i++)
            {
                selectedQuestions[i] = matchingPairs[i].GetQuestion();
                selectedCorrectAnswers[i] = matchingPairs[i].GetCorrectAnswer();
            }
            
            // Select choices
            selectedChoices = selectedCorrectAnswers.CloneArray();
            if (wrongAnswers != null && wrongAnswers.Length != 0)
            {
                selectedChoices = selectedChoices.Concat(wrongAnswers).ToArray();
            }
            selectedChoices.Shuffle();
        }
        
        public override float CheckAnswerAndGetScore(QuizAnswer quizAnswer, Difficulty? forcedDifficulty = null)
        {
            // Convert to multiple choice answer
            if (quizAnswer is not MatchingAnswer answer) return 0f;
            
            // Calculate and return score
            float points = 0;
            for (int i = 0; i < selectedCorrectAnswers.Length; i++)
            {
                if (selectedCorrectAnswers[i] == answer.answers[i])
                    points++;
            }
            
            return points * (forcedDifficulty == null ? difficulty : forcedDifficulty.Value).AsMultiplier();
        }
        
        public override float GetMaxScore(Difficulty? forcedDifficulty = null) => selectedCorrectAnswers.Length * (forcedDifficulty == null ? difficulty : forcedDifficulty.Value).AsMultiplier();
        
        public override string GetQuestionAsString() => $"{selectedQuestions.Length} matching questions.";
        
        public override string GetAnswerAsString() => string.Join(", ", selectedCorrectAnswers);
        
        
        // PUBLIC METHODS
        public string[] GetQuestions() => selectedQuestions;
        
        public string[] GetChoices() => selectedChoices;
        
        public string[] GetCorrectAnswers() => selectedCorrectAnswers;
    }
    
    [Serializable]
    public class MatchingPair
    {
        [SerializeField] [Indent(-0.5f)] [TextArea(1, 4)] private string[] question;
        [SerializeField] [Indent(-0.5f)] [TextArea(1, 4)] private string correctAnswer;
        
        private string selectedQuestion;
        
        public MatchingPair()
        {
            question = new[] { "Enter question.", "Enter paraphrased question." };
            correctAnswer = "Enter correct answer.";
        }
        
        public void Initialize(bool isParaphrased)
        {
            selectedQuestion = isParaphrased
                ? question[UnityEngine.Random.Range(0, question.Length)]
                : question[0];
        }
        
        public string GetQuestion() => selectedQuestion;
        public string GetCorrectAnswer() => correctAnswer;
    }
}
