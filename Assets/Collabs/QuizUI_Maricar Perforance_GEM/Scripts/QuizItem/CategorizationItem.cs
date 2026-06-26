using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace ProjectATLAS.Beta.Quiz
{
    
    [Serializable]
    public class CategorizationItem : QuizItem
    {

        public struct CategoryItem
        {
            public string category;
            public string[] items;
        }
        public List<CategoryItem> categoryItems;
        public override void Initialize()
        {
            
        }

        public override float CheckAnswerAndGetScore(QuizAnswer answer)
        {
            CategorizationAnswer categorizationAnswer = answer as CategorizationAnswer;
            if (categorizationAnswer == null)
                return 0f;

            // int totalCorrect = 0;
            // int totalItems = 0;

            return 0; // FFIX LATER
        }

        public string GetQuestion()
        {
            return "Categorize the following items into their correct categories.";
        }

        public List<CategoryItem> GetCorrectAnswer()
        {
            return categoryItems;
        }
    }
}
