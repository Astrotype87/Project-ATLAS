using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using AstrotypeTools.ClassSelector;

using ProjectATLAS.Gameplay;
using ProjectATLAS.Utility;

namespace ProjectATLAS.Quiz
{
    /// <summary>
    /// Scriptable object for a single quiz document
    /// which contains a list of various quiz items.
    /// </summary>
    [CreateAssetMenu(fileName = "QuizData", menuName = "Scriptable Objects/QuizData")]
    public class QuizData : ScriptableObject
    {
        [Header("Details")]
        [SerializeField] private string level = "Level 00";
        [SerializeField] private new string name = "Enter topic here.";
        [SerializeField] [TextArea] private string description = "Enter description here.";
        
        [Header("Content")]
        [SerializeReference, ClassSelector]
        [SerializeField] private List<QuizItem> quizItems;
        
        public string Level => level;
        public string Name => name;
        public string Description => description;
        
        
        // PUBLIC STATIC METHODS
        public static string GetQuizDetails(List<QuizItem> quizItems)
        {
            int multipleChoice = 0;
            int trueOrFalse = 0;
            int matching = 0;
            int fillInTheBlanks = 0;
            int sequence = 0;
            int categorization = 0;
            int solving = 0;
            
            for (int i = 0; i < quizItems.Count; i++)
            {
                QuizItem quizItem = quizItems[i];
                
                if (quizItem is MultipleChoiceItem) multipleChoice++;
                else if (quizItem is TrueOrFalseItem) trueOrFalse++;
                else if (quizItem is MatchingItem) matching++;
                else if (quizItem is FillInTheBlanksItem) fillInTheBlanks++;
                else if (quizItem is SequenceItem) sequence++;
                else if (quizItem is CategorizationItem) categorization++;
                else if (quizItem is SolvingItem) solving++;
            }
            
            string text = "";
            if (multipleChoice > 0) text += $"- {multipleChoice} Multiple Choice\n";
            if (trueOrFalse > 0) text += $"- {trueOrFalse} True Or False\n";
            if (matching > 0) text += $"- {matching} Matching\n";
            if (fillInTheBlanks > 0) text += $"- {fillInTheBlanks} Fill In The Blanks\n";
            if (sequence > 0) text += $"- {sequence} Sequence\n";
            if (categorization > 0) text += $"- {categorization} Categorization\n";
            if (solving > 0) text += $"- {solving} Solving\n";
            
            return text;
        }
        
        
        // PUBLIC METHODS
        public static List<QuizItem> CombineQuizItems(QuizData[] quizDatas)
        {
            List<QuizItem> quizItems = new();
            foreach (var quizData in quizDatas)
            {
                quizItems.AddRange(quizData.quizItems);
            }
            return quizItems;
        }
        
        public static List<QuizItem> GenerateQuizItems(List<QuizItem> quizItems, QuizSettings quizSettings, Difficulty difficulty)
        {
            List<QuizItem> resultQuizItems = new();
            
            // Filter quiz item types
            List<QuizItem> filteredItems = quizItems
                .Where(item => IsAllowedItem(item, quizSettings.itemTypes))
                .ToList();
            
            // Filter difficulty
            resultQuizItems = difficulty switch
            {
                Difficulty.Easy   => GetEasyItems(filteredItems, quizSettings),
                Difficulty.Medium => GetMediumItems(filteredItems, quizSettings),
                Difficulty.Hard   => GetHardItems(filteredItems, quizSettings),
                _ => new List<QuizItem>() // ensure no nulls
            };
            
            // Initialize quiz items
            for (int i = 0; i < resultQuizItems.Count; i++)
            {
                bool isParaphrased = Random.value < quizSettings.paraphraseChance;
                resultQuizItems[i].Initialize(isParaphrased);
            }
            
            // Return result
            return resultQuizItems;
        }
        
        public static float GetTotalScore(List<QuizItem> quizItems, Difficulty? forcedDifficulty = null)
        {
            float totalScore = 0;
            for (int i = 0; i < quizItems.Count; i++)
            {
                totalScore += quizItems[i].GetMaxScore(forcedDifficulty);
            }
            return totalScore;
        }
        
        
        // PRIVATE METHODS
        private static bool IsAllowedItem(QuizItem item, ItemTypes allowedTypes)
        {
            return item switch
            {
                MultipleChoiceItem => allowedTypes.HasFlag(ItemTypes.MultipleChoice),
                TrueOrFalseItem    => allowedTypes.HasFlag(ItemTypes.TrueOrFalse),
                MatchingItem       => allowedTypes.HasFlag(ItemTypes.Matching),
                FillInTheBlanksItem => allowedTypes.HasFlag(ItemTypes.FillInTheBlanks),
                SequenceItem       => allowedTypes.HasFlag(ItemTypes.Sequence),
                CategorizationItem => allowedTypes.HasFlag(ItemTypes.Categorization),
                SolvingItem        => allowedTypes.HasFlag(ItemTypes.Solving),
                _ => false
            };
        }
        
