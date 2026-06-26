using System;
using UnityEngine;

using static ProjectATLAS.Quiz.CategorizationItem;

namespace ProjectATLAS.Quiz.UI
{
    public class CategorizationView : MonoBehaviour
    {
        [SerializeField] private CategorizationGroupsPanel categorizationGroupsPanel;
        [SerializeField] private CategorizationAnswersPanel categorizationAnswersPanel;
        
        private CategoryItems[] answers;
        
        
        // PROPERTIES
        public event Action<CategoryItems[]> OnAnswerUpdated; // <string category, string answer>
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            categorizationGroupsPanel.OnAnswersUpdated += CategorizationGroupsPanel_OnAnswerDropped;
            categorizationAnswersPanel.OnContainedAnswersChanged += MatchingAnswersPanel_OnContainedAnswersChanged;
        }
        
        
        // PUBLIC METHODS
        public void DisplayItem(CategorizationItem categorizationItem, CategorizationAnswer categorizationAnswer)
        {
            // Initialize answers variable
            string[] choices = categorizationItem.GetChoices();
            string[] categories = categorizationItem.GetCategories();
            CategoryItems[] answersPerCategory;
            
            if (categorizationAnswer == null || categorizationAnswer.answers == null || categorizationAnswer.answers.Length == 0)
                answersPerCategory = categorizationItem.GetEmptyCategoryItems();
            else
                answersPerCategory = categorizationAnswer.answers;
            
            // Display answers
            categorizationAnswersPanel.DisplayAnswers(choices);
            categorizationGroupsPanel.DisplayCategories(categories);
            
            // Assign answers
            for (int i = 0; i < answersPerCategory.Length; i++)
            {
                string category = answersPerCategory[i].category;
                string[] items = answersPerCategory[i].items;
                if (items == null || items.Length == 0) continue;
                
                for (int j = 0; j < items.Length; j++)
                {
                    string item = items[j];
                    CategorizationAnswerView answerView = categorizationAnswersPanel.GetAnswerView(item);
                    if (answerView == null) continue;
                    
                    categorizationGroupsPanel.AssignAnswerToCategory(category, answerView);
                }
            }
            
            this.answers = answersPerCategory;
            OnAnswerUpdated?.Invoke(this.answers);
        }
        
        // EVENT LISTENER METHODS
        private void CategorizationGroupsPanel_OnAnswerDropped(string category, string[] answers)
        {
            this.answers = categorizationGroupsPanel.GetAssignedAnswersPerCategory();
            OnAnswerUpdated?.Invoke(this.answers);
        }
        
        private void MatchingAnswersPanel_OnContainedAnswersChanged()
        {
            this.answers = categorizationGroupsPanel.GetAssignedAnswersPerCategory();
            OnAnswerUpdated?.Invoke(this.answers);
        }
    }
}
