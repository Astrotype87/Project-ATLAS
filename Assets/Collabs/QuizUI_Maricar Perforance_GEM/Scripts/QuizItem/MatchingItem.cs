using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectATLAS.Beta.Quiz
{
    [Serializable]
    public class MatchingItem : QuizItem
    {
        public string[] wrongAnswers;

        [Serializable]
        public class MatchingPair
        {
            public string[] question;
            public string correctAnswer;
            [NonSerialized] private string selectedQuestion;

            public void Initialize()
            {
                selectedQuestion = question[UnityEngine.Random.Range(0, question.Length)];
            }

            public string GetQuestion()
            {
                return selectedQuestion;
            }
        }

        public List<MatchingPair> pairs;

        public override void Initialize()
        {
            foreach (var pair in pairs)
            {
                pair.Initialize();
            }
        }

        public override float CheckAnswerAndGetScore(QuizAnswer answer)
        {
            MatchingAnswer matchingAnswer = answer as MatchingAnswer;

            if (matchingAnswer == null)
            {
                Debug.LogWarning("Invalid answer type passed to MatchingItem.");
                return 0.0f;
            }

            int correctCount = 0;
            int total = Mathf.Min(matchingAnswer.answers.Length, pairs.Count);

            for (int i = 0; i < total; i++)
            {
                if (matchingAnswer.answers[i] == pairs[i].correctAnswer)
                {
                    correctCount++;
                }
            }

            return (float)correctCount / total;
        }

        public List<string> GetQuestions()
        {
            List<string> result = new List<string>();
            foreach (var pair in pairs)
            {
                result.Add(pair.GetQuestion());
            }
            return result;
        }

        public List<string> GetAnswers()
        {
            List<string> result = new List<string>();
            foreach (var pair in pairs)
            {
                result.Add(pair.correctAnswer);
            }
            return result;
        }
    }
    
}
