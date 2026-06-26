using UnityEngine;
using KBCore.Refs;
using AstrotypeTools.InspectorAttributes;

namespace ProjectATLAS.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class UIScreenSafe : MonoBehaviour
    {
        [SerializeField, Self] private RectTransform rectTransform;
        [SerializeField] private bool autoDetect;
        [SerializeField, DisableIf(nameof(autoDetect))] private Padding padding;
        
        // PROPERTIES
        public bool AutoDetect { get => autoDetect; set => autoDetect = value; }
        
        
        // MONOBEHAVIOUR METHODS
        private void OnValidate()
        {
            this.ValidateRefs();
            UpdateSafeArea();
        }
        
        
        // PUBLIC METHODS
        public void SetPadding(Padding padding)
        {
            this.padding = padding;
        }
        
        public void UpdateSafeArea()
        {
            if (autoDetect)
            {
                // Debug.Log($"Screen.width: {Screen.width}  Screen.height: {Screen.height}");
                // Debug.Log($"Screen.currentResolution.width: {Screen.currentResolution.width}  Screen.currentResolution.height: {Screen.currentResolution.height}");
                // Debug.Log($"Screen.safeArea: {Screen.safeArea}");
                
                float screenWidth = Screen.width;
                float screenHeight = Screen.height;
                Rect safeArea = Screen.safeArea;
                
                padding.left = safeArea.xMin;
                padding.right = screenWidth - safeArea.xMax;
                padding.bottom = safeArea.yMin;
                padding.top = screenHeight - safeArea.yMax;
            }
            
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            
            rectTransform.offsetMin = padding.GetOffsetMin();
            rectTransform.offsetMax = padding.GetOffsetMax();
        }
    }
}
