using UnityEngine;
using KBCore.Refs;
using System.Collections.Generic;

namespace ProjectATLAS.Gameplay.UI
{
    public class PointsListPanel : MonoBehaviour
    {
        [SerializeField, Child] private List<PointsItemView> pointsItemViews;
        
        // PUBLIC METHODS
        public void DisplayPointsEntries(PointsEntry[] pointsEntries)
        {
            // Handle points item view duplication logic
            int targetChildCount = Mathf.FloorToInt(pointsEntries.Length);
            int interval = targetChildCount - transform.childCount;
            
            if (interval > 0)
            {
                for (int i = 0; i < interval; i++)
                {
                    AddPointsItemView();
                }
            }
            else if (interval < 0)
            {
                interval = -interval; // flip the sign
                for (int i = 0; i < interval; i++)
                {
                    RemovePointsItemView();
                }
            }
            
            // Update each points item view
            for (int i = 0; i < pointsItemViews.Count; i++)
            {
                pointsItemViews[i].DisplayPoints(pointsEntries[i]);
            }
        }
        
        
        // PRIVATE METHODS
        private void AddPointsItemView()
        {
            GameObject gameObject = transform.GetChild(0).gameObject;
            GameObject newGameObject = Instantiate(gameObject, transform);
            
            if (newGameObject.TryGetComponent(out PointsItemView pointsItemView))
            {
                pointsItemViews.Add(pointsItemView);
            }
        }
        
        private void RemovePointsItemView()
        {
            int index = transform.childCount - 1;
            GameObject gameObject = transform.GetChild(index).gameObject;
            
            if (gameObject.TryGetComponent(out PointsItemView pointsItemView))
            {
                pointsItemViews.Remove(pointsItemView);
            }
            
            if (index <= 0) return;
            Destroy(gameObject);
        }
    }
}
