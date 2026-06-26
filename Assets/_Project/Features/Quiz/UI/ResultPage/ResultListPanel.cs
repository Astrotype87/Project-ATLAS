using System.Collections.Generic;
using UnityEngine;
using KBCore.Refs;

namespace ProjectATLAS.Quiz.UI
{
    public class ResultListPanel : MonoBehaviour
    {
        [SerializeField, Child] private List<ResultItemView> resultItemViews;
        
        
        // PUBLIC METHODS
        public void DisplayResultData(QuizResultData resultData)
        {
            // Handle result item view duplication logic
            int targetChildCount = Mathf.FloorToInt(resultData.ResultCount);
            int interval = targetChildCount - resultItemViews.Count;
            
            if (interval > 0)
            {
                for (int i = 0; i < interval; i++)
                {
                    AddResultItemView();
                }
            }
            else if (interval < 0)
            {
                interval = -interval; // flip the sign
                for (int i = 0; i < interval; i++)
                {
                    RemoveResultItemView();
                }
            }
            
            // Update each result item view
            for (int i = 0; i < resultItemViews.Count; i++)
            {
                QuizResultData.ItemResult itemResult = resultData.itemResults[i];
                resultItemViews[i].DisplayResult(itemResult);
            }
            
            // Update content viewport height
            UpdateViewportHeight();
        }
        
        
        // PRIVATE METHODS
        private void AddResultItemView()
        {
            ResultItemView template = resultItemViews[0];
            GameObject newGameObject = Instantiate(template.gameObject, transform);
            
            if (newGameObject.TryGetComponent(out ResultItemView resultItemView))
            {
                resultItemViews.Add(resultItemView);
            }
        }
        
        private void RemoveResultItemView()
        {
            int lastIndex = resultItemViews.Count - 1;
            if (lastIndex <= 0) return;
            
            ResultItemView viewToRemove = resultItemViews[lastIndex];
            
            resultItemViews.Remove(viewToRemove);
            
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
    }
}
