using System;
using UnityEngine;


namespace ProjectATLAS.Beta.Quiz
{
    [Serializable]
    public class SequencingItem : QuizItem
    {
        public string[] correctOrderedAnswers;
        public override void Initialize()
        {
        
        }

        public override float CheckAnswerAndGetScore(QuizAnswer answer)
        {
            SequenceAnswer sequenceAnswer = answer as SequenceAnswer;

            if (answer is SequenceAnswer sequencingAnswer)
            {
                string[] userAnswers = sequencingAnswer.orderedAnswers;
                int correctCount = 0;

                for (int i = 0; i < Mathf.Min(correctOrderedAnswers.Length, userAnswers.Length); i++)
                {
                    if (correctOrderedAnswers[i] == userAnswers[i])
                    {
                        correctCount++;
                    }
                }

                return (float)correctCount;
            }

            return 0f; 
        }

        public string GetQuestion()
        {
            return GetQuestion();
        }

        public string[] GetCorrectAnswer()
        {
            return correctOrderedAnswers;
        }
    }
    
}

