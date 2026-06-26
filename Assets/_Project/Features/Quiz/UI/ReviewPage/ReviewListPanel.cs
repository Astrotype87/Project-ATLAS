using System;
using System.Collections.Generic;
using UnityEngine;
using KBCore.Refs;

namespace ProjectATLAS.Quiz.UI
{
    public class ReviewListPanel : MonoBehaviour
    {
        [SerializeField, Child] private List<ReviewItemView> reviewItemViews;
        
        // PROPERTIES
        public event Action<int> OnEditClicked;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            for (int i = 0; i < reviewItemViews.Count; i++)
            {
                reviewItemViews[i].OnEditClicked += ReviewItemView_OnEditClicked;
            }
        }
        
        // PUBLIC METHODS
        public void DisplayReviewItems((string, string, string)[] items,  bool hideDifficulty)
        {
            // Handle review item view duplication logic
            int targetChildCount = Mathf.FloorToInt(items.Length);
            int interval = targetChildCount - reviewItemViews.Count;
            
            if (interval > 0)
            {
                for (int i = 0; i < interval; i++)
                {
                    AddReviewItemView();
                }
            }
            else if (interval < 0)
            {
                interval = -interval; // flip the sign
                for (int i = 0; i < interval; i++)
                {
                    RemoveReviewItemView();
                }
            }
            
            // Update each review item view
            for (int i = 0; i < reviewItemViews.Count; i++)
            {
                reviewItemViews[i].UpdateText(items[i].Item1, items[i].Item2, items[i].Item3, hideDifficulty);
            }
            
            // Update content viewport height
            UpdateViewportHeight();
        }
        
        // PRIVATE METHODS
        private void AddReviewItemView()
        {
            ReviewItemView template = reviewItemViews[0];
            GameObject newGameObject = Instantiate(template.gameObject, transform);
            
            if (newGameObject.TryGetComponent(out ReviewItemView reviewItemView))
            {
                reviewItemView.OnEditClicked += ReviewItemView_OnEditClicked;
                reviewItemViews.Add(reviewItemView);
            }
        }
        
        private void RemoveReviewItemView()
        {
            int lastIndex = reviewItemViews.Count - 1;
            if (lastIndex <= 0) return;
            
            ReviewItemView viewToRemove = reviewItemViews[lastIndex];
            
            viewToRemove.OnEditClicked -= ReviewItemView_OnEditClicked;
            reviewItemViews.Remove(viewToRemove);
            
            Destroy(viewToRemove.gameObject);
        }
        
        private void UpdateViewportHeight()
        {
            float height = 0;
            float verticalSpacing = 30;
            height -= verticalSpacing;
            
            for (int i = 0; i < transform.childCount; i++)
            {
                RectTransform childRectTransform = transform.GetChild(i).gameObject.GetComponent<RectTransform>();
                height += childRectTransform.sizeDelta.y + verticalSpacing;
            }
            
            RectTransform rectTransform = transform as RectTransform;
            Vector2 sizeDelta = rectTransform.sizeDelta;
            sizeDelta.y = height;
            rectTransform.sizeDelta = sizeDelta;
        }
        
        
        // EVENT LISTENER METHODS
        private void ReviewItemView_OnEditClicked(int index)
        {
            OnEditClicked?.Invoke(index);
        }
    }
}
