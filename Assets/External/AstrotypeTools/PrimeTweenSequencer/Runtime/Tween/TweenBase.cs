using System;
using UnityEngine;
using PrimeTween;

namespace AstrotypeTools.PrimeTweenSequencer
{
    [Serializable]
    public abstract class TweenBase
    {
        public abstract bool IsTargetSet { get; }
        public abstract Type TargetType { get; }
        public abstract float TotalDuration { get; }
        public abstract float SingleDuration { get; }
        public abstract float Duration { get; }
        public abstract float StartDelay { get; }
        public abstract float EndDelay { get; }
        public abstract bool IsInfiniteCycle { get; }
        public virtual CycleMode CycleMode => CycleMode.Restart;
        
        public abstract void SetTarget(GameObject gameObject);
        public abstract Tween CreateTween(bool forceFiniteCycle = false);
        public abstract void OnTweenStart();
        
        public abstract void SaveOriginalValue();
        public abstract void RestoreOriginalValue();
        public abstract void ValidateSettings();
    }
    
    [Serializable]
    public abstract class TweenBase<TTarget> : TweenBase
        where TTarget : Component
    {
        [SerializeField] public TTarget target;
        
        public override bool IsTargetSet => target != null;
        public override Type TargetType => typeof(TTarget);
        
        public override void SetTarget(GameObject gameObject)
        {
            if (gameObject != null && gameObject.TryGetComponent<TTarget>(out var component))
            {
                target = component;
            }
            else
            {
                target = null;
            }
        }
    }
    
}
