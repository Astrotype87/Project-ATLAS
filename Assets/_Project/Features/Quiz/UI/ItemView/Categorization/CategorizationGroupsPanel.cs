using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using KBCore.Refs;

using ProjectATLAS.UI;
using static ProjectATLAS.Quiz.CategorizationItem;

namespace ProjectATLAS.Quiz.UI
{
    public class CategorizationGroupsPanel : MonoBehaviour
    {
        [SerializeField, Child] private List<CategorizationGroupContainer> groupContainers;
        [SerializeField, Self] private HorizontalLayoutGroup horizontalLayoutGroup;
        
        // PROPERTIES
        public event Action<string, string[]> OnAnswersUpdated; // string category, string answer
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            for (int i = 0; i < groupContainers.Count; i++)
            {
                groupContainers[i].OnAnswerDropped += GroupContainer_OnAnswerDropped;
            }
        }
        
        private void OnValidate()
        {
            this.ValidateRefs();
        }
        
        
        // PUBLIC METHODS
        public void DisplayCategories(string[] categories)
        {
            int targetChildCount = Mathf.FloorToInt(categories.Length);
            int interval = targetChildCount - groupContainers.Count;
            
            if (interval > 0)
            {
                for (int i = 0; i < interval; i++)
                {
                    AddCategoryContainer();
                }
            }
            else if (interval < 0)
            {
                interval = -interval; // flip the sign
                for (int i = 0; i < interval; i++)
                {
                    RemoveCategoryContainer();
                }
            }
            
            // Update answer view text
            for (int i = 0; i < groupContainers.Count; i++)
            {
                groupContainers[i].DisplayCategory(categories[i]);
            }
        }
        
        public void AssignAnswerToCategory(string category, CategorizationAnswerView answerView)
        {
            CategorizationGroupContainer groupContainer = groupContainers.Where(c => c.Category == category).First();
            groupContainer.AssignAnswer(answerView);
        }
        
        public CategoryItems[] GetAssignedAnswersPerCategory()
        {
            CategoryItems[] categoryItems = new CategoryItems[groupContainers.Count];
            
            for (int i = 0; i < groupContainers.Count; i++)
            {
                categoryItems[i].category = groupContainers[i].Category;
                categoryItems[i].items = groupContainers[i].GetAssignedAnswers();
            }
            
            return categoryItems;
        }
        
        
        // PRIVATE METHODS
        private void AddCategoryContainer()
        {
            CategorizationGroupContainer template = groupContainers[0];
            GameObject newGameObject = Instantiate(template.gameObject, transform);
            
            if (newGameObject.TryGetComponent(out CategorizationGroupContainer answerView))
            {
                answerView.OnAnswerDropped += GroupContainer_OnAnswerDropped;
                groupContainers.Add(answerView);
            }
        }
        
        private void RemoveCategoryContainer()
        {
            int lastIndex = groupContainers.Count - 1;
            if (lastIndex <= 0) return;
            
            CategorizationGroupContainer viewToRemove = groupContainers[lastIndex];
            
            viewToRemove.OnAnswerDropped -= GroupContainer_OnAnswerDropped;
            groupContainers.Remove(viewToRemove);
            
            Destroy(viewToRemove.gameObject);
        }
        
        
        // EVENT LISTENER METHODS
        private void GroupContainer_OnAnswerDropped(string category, string[] answers)
        {
            OnAnswersUpdated?.Invoke(category, answers);
        }
    }
}
