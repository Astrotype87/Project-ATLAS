using UnityEngine;

namespace ProjectATLAS.Utility
{
    public static class RectUtil
    {
        public static void SetAnchorLeft(this RectTransform rectTransform, float value)
            => rectTransform.anchorMin = new(value, rectTransform.anchorMin.y);
            
        public static void SetAnchorRight(this RectTransform rectTransform, float value)
            => rectTransform.anchorMax = new(value, rectTransform.anchorMax.y);
        
        public static void SetAnchorBottom(this RectTransform rectTransform, float value)
            => rectTransform.anchorMin = new(rectTransform.anchorMin.x, value);
        
        public static void SetAnchorTop(this RectTransform rectTransform, float value)
            => rectTransform.anchorMax = new(rectTransform.anchorMax.x, value);
        
        
        public static void SetAnchorOffsetLeft(this RectTransform rectTransform, float value)
            => rectTransform.offsetMin = new(value, rectTransform.offsetMin.y);
            
        public static void SetAnchorOffsetRight(this RectTransform rectTransform, float value)
            => rectTransform.offsetMax = new(-value, rectTransform.offsetMax.y);
        
        public static void SetAnchorOffsetBottom(this RectTransform rectTransform, float value)
            => rectTransform.offsetMin = new(rectTransform.offsetMin.x, value);
        
        public static void SetAnchorOffsetTop(this RectTransform rectTransform, float value)
            => rectTransform.offsetMax = new(rectTransform.offsetMax.x, -value);
    }
}
