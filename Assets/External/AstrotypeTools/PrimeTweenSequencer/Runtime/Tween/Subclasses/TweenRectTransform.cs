using UnityEngine;
using PrimeTween;

namespace AstrotypeTools.PrimeTweenSequencer
{
    public class UIAnchorMin : StandardTween<RectTransform, Vector2>
    {
        protected override Vector2 CurrentValue
        {
            get => target.anchorMin;
            set => target.anchorMin = value;
        }
        protected override Tween CreateTween(TweenSettings<Vector2> tweenSettings)
            => Tween.UIAnchorMin(target, tweenSettings);
        
        protected override Vector2 GetFollowTargetOffset(RectTransform followTarget, Vector2 offset)
            => followTarget.anchorMin + offset;
        
        protected override Vector2 GetSumOfValues(Vector2 a, Vector2 b) => a + b;
    }
    
    public class UIAnchorMax : StandardTween<RectTransform, Vector2>
    {
        protected override Vector2 CurrentValue
        {
            get => target.anchorMax;
            set => target.anchorMax = value;
        }
        protected override Tween CreateTween(TweenSettings<Vector2> tweenSettings)
            => Tween.UIAnchorMax(target, tweenSettings);
        
        protected override Vector2 GetFollowTargetOffset(RectTransform followTarget, Vector2 offset)
            => followTarget.anchorMax + offset;
        
        protected override Vector2 GetSumOfValues(Vector2 a, Vector2 b) => a + b;
    }
    
    public class UIAnchoredPosition : StandardTween<RectTransform, Vector2>
    {
        protected override Vector2 CurrentValue
        {
            get => target.anchoredPosition;
            set => target.anchoredPosition = value;
        }
        protected override Tween CreateTween(TweenSettings<Vector2> tweenSettings)
            => Tween.UIAnchoredPosition(target, tweenSettings);
        
        protected override Vector2 GetFollowTargetOffset(RectTransform followTarget, Vector2 offset)
            => followTarget.anchoredPosition + offset;
        
        protected override Vector2 GetSumOfValues(Vector2 a, Vector2 b) => a + b;
    }
    
    public class UISizeDelta : StandardTween<RectTransform, Vector2>
    {
        protected override Vector2 CurrentValue
        {
            get => target.sizeDelta;
            set => target.sizeDelta = value;
        }
        protected override Tween CreateTween(TweenSettings<Vector2> tweenSettings)
            => Tween.UISizeDelta(target, tweenSettings);
        
        protected override Vector2 GetFollowTargetOffset(RectTransform followTarget, Vector2 offset)
            => followTarget.sizeDelta + offset;
        
        protected override Vector2 GetSumOfValues(Vector2 a, Vector2 b) => a + b;
    }
    
    public class UIPivot : StandardTween<RectTransform, Vector2>
    {
        protected override Vector2 CurrentValue
        {
            get => target.pivot;
            set => target.pivot = value;
        }
        protected override Tween CreateTween(TweenSettings<Vector2> tweenSettings)
            => Tween.UIPivot(target, tweenSettings);
        
        protected override Vector2 GetFollowTargetOffset(RectTransform followTarget, Vector2 offset)
            => followTarget.pivot + offset;
        
        protected override Vector2 GetSumOfValues(Vector2 a, Vector2 b) => a + b;
    }
    
    
    public class UIRotation : CustomTween<RectTransform, float>
    {
        private Quaternion _startRotation;
        private Quaternion _endRotation;
        
        protected override float CurrentValue
        {
            get => target.localRotation.eulerAngles.z;
            set => target.localRotation =
                Quaternion.Euler(target.localRotation.eulerAngles.x, target.localRotation.eulerAngles.y, value);
        }
        public override void OnTweenStart()
        {
            base.OnTweenStart();
            
            Vector3 currentEuler = target.localRotation.eulerAngles;
            _startRotation = Quaternion.Euler(currentEuler.x, currentEuler.y, _appliedStartValue);
            _endRotation = Quaternion.Euler(currentEuler.x, currentEuler.y, _appliedEndValue);
        }
        protected override void OnValueChange(float value)
        {
            target.localRotation = Quaternion.LerpUnclamped(_startRotation, _endRotation, value);
        }
        protected override float GetFollowTargetOffset(RectTransform followTarget, float offset)
            => followTarget.localRotation.eulerAngles.z + offset;
        
        protected override float GetSumOfValues(float a, float b) => a + b;
    }
    
