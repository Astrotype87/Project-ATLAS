using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KBCore.Refs;

namespace ProjectATLAS.UI
{
    // TODO: Update certain GameObjects
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class UITextHeightFitter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text targetText;
        
        [Header("Settings")]
        [SerializeField] private float minHeight = 100f;
        [SerializeField] private float heightOffset = 0f;
        
        private RectTransform rectTransform;
        private float lastPreferredHeight;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            rectTransform = transform as RectTransform;
        }
        
        private void LateUpdate()
        {
            RefreshHeight();
        }
        
        
        // PRIVATE METHODS
        private void RefreshHeight()
        {
            if (targetText == null) return;
            
            float preferredHeight = targetText.preferredHeight;
            
            if (!Mathf.Approximately(preferredHeight, lastPreferredHeight))
            {
                lastPreferredHeight = preferredHeight;
                
                float newHeight = Mathf.Max(preferredHeight + heightOffset, minHeight);
                
                Vector2 sizeDelta = rectTransform.sizeDelta;
                sizeDelta.y = newHeight;
                rectTransform.sizeDelta = sizeDelta;
            }
        }
    }
}
