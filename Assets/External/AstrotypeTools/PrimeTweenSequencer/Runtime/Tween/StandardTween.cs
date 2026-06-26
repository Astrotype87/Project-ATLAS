using System;
using UnityEngine;
using PrimeTween;

namespace AstrotypeTools.PrimeTweenSequencer
{
    [Serializable]
    public abstract class StandardTween<TTarget, TValue> : TweenBase<TTarget>
        where TTarget : Component
        where TValue : struct
    {
        public StartMode startMode;
        public TValue startValue;
        public EndMode endMode;
        public TValue endValue;
        public TTarget followTarget;
        
        public float duration = 1;
            [EaseMenu] public Ease ease = Ease.Default;
                public CustomEase customEase = CustomEase.Curve;
                public AnimationCurve curve = new(new Keyframe[] { new(0f, 0f, 1f, 1f), new(1f, 1f, 1f, 1f) });
                public float strength = 1;
                public float period = 0.3f;
            public Direction direction = Direction.Forward;
        
        public int cycles = 1;
            public CycleMode cycleMode = CycleMode.Restart;
            public float startDelay = 0;
            public float endDelay = 0;
            public bool useUnscaledTime = false;
            public UpdateType updateType = UpdateType.Default;
        
        protected TValue _originalValue;    // Original setting saved before tween is created
        // protected TValue _initialValue;     // Absolute starting value of a tween
        // protected TValue _finalValue;       // Absolute ending value of a tween
        
        #if UNITY_EDITOR
        public bool e_durationFoldout;
        public bool e_cyclesFoldout;
        #endif
        
        
        public override float TotalDuration => cycles == -1 ? float.PositiveInfinity : (duration + startDelay + endDelay) * cycles;
        public override float SingleDuration => duration + startDelay + endDelay;
        public override float Duration => duration;
        public override float StartDelay => startDelay;
        public override float EndDelay => endDelay;
        public override bool IsInfiniteCycle => cycles == -1;
        public override CycleMode CycleMode => cycleMode;
        
        
        public override Tween CreateTween(bool forceFiniteCycle = false)
        {
            if (target == null || (endMode == EndMode.Follow && followTarget == null))
                throw new NullReferenceException($"Tween creation of \"{GetType().Name}\" failed.\nThe target component or follow component is not set.");
            
            TValue appliedStartValue = GetAppliedStartValue();
            TValue appliedEndValue = GetAppliedEndValue();
            TweenSettings tweenSettings = GetTweenSettings(forceFiniteCycle);
            
            TweenSettings<TValue> fullSettings = startMode == StartMode.Current
                ? new TweenSettings<TValue>(appliedEndValue, tweenSettings)
                : new TweenSettings<TValue>(appliedStartValue, appliedEndValue, tweenSettings);
            
            return CreateTween(fullSettings);
        }
        
        // BUG: This function is called late. CreateTween is called first before OnTweenStart
        // OnTweenStart is supposed to be called first before CreateTween()
        // so that appliedStartValue = GetAppliedStartValue(); and appliedEndValue = GetAppliedEndValue();
        // inside CreateTween() retrieves the updated _initialValue
        public override void OnTweenStart()
        {
            // _initialValue = CurrentValue;
            
            if (duration <= 0)
                CurrentValue = GetAppliedEndValue();
        }
        
        
        public override void SaveOriginalValue()
        {
            if (target != null)
                _originalValue = CurrentValue;
            
        }
        
        public override void RestoreOriginalValue()
        {
            if (target != null)
                CurrentValue = _originalValue;
        }
        
        public override void ValidateSettings()
        {
            duration = Mathf.Max(0f, duration);
            cycles = Math.Max(-1, cycles);
            if (cycles == 0) cycles = 1;
            
            startDelay = Mathf.Max(0f, startDelay);
            endDelay = Mathf.Max(0f, endDelay);
        }
        
        
        protected TValue GetAppliedStartValue()
        {
            return startMode switch {
                StartMode.Current => CurrentValue,
                StartMode.Absolute => startValue,
                StartMode.Relative => GetSumOfValues(_originalValue, startValue),
                // StartMode.Additive => GetSumOfValues(_initialValue, startValue),
                _ => CurrentValue
            };
        }
        
        protected TValue GetAppliedEndValue()
        {
            return endMode switch {
                EndMode.Absolute => endValue,
                EndMode.Relative => GetSumOfValues(_originalValue, endValue),
                // EndMode.Additive => GetSumOfValues(GetAppliedStartValue(), endValue),
                EndMode.Follow => GetFollowTargetOffset(followTarget, endValue),
                _ => endValue
            };
        }
        
        protected TweenSettings GetTweenSettings(bool forceFiniteCycle)
        {
            int cycles = forceFiniteCycle && this.cycles == -1 ? 1 : this.cycles;
            
            if (ease != Ease.Custom) {
                return new(duration, ease, cycles, cycleMode,
                    startDelay, endDelay, useUnscaledTime, updateType);
            } else {
                Easing easing = customEase switch {
                    CustomEase.Curve => Easing.Curve(curve),
                    CustomEase.Overshoot => Easing.Overshoot(strength),
                    CustomEase.Bounce => Easing.Bounce(strength),
                    CustomEase.Elastic => Easing.Elastic(strength, period),
                    CustomEase.BounceExact => Easing.BounceExact(strength),
                    _ => Easing.Curve(curve)
                };
                return new(duration, easing, cycles, cycleMode,
                    startDelay, endDelay, useUnscaledTime, updateType);
            }
        }
        
        
        protected abstract TValue CurrentValue { get; set; }
        
        protected abstract Tween CreateTween(TweenSettings<TValue> tweenSettings);
        protected abstract TValue GetSumOfValues(TValue a, TValue b);
        protected abstract TValue GetFollowTargetOffset(TTarget followTarget, TValue offset);
        
        
    }
    
}
