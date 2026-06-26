using System;
using System.Collections.Generic;
using UnityEngine;
using KBCore.Refs;
using UnityEngine.UI;

namespace ProjectATLAS.Quiz.UI
{
    public class FillInTheBlanksDropDownsPanel : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField, Child] private List<FillInTheBlanksDropArea> dropAreas;
        [SerializeField, Self] private VerticalLayoutGroup verticalLayoutGroup;
        
        public event Action<int, string> OnChoiceDropped;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            for (int i = 0; i < dropAreas.Count; i++)
            {
                dropAreas[i].OnChoiceDropped += DropArea_OnChoiceDropped;
            }
        }
        
        // PUBLIC METHODS
        public void DisplayDropDowns(int count)
        {
            // Handle answer button duplication logic
            int targetChildCount = Mathf.FloorToInt(count);
            int interval = targetChildCount - dropAreas.Count;
            
            if (interval > 0)
            {
                for (int i = 0; i < interval; i++)
                {
                    AddDropArea();
                }
            }
            else if (interval < 0)
            {
                interval = -interval; // flip the sign
                for (int i = 0; i < interval; i++)
                {
                    RemoveDropArea();
                }
            }
        }
        
        public string[] GetAssignedChoices()
        {
            string[] choices = new string[dropAreas.Count];
            for (int i = 0; i < dropAreas.Count; i++)
            {
                choices[i] = dropAreas[i].GetAssignedChoice();
            }
            return choices;
        }
        
        public void AssignAnswer(FillInTheBlanksChoiceView choiceView, int index)
        {
            if (index >= dropAreas.Count)
            {
                Debug.LogWarning("[MatchingQuestionList] Tried to assign a MatchingChoice but index out of range.");
                return;
            }
            
            dropAreas[index].AssignChoice(choiceView);
        }
        
        
        // PRIVATE METHODS
        private void AddDropArea()
        {
            FillInTheBlanksDropArea template = dropAreas[0];
            GameObject newGameObject = Instantiate(template.gameObject, transform);
            
            if (newGameObject.TryGetComponent(out FillInTheBlanksDropArea dropArea))
            {
                dropArea.OnChoiceDropped += DropArea_OnChoiceDropped;
                dropAreas.Add(dropArea);
            }
        }
        
        private void RemoveDropArea()
        {
            int lastIndex = dropAreas.Count - 1;
            if (lastIndex <= 0) return;
            
            FillInTheBlanksDropArea viewToRemove = dropAreas[lastIndex];
            
            viewToRemove.OnChoiceDropped += DropArea_OnChoiceDropped;
            dropAreas.Remove(viewToRemove);
            
            Destroy(viewToRemove.gameObject);
        }
        
        
        // EVENT LISTENER METHODS
        private void DropArea_OnChoiceDropped(int index, string choice)
        {
            OnChoiceDropped?.Invoke(index, choice);
        }
    }
}
