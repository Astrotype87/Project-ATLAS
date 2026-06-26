using System;
using UnityEngine;
using PrimeTween;

namespace AstrotypeTools.PrimeTweenSequencer
{
    [Serializable]
    public abstract class CustomTween<TTarget, TValue> : StandardTween<TTarget, TValue>
        where TTarget : Component
        where TValue : struct
    {
        protected TValue _appliedStartValue;
        protected TValue _appliedEndValue;
        
        private static readonly Action<CustomTween<TTarget, TValue>, float> OnValueChangeCallback =
            (customTween, value) => customTween.OnValueChange(value);
        
        public override Tween CreateTween(bool forceFiniteCycle = false)
        {
            if (target == null || (endMode == EndMode.Follow && followTarget == null))
                throw new NullReferenceException("Tween creation failed due to target component or follow component not being set.");
            
            TValue appliedStartValue = GetAppliedStartValue();
            TValue appliedEndValue = GetAppliedEndValue();
            TweenSettings tweenSettings = GetTweenSettings(forceFiniteCycle);
            
            TweenSettings<TValue> fullSettings = new(appliedStartValue, appliedEndValue, tweenSettings);
            
            return CreateTween(fullSettings);
        }
        
        protected override Tween CreateTween(TweenSettings<TValue> tweenSettings)
        {
            TweenSettings<float> fullSettings = new(0f, 1f, tweenSettings.settings);
            return Tween.Custom(this, fullSettings, OnValueChangeCallback);
        }
        
        public override void OnTweenStart()
        {
            base.OnTweenStart();
            
            // _initialValue = CurrentValue;
            _appliedStartValue = GetAppliedStartValue();
            _appliedEndValue = GetAppliedEndValue();
        }
        
        /// <summary> Use the value parameter as time for your lerp functions. </summary>
        protected abstract void OnValueChange(float value);
    }
}
