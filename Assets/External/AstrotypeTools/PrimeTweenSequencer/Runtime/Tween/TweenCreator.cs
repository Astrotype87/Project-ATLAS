using System;
using PrimeTween;
using UnityEngine;

namespace AstrotypeTools.PrimeTweenSequencer
{
    [Serializable]
    public class TweenCreator
    {
        [SerializeField] private GameObject gameObject;
        [SerializeReference] private TweenBase tweenBase;
        
        public bool HasTweenSelected => tweenBase != null;
        public Type TweenType => tweenBase?.GetType();
        public Type TargetType => tweenBase?.TargetType;
        public bool IsTargetSet => tweenBase != null && tweenBase.IsTargetSet;
        
        public float TotalDuration => tweenBase == null ? 0 : tweenBase.TotalDuration;
        public float SingleDuration => tweenBase == null ? 0 : tweenBase.SingleDuration;
        public float Duration => tweenBase == null ? 0 : tweenBase.Duration;
        public float StartDelay => tweenBase == null ? 0 : tweenBase.StartDelay;
        public float EndDelay => tweenBase == null ? 0 : tweenBase.EndDelay;
        public bool IsInfiniteCycle => tweenBase != null && tweenBase.IsInfiniteCycle;
        public CycleMode CycleMode => tweenBase == null ? CycleMode.Restart : tweenBase.CycleMode;
        
        public void SetGameObject(GameObject gameObject) => this.gameObject = gameObject;
        public void SetTweenBase(TweenBase tweenBase)
        {
            this.tweenBase = tweenBase;
            this.tweenBase.SetTarget(gameObject);
        }
        public Tween CreateTween(bool forceFiniteCycle) => tweenBase.CreateTween(forceFiniteCycle);
        public void OnTweenStart() => tweenBase?.OnTweenStart();
        
        public void SaveOriginalValue() => tweenBase?.SaveOriginalValue();
        public void RestoreOriginalValue() => tweenBase?.RestoreOriginalValue();
        public void ValidateSettings() => tweenBase?.ValidateSettings();
    }
    
}