        private static List<QuizItem> GetItemsByDifficulty(
            List<QuizItem> quizItems,
            QuizSettings quizSettings,
            Difficulty primaryDifficulty,
            Difficulty[] fallbackDifficulties = null)
        {
            int totalItems = quizSettings.totalItems;
            List<QuizItem> result = new();
            
            // Get items with primary difficulty
            QuizItem[] primaryItems = quizItems.Where(i => i.Difficulty == primaryDifficulty).ToArray();
            if (quizSettings.shuffled) primaryItems = primaryItems.Shuffle();
            
            for (int i = 0; i < primaryItems.Length && totalItems > 0; i++)
            {
                result.Add(primaryItems[i]);
                totalItems--;
            }
            
            // Allowed difficulties
            if (quizSettings.includeLowerDifficulty
                && totalItems > 0
                && fallbackDifficulties != null)
            {
                foreach (var fallback in fallbackDifficulties)
                {
                    // Get items with current difficulty
                    QuizItem[] fallbackItems = quizItems.Where(i => i.Difficulty == fallback).ToArray();
                    if (quizSettings.shuffled) fallbackItems = fallbackItems.Shuffle();
                    
                    for (int i = 0; i < fallbackItems.Length && totalItems > 0; i++)
                    {
                        result.Add(fallbackItems[i]);
                        totalItems--;
                    }
                    
                    if (totalItems <= 0) break;
                }
            }
            
            if (quizSettings.shuffled) result = result.Shuffle();
            return result;
        }
        
        private static List<QuizItem> GetEasyItems(List<QuizItem> quizItems, QuizSettings quizSettings)
        {
            return GetItemsByDifficulty(quizItems, quizSettings, Difficulty.Easy);
        }
        
        private static List<QuizItem> GetMediumItems(List<QuizItem> quizItems, QuizSettings quizSettings)
        {
            return GetItemsByDifficulty(quizItems, quizSettings, Difficulty.Medium, new[] { Difficulty.Easy });
        }
        
        private static List<QuizItem> GetHardItems(List<QuizItem> quizItems, QuizSettings quizSettings)
        {
            return GetItemsByDifficulty(quizItems, quizSettings, Difficulty.Hard, new[] { Difficulty.Medium, Difficulty.Easy });
        }
        
        
        
        // private static List<QuizItem> GetEasyItems(List<QuizItem> quizItems, QuizSettings quizSettings)
        // {
        //     int totalItems = quizSettings.totalItems;
            
        //     List<QuizItem> easyQuizItems = new();
            
        //     QuizItem[] easyItems = quizItems.Where(i => i.Difficulty == Difficulty.Easy).ToArray().Shuffle();
        //     for (int i = 0; i < easyItems.Length; i++)
        //     {
        //         if (totalItems <= 0) break;
        //         easyQuizItems.Add(easyItems[i]);
        //         totalItems--;
        //     }
            
        //     return easyQuizItems.Shuffle();
        // }
        
        // private static List<QuizItem> GetMediumItems(List<QuizItem> quizItems, QuizSettings quizSettings)
        // {
        //     int totalItems = quizSettings.totalItems;
            
        //     List<QuizItem> mediumQuizItems = new();
            
        //     QuizItem[] mediumItems = quizItems.Where(i => i.Difficulty == Difficulty.Medium).ToArray().Shuffle();
        //     for (int i = 0; i < mediumItems.Length; i++)
        //     {
        //         if (totalItems <= 0) break;
        //         mediumQuizItems.Add(mediumItems[i]);
        //         totalItems--;
        //     }
            
        //     if (!quizSettings.includeLowerDifficulty)
        //         return mediumQuizItems.Shuffle();
            
        //     QuizItem[] easyItems = quizItems.Where(i => i.Difficulty == Difficulty.Easy).ToArray().Shuffle();
        //     for (int i = 0; i < easyItems.Length; i++)
        //     {
        //         if (totalItems <= 0) break;
        //         mediumQuizItems.Add(easyItems[i]);
        //         totalItems--;
        //     }
            
        //     return mediumQuizItems.Shuffle();
        // }
        
        // private static List<QuizItem> GetHardItems(List<QuizItem> quizItems, QuizSettings quizSettings)
        // {
        //     int totalItems = quizSettings.totalItems;
            
        //     List<QuizItem> hardQuizItems = new();
            
        //     QuizItem[] hardItems = quizItems.Where(i => i.Difficulty == Difficulty.Hard).ToArray().Shuffle();
        //     for (int i = 0; i < hardItems.Length; i++)
        //     {
        //         if (totalItems <= 0) break;
        //         hardQuizItems.Add(hardItems[i]);
        //         totalItems--;
        //     }
            
        //     if (!quizSettings.includeLowerDifficulty)
        //         return hardQuizItems.Shuffle();
            
        //     QuizItem[] mediumItems = quizItems.Where(i => i.Difficulty == Difficulty.Medium).ToArray().Shuffle();
        //     for (int i = 0; i < mediumItems.Length; i++)
        //     {
        //         if (totalItems <= 0) break;
        //         hardQuizItems.Add(mediumItems[i]);
        //         totalItems--;
        //     }
            
        //     QuizItem[] easyItems = quizItems.Where(i => i.Difficulty == Difficulty.Easy).ToArray().Shuffle();
        //     for (int i = 0; i < easyItems.Length; i++)
        //     {
        //         if (totalItems <= 0) break;
        //         hardQuizItems.Add(easyItems[i]);
        //         totalItems--;
        //     }
            
        //     return hardQuizItems.Shuffle();
        // }
        
    }
}
