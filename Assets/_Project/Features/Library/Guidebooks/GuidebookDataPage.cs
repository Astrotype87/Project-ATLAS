using UnityEngine;
using TMPro;
using KBCore.Refs;

using ProjectATLAS.UI;

namespace ProjectATLAS.Library.Guidebooks
{
    public class GuidebookDataPage : UIPage
    {
        [Header("Guidebook")]
        [SerializeField] private int guidebookID;
        [SerializeField] private string guidebookName;
        [SerializeField] private int chapter;
        [SerializeField] private int level;
        
        [Header("Components")]
        [SerializeField, Child] private TMP_Text titleText;
        
        public int GuidebookID => guidebookID;
        public string GuidebookName => guidebookName;
        public int Chapter => chapter;
        public int Level => level;
        
        // PRIVATE FIELDS
        private RectTransform rectTransform;
        private RectTransform parentTransform;
        
        
        // MONOBEHAVIOUR METHODS
        protected override void OnValidate()
        {
            pageName = $"Guidebook {guidebookID}";
            if (titleText) titleText.text = $"GUIDEBOOK {guidebookID} : {guidebookName}";
            
            base.OnValidate();
        }
        
        // UIPage METHODS
        public override void OpenPage()
        {
            base.OpenPage();
            
            if (!rectTransform) rectTransform = transform as RectTransform;
            if (!parentTransform) parentTransform = transform.parent as RectTransform;
            
            if (rectTransform && parentTransform)
            {
                // Reset y position
                parentTransform.position = new(parentTransform.position.x, 0);
                
                // Auto adjust scroll view content
                Vector2 sizeDelta = parentTransform.sizeDelta;
                sizeDelta.y = rectTransform.sizeDelta.y;
                parentTransform.sizeDelta = sizeDelta;
            }
        }
    }
}
