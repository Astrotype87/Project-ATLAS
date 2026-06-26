using System;
using System.Collections.Generic;
using UnityEngine;
using KBCore.Refs;
using UnityEngine.UI;

using ProjectATLAS.UI;

namespace ProjectATLAS.Quiz.UI
{
    public class FillInTheBlanksChoicePanel : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField, Child] private List<FillInTheBlanksChoiceView> choiceViews;
        [SerializeField, Self] private HorizontalLayoutGroup horizontalLayoutGroup;
        [SerializeField] private UIDropArea dropArea;
        
        private int previousChildCount;
        
        // PROPERTIES
        public event Action OnContainedChoicesChanged;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            previousChildCount = transform.childCount;
        }
        
        private void Update()
        {
            if (transform.childCount != previousChildCount)
            {
                OnContainedChoicesChanged?.Invoke();
                previousChildCount = transform.childCount;
            }
        }
        
        // PUBLIC METHODS
        public void DisplayChoices(string[] choices)
        {
            // Drop all choice views back to choices list container text
            for (int i = 0; i < choiceViews.Count; i++)
            {
                dropArea.DropObject(choiceViews[i].gameObject);
            }
            
            // Handle choice button duplication logic
            int targetChildCount = Mathf.FloorToInt(choices.Length);
            int interval = targetChildCount - choiceViews.Count;
            
            if (interval > 0)
            {
                for (int i = 0; i < interval; i++)
                {
                    AddChoiceView();
                }
            }
            else if (interval < 0)
            {
                interval = -interval; // flip the sign
                for (int i = 0; i < interval; i++)
                {
                    RemoveChoiceView();
                }
            }
            
            // Update choice view text
            for (int i = 0; i < choiceViews.Count; i++)
            {
                choiceViews[i].DisplayChoice(choices[i]);
            }
        }
        
        public FillInTheBlanksChoiceView GetChoiceView(string choice)
        {
            for (int i = 0; i < choiceViews.Count; i++)
            {
                if (choiceViews[i].Choice == choice) return choiceViews[i];
            }
            return null;
        }
        
        
        // PRIVATE METHODS
        private void AddChoiceView()
        {
            FillInTheBlanksChoiceView template = choiceViews[0];
            GameObject newGameObject = Instantiate(template.gameObject, transform);
            
            if (newGameObject.TryGetComponent(out FillInTheBlanksChoiceView choiceView))
            {
                choiceViews.Add(choiceView);
            }
        }
        
        private void RemoveChoiceView()
        {
            int lastIndex = choiceViews.Count - 1;
            if (lastIndex <= 0) return;
            
            FillInTheBlanksChoiceView viewToRemove = choiceViews[lastIndex];
            
            choiceViews.Remove(viewToRemove);
            
            Destroy(viewToRemove.gameObject);
        }
    }
}
