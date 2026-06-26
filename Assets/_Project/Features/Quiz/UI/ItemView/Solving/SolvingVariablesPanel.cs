using System;
using System.Collections.Generic;
using UnityEngine;
using KBCore.Refs;

namespace ProjectATLAS.Quiz.UI
{
    public class SolvingVariablesPanel : MonoBehaviour
    {
        [SerializeField, Child] private List<SolvingVariableView> variableViews;
        
        // PUBLIC METHODS
        public void DisplayVariables(SolvingItem.SolvingVariable[] variables, int rounding)
        {
            // Handle variables duplication logic
            int targetContainerCount = (int)Math.Ceiling(variables.Length / 2.0);
            int interval = targetContainerCount - transform.childCount;
            
            List<GameObject> childContainers = new();
            for (int i = 0; i < transform.childCount; i++)
                childContainers.Add(transform.GetChild(i).gameObject);
            
            // Duplicate or destroy two column container
            if (interval > 0)
            {
                for (int i = 0; i < interval; i++)
                {
                    DuplicateTwoColumnContainer();
                }
            }
            else if (interval < 0)
            {
                interval = -interval; // flip the sign
                for (int i = 0; i < interval; i++)
                {
                    if (childContainers.Count == 1) break; // Don't destroy last
                    DestroyLastTwoColumnContainer(childContainers);
                }
            }
            
            // Disable each variable views
            foreach (var variableView in variableViews)
            {
                variableView.SetVisible(false);
            }
            
            // Update variable views
            for (int i = 0; i < variables.Length; i++)
            {
                SolvingItem.SolvingVariable v = variables[i];
                variableViews[i].DisplayVariable(v.Variable, v.Name, v.Value, v.Unit, rounding);
                variableViews[i].SetVisible(true);
            }
        }
        
        private void DuplicateTwoColumnContainer()
        {
            // Duplicate game object template
            GameObject template = transform.GetChild(0).gameObject;
            GameObject newGameObject = Instantiate(template, transform);
            
            // Get child variable views
            var childVariableViews = newGameObject.GetComponentsInChildren<SolvingVariableView>();
            foreach (var variableView in childVariableViews)
            {
                variableViews.Add(variableView);
            }
        }
        
        private void DestroyLastTwoColumnContainer(List<GameObject> childObjects)
        {
            int lastIndex = childObjects.Count - 1;
            if (lastIndex <= 0) return;
            
            // Get last gameobject
            GameObject gameObject = childObjects[^1];
            
            // Get child variable views
            var childVariableViews = gameObject.GetComponentsInChildren<SolvingVariableView>();
            foreach (var variableView in childVariableViews)
            {
                variableViews.Remove(variableView);
            }
            
            // Destroy two column container
            Destroy(gameObject);
            childObjects.Remove(gameObject);
        }
    }
}
