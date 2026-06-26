using UnityEngine;
using UnityEngine.UI;
using KBCore.Refs;

namespace ProjectATLAS.UI.Layout
{
    public class UIVerticalContentExpander : MonoBehaviour
    {
        [SerializeField, Self] private VerticalLayoutGroup verticalLayoutGroup;
        [SerializeField] private float minHeight = 500;
        
        private void OnValidate()
        {
            this.ValidateRefs();
        }
        
        public void UpdateViewportHeight()
        {
            float height = 0;
            float verticalSpacing = verticalLayoutGroup.spacing;
            height -= verticalSpacing;
            
            for (int i = 0; i < transform.childCount; i++)
            {
                RectTransform childRectTransform = transform.GetChild(i).gameObject.GetComponent<RectTransform>();
                height += childRectTransform.sizeDelta.y + verticalSpacing;
            }
            
            RectTransform rectTransform = transform as RectTransform;
            Vector2 sizeDelta = rectTransform.sizeDelta;
            sizeDelta.y = Mathf.Max(height, minHeight);
            rectTransform.sizeDelta = sizeDelta;
        }
    }
}