    public class UIScale : CustomTween<RectTransform, Vector2>
    {
        protected override Vector2 CurrentValue
        {
            get => target.localScale;
            set => target.localScale = new(value.x, value.y, target.localScale.z);
        }
        protected override void OnValueChange(float value)
        {
            Vector2 newValue = Vector2.LerpUnclamped(_appliedStartValue, _appliedEndValue, value);
            target.localScale = new(newValue.x, newValue.y, target.localScale.z);
        }
        protected override Vector2 GetFollowTargetOffset(RectTransform followTarget, Vector2 offset)
            => followTarget.localScale + new Vector3(offset.x, offset.y, target.localScale.z);
        
        protected override Vector2 GetSumOfValues(Vector2 a, Vector2 b) => a + b;
    }
    
    
    public class UIAnchorLeft : CustomTween<RectTransform, float>
    {
        protected override float CurrentValue
        {
            get => target.anchorMin.x;
            set => target.anchorMin = new(value, target.anchorMin.y);
        }
        protected override void OnValueChange(float value)
        {
            float newValue = Mathf.LerpUnclamped(_appliedStartValue, _appliedEndValue, value);
            target.anchorMin = new(newValue, target.anchorMin.y);
        }
        protected override float GetFollowTargetOffset(RectTransform followTarget, float offset)
            => followTarget.anchorMin.x + offset;
        
        protected override float GetSumOfValues(float a, float b) => a + b;
    }
    
    public class UIAnchorRight : CustomTween<RectTransform, float>
    {
        protected override float CurrentValue
        {
            get => target.anchorMax.x;
            set => target.anchorMax = new(value, target.anchorMax.y);
        }
        protected override void OnValueChange(float value)
        {
            float newValue = Mathf.LerpUnclamped(_appliedStartValue, _appliedEndValue, value);
            target.anchorMax = new(newValue, target.anchorMax.y);
        }
        protected override float GetFollowTargetOffset(RectTransform followTarget, float offset)
            => followTarget.anchorMax.x + offset;
        
        protected override float GetSumOfValues(float a, float b) => a + b;
    }
    
    public class UIAnchorBottom : CustomTween<RectTransform, float>
    {
        protected override float CurrentValue
        {
            get => target.anchorMin.y;
            set => target.anchorMin = new(target.anchorMin.x, value);
        }
        protected override void OnValueChange(float value)
        {
            float newValue = Mathf.LerpUnclamped(_appliedStartValue, _appliedEndValue, value);
            target.anchorMin = new(target.anchorMin.x, newValue);
        }
        protected override float GetFollowTargetOffset(RectTransform followTarget, float offset)
            => followTarget.anchorMin.y + offset;
        
        protected override float GetSumOfValues(float a, float b) => a + b;
    }
    
    public class UIAnchorTop : CustomTween<RectTransform, float>
    {
        protected override float CurrentValue
        {
            get => target.anchorMax.y;
            set => target.anchorMax = new(target.anchorMax.x, value);
        }
        protected override void OnValueChange(float value)
        {
            float newValue = Mathf.LerpUnclamped(_appliedStartValue, _appliedEndValue, value);
            target.anchorMax = new(target.anchorMax.x, newValue);
        }
        protected override float GetFollowTargetOffset(RectTransform followTarget, float offset)
            => followTarget.anchorMax.y + offset;
        
        protected override float GetSumOfValues(float a, float b) => a + b;
    }
    
    
    public class UISizeDeltaWidth : CustomTween<RectTransform, float>
    {
        protected override float CurrentValue
        {
            get => target.sizeDelta.x;
            set => target.sizeDelta = new(value, target.sizeDelta.y);
        }
        protected override void OnValueChange(float value)
        {
            float newValue = Mathf.LerpUnclamped(_appliedStartValue, _appliedEndValue, value);
            target.sizeDelta = new(newValue, target.sizeDelta.y);
        }
        protected override float GetFollowTargetOffset(RectTransform followTarget, float offset)
            => followTarget.sizeDelta.x + offset;
        
        protected override float GetSumOfValues(float a, float b) => a + b;
    }
    
    public class UISizeDeltaHeight : CustomTween<RectTransform, float>
    {
        protected override float CurrentValue
        {
            get => target.sizeDelta.y;
            set => target.sizeDelta = new(target.sizeDelta.x, value);
        }
        protected override void OnValueChange(float value)
        {
            float newValue = Mathf.LerpUnclamped(_appliedStartValue, _appliedEndValue, value);
            target.sizeDelta = new(target.sizeDelta.x, newValue);
        }
        protected override float GetFollowTargetOffset(RectTransform followTarget, float offset)
            => followTarget.sizeDelta.y + offset;
        
        protected override float GetSumOfValues(float a, float b) => a + b;
    }
    
}
