using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AstrotypeTools.InspectorAttributes;

using ProjectATLAS.Gameplay;
using ProjectATLAS.Utility;

namespace ProjectATLAS.Quiz
{
    public class CategorizationItem : QuizItem
    {
        [SerializeField] [Indent(-1, applyToCollection: true)]
        private List<CategoryItems> categoryItems = new()
        {
            new("Fundamental Quantities", "Length", "Mass", "Time"),
            new("Derived Quantities", "Force", "Energy", "Power")
        };
        
        private string[] selectedCategories;
        private string[] selectedChoices;
        private CategoryItems[] selectedCategoryItems;
        
        
        // QUIZ ITEM METHODS
        public override void Initialize(bool isParaphrased)
        {
            // Get randomized correct category items
            selectedCategoryItems = categoryItems.ToArray().Shuffle();
            List<string> choices = new();
            
            // Get categories
            selectedCategories = selectedCategoryItems.Select(c => c.category).ToArray();
            
            // Get choices
            foreach (var categoryItem in categoryItems)
            {
                foreach (var item in categoryItem.items)
                {
                    choices.Add(item);
                }
            }
            selectedChoices = choices.ToArray().Shuffle();
        }
        
        public override float CheckAnswerAndGetScore(QuizAnswer quizAnswer, Difficulty? forcedDifficulty = null)
        {
            // Convert to sequence answer
            if (quizAnswer is not CategorizationAnswer answer) return 0f;
            
            int totalScore = 0;
            for (int i = 0; i < selectedCategoryItems.Length; i++)
            {
                // Check one category
                CategoryItems categoryItems = selectedCategoryItems[i];
                HashSet<string> correctAnswers = categoryItems.items.ToHashSet();
                string[] submittedAnswers = answer.GetAnswersByCategory(categoryItems.category);
                if (submittedAnswers == null) continue;
                
                // Check if correctAnswers for one category is contained in submittedAnswers
                foreach (string submittedAnswer in submittedAnswers)
                {
                    if (correctAnswers.Contains(submittedAnswer))
                    {
                        totalScore++;
                        correctAnswers.Remove(submittedAnswer); // Avoid double counting
                    }
                }
            }
            
            return totalScore * (forcedDifficulty == null ? difficulty : forcedDifficulty.Value).AsMultiplier();
        }
        
        public override float GetMaxScore(Difficulty? forcedDifficulty = null)
        {
            float totalScore = 0;
            for (int i = 0; i < categoryItems.Count; i++)
            {
                totalScore += categoryItems[i].items.Length;
            }
            return totalScore * (forcedDifficulty == null ? difficulty : forcedDifficulty.Value).AsMultiplier();
        }
        
        public override string GetQuestionAsString() => $"Categorization: ({string.Join(", ", categoryItems.Select(c => c.category))})";
        public override string GetAnswerAsString()
        {
            string answerList = "";
            foreach (var categoryItem in selectedCategoryItems)
            {
                // Category: [Item1, item2, item3] \n 
                answerList += $"{categoryItem.category} : {string.Join(", ", categoryItem.items)}\n";
            }
            return answerList;
        }
        
        
        // PUBLIC METHODS
        public string[] GetCategories() => selectedCategories;
        public string[] GetChoices() => selectedChoices;
        public CategoryItems[] GetCategoryItems() => selectedCategoryItems;
        public CategoryItems[] GetEmptyCategoryItems()
        {
            CategoryItems[] emptyCategoryItems = new CategoryItems[selectedCategoryItems.Length];
            for (int i = 0; i < emptyCategoryItems.Length; i++)
            {
                emptyCategoryItems[i].category = selectedCategoryItems[i].category;
            }
            return emptyCategoryItems;
        }
        
        
        [Serializable]
        public struct CategoryItems
        {
            [Indent(-1)] public string category;
            [HideLabel] public string[] items;
            
            public CategoryItems(string category, params string[] items)
            {
                this.category = category;
                this.items = items;
            }
        }
    }
}
