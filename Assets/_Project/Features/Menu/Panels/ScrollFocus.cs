using UnityEngine;
using UnityEditor;
using System.Collections;
using PrimeTween;
using UnityEngine.EventSystems;

namespace ProjectATLAS.Menu
{
    public class ScrollFocus : MonoBehaviour, IBeginDragHandler
    {
        [Header("Transform")]
        [SerializeField] private RectTransform targetTransform;
        [SerializeField] private RectTransform frameTransform;
        [SerializeField] private RectTransform contentTransform;
        
        [Header("Scroll Area")]
        [SerializeField] private float extraWidth;
        
        [Header("Animation")]
        [SerializeField] private float focusTime;
        [SerializeField] private Ease focusEase;
        
        public bool IsAnimating { get; private set; }
        
        private Vector2 originalPosition;
        private float originalWidth;
        
        private Coroutine animationCoroutine;
        private Tween tweenAnimation;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            this.targetTransform = null;
            
            originalPosition = contentTransform.anchoredPosition;
            originalWidth = contentTransform.sizeDelta.x;
        }
        
        
        // PUBLIC METHODS
        public void FocusContentOnTarget(RectTransform targetTransform, bool isAnimated = true)
        {
            if (this.targetTransform == targetTransform) return;
            this.targetTransform = targetTransform;
            
            InterruptAnimation();
            
            Vector2 contentPosition = GetContentPosition(targetTransform);
            if (isAnimated) animationCoroutine = StartCoroutine(AnimateContentMovement(contentTransform, contentPosition));
            else contentTransform.anchoredPosition = contentPosition;
        }
        
        public void UnfocusReference()
        {
            this.targetTransform = null;
        }
        
        public void InterruptAnimation()
        {
            if (animationCoroutine != null) StopCoroutine(animationCoroutine);
            if (tweenAnimation.isAlive) tweenAnimation.Stop();
            
            IsAnimating = false;
        }
        
        public void SetExpandedScrollArea(bool enabled)
        {
            Vector2 sizeDelta = contentTransform.sizeDelta;
            sizeDelta.x = originalWidth + (enabled ? extraWidth : 0);
            contentTransform.sizeDelta = sizeDelta;
        }
        
        
        
        // EVENT LISTENER METHODS
        public void OnBeginDrag(PointerEventData eventData)
        {
            InterruptAnimation();
        }
        
        // PRIVATE METHODS
        private IEnumerator AnimateContentMovement(RectTransform contentTransform, Vector2 contentPosition)
        {
            IsAnimating = true;
            tweenAnimation = Tween.UIAnchoredPosition(contentTransform, originalPosition + contentPosition, focusTime, focusEase);
            yield return tweenAnimation.ToYieldInstruction();
            IsAnimating = false;
        }
        
        private Vector2 GetContentPosition(RectTransform targetTransform)
        {
            Vector2 targetPosition = targetTransform.anchoredPosition;
            Vector2 frameTopLeftPosition = GetTopLeftLocal(frameTransform);
            Vector2 frameSize = GetActualSize(frameTransform);
            Vector2 frameOffset = new Vector2(frameSize.x, -frameSize.y) / 2;
            
            Vector2 contentPosition = -targetPosition + frameTopLeftPosition + frameOffset;
            
            return contentPosition;
        }
        
        private static Vector2 GetTopLeftLocal(RectTransform rt)
        {
            Vector2 pivotOffset = new(
                -rt.pivot.x * rt.sizeDelta.x, 
                (1 - rt.pivot.y) * rt.sizeDelta.y
            );
            
            return rt.anchoredPosition + pivotOffset;
        }
        
        public static Vector2 GetActualSize(RectTransform rt)
        {
            if (rt.parent == null) return rt.sizeDelta; // fallback
            
            RectTransform parent = rt.parent as RectTransform;
            
            // For stretched rects, actual size = parent size * anchor diff + sizeDelta
            Vector2 parentSize = parent.rect.size;
            Vector2 anchorDiff = rt.anchorMax - rt.anchorMin;
            
            Vector2 size = new Vector2(
                parentSize.x * anchorDiff.x + rt.sizeDelta.x,
                parentSize.y * anchorDiff.y + rt.sizeDelta.y
            );
            
            return size;
        }
        
    }
}
