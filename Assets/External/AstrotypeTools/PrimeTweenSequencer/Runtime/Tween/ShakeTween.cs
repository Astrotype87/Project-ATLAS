using System;
using PrimeTween;
using UnityEngine;

namespace AstrotypeTools.PrimeTweenSequencer
{
    [Serializable]
    public abstract class ShakeTween<TTarget, TValue> : TweenBase<TTarget>
        where TTarget : Component
        where TValue : struct
    {
        public Vector3 strength;
        public float frequency = 10;
        public float duration = 0.5f;
            public bool enableFalloff = false;
                public Ease falloffEase = Ease.Default;
                public AnimationCurve strengthOverTime = new(new Keyframe[] { new(0f, 0f, 1f, 1f), new(1f, 1f, 1f, 1f) });
            [Range(0f, 1f)] public float asymmetry = 0;
            public Ease easeBetweenShakes = Ease.Default;
        public int cycles = 1;
            public float startDelay = 0;
            public float endDelay = 0;
            public bool useUnscaledTime = false;
            public UpdateType updateType = UpdateType.Default;
        
        protected TValue _originalValue;     // Original setting saved before tween is created
        protected TValue _initialValue;      // Current value before tween starts playing
        
        
        public override float TotalDuration => cycles == -1 ? float.PositiveInfinity : (duration + startDelay + endDelay) * cycles;
        public override float SingleDuration => duration + startDelay + endDelay;
        public override float Duration => duration;
        public override float StartDelay => startDelay;
        public override float EndDelay => endDelay;
        public override bool IsInfiniteCycle => cycles == -1;
        
        
        public override Tween CreateTween(bool forceFiniteCycle)
        {
            if (target == null)
                throw new NullReferenceException("Tween creation failed due to target component not being set.");
            
            ShakeSettings shakeSettings = GetShakeSettings(forceFiniteCycle);
            
            return CreateTween(shakeSettings);
        }
        
        public override void OnTweenStart()
        {
            _initialValue = CurrentValue;
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
        
        
        private ShakeSettings GetShakeSettings(bool forceFiniteCycle)
        {
            int cycles = forceFiniteCycle && this.cycles == -1 ? 1 : this.cycles;
            
            ShakeSettings shakeSettings = new(strength, duration, frequency, enableFalloff,
                easeBetweenShakes, asymmetry, cycles, startDelay, endDelay, useUnscaledTime, updateType);
            
            shakeSettings.falloffEase = falloffEase;
            shakeSettings.strengthOverTime = strengthOverTime;
            
            return shakeSettings;
        }
        
        
        protected abstract TValue CurrentValue { get; set; }
        
        protected abstract Tween CreateTween(ShakeSettings shakeSettings);
        
    }
}
