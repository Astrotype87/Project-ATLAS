using System;
using System.Collections.Generic;
using UnityEngine;
using KBCore.Refs;

namespace ProjectATLAS.Library.Glossary
{
    public class GlossaryTermsPanel : MonoBehaviour
    {
        [SerializeField, Child] private List<TermView> termViews;
        
        public event Action<TermResult> OnTermClicked;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            foreach (var termView in termViews)
            {
                termView.OnClicked += TermView_OnClicked;
            }
        }
        
        // PUBLIC METHODS
        public void DisplayTermResults(TermResult[] termResults)
        {
            // Handle terms duplication logic
            int targetContainerCount = (int)Math.Ceiling(termResults.Length / 2.0);
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
            
            // Disable each term views
            for (int i = 0; i < termViews.Count; i++)
            {
                termViews[i].SetVisible(false);
            }
            
            // Update term views
            for (int i = 0; i < termResults.Length; i++)
            {
                termViews[i].DisplayTerm(termResults[i]);
                termViews[i].SetVisible(true);
            }
        }
        
        private void DuplicateTwoColumnContainer()
        {
            // Duplicate game object template
            GameObject template = transform.GetChild(0).gameObject;
            GameObject newGameObject = Instantiate(template, transform);
            
            // Get child term views
            var childTermViews = newGameObject.GetComponentsInChildren<TermView>();
            foreach (var termView in childTermViews)
            {
                termView.OnClicked += TermView_OnClicked;
                termViews.Add(termView);
            }
        }
        
        private void DestroyLastTwoColumnContainer(List<GameObject> childObjects)
        {
            int lastIndex = childObjects.Count - 1;
            if (lastIndex <= 0) return;
            
            // Get last gameobject
            GameObject gameObject = childObjects[^1];
            
            // Get child term views
            var childTermViews = gameObject.GetComponentsInChildren<TermView>();
            foreach (var termView in childTermViews)
            {
                termView.OnClicked -= TermView_OnClicked;
                termViews.Remove(termView);
            }
            
            // Destroy two column container
            Destroy(gameObject);
            childObjects.Remove(gameObject);
        }
        
        // EVENT LISTENER METHODS
        private void TermView_OnClicked(TermResult termResult)
        {
            OnTermClicked?.Invoke(termResult);
        }
    }
}
