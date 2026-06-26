using System;
using UnityEngine;

namespace ProjectATLAS.Beta.Quiz
{
    [Serializable]
    public class MultipleChoiceItem : QuizItem
    {
        public string[] question;
        public string[] correctAnswer;
        public string[] wrongAnswer;
        public int choiceCount = 4;
        public float scoreMultiplier;
        public Difficulty difficulty;

        private string selectedQuestion;
        private string selectedCorrectAnswer;
        private string[] selectedChoices;

        public override void Initialize()
        {
            selectedQuestion = question[UnityEngine.Random.Range(0, question.Length)];
            selectedCorrectAnswer = correctAnswer[UnityEngine.Random.Range(0, correctAnswer.Length)];

            choiceCount = Mathf.Clamp(choiceCount, 2, wrongAnswer.Length);

            selectedChoices = new string[choiceCount];
            selectedChoices[0] = selectedCorrectAnswer;

            string[] selectedWrongAnswers = ArrayUtils.Shuffle(wrongAnswer);
            for (int i = 1; i < choiceCount; i++)
            {
                selectedChoices[i] = selectedWrongAnswers[i];
            }

            selectedChoices = ArrayUtils.Shuffle(selectedChoices);

        }

        public override float CheckAnswerAndGetScore(QuizAnswer answer)
        {
            MultipleChoiceAnswer multipleChoiceAnswer = answer as MultipleChoiceAnswer;
            if (multipleChoiceAnswer == null) return 0;

            bool isCorrect = multipleChoiceAnswer.answer == selectedCorrectAnswer;

            float score = isCorrect ? 1f * GetDifficultyMultiplier(difficulty) : 0f;
            return score;
        }

        public string GetQuestion()
        {
            return selectedQuestion;
        }
        public string[] GetChoices()
        {
            return selectedChoices;
        }
        public string GetCorrectAnswer()
        {
            return selectedCorrectAnswer;
        }
        private float GetDifficultyMultiplier(Difficulty difficulty)
        {
            return difficulty switch
            {
                Difficulty.Easy => 1f,
                Difficulty.Medium => 2f,
                Difficulty.Hard => 3f,
                _ => 1f
            };
        }
    
    }
}
