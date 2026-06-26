using System;
using UnityEngine;

namespace ProjectATLAS.Quiz
{
    public struct QuizResultData
    {
        public float score;
        public float maxScore;
        public TimeSpan time;
        public ItemResult[] itemResults;
        
        public int ResultCount => itemResults != null? itemResults.Length : 0;
        public float ScorePercentage => score / maxScore;
        
        public struct ItemResult
        {
            public bool isCorrect;
            public string question;
            public string points;
            public string difficulty;
            public string answer;
            public string correct;
        }
    }
}