using System;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.Events;

namespace AstrotypeTools.PrimeTweenSequencer
{
    [Serializable]
    public abstract class StepBase
    {
        [SerializeField] private bool enabled = true;
        [SerializeField] private StepMode stepMode = StepMode.Chain;
        [SerializeField] private float insertTime = 0;  // Show if stepMode == StepMode.Insert
        
        public bool Enabled => enabled;
        public StepMode StepMode => stepMode;
        public float InsertTime => insertTime;
        
        public abstract void SaveOriginalValue();
        public abstract void RestoreOriginalValue();
        
        #if UNITY_EDITOR
        public bool e_expanded = false;
        #endif
    }
    
    [Serializable]
    public class TweenStep : StepBase
    {
        [SerializeField] private TweenCreator tween = new();
        
        public static readonly Action<TweenStep> OnTweenStart = tweenStep => tweenStep.tween.OnTweenStart();
        
        public bool HasTweenSelected => tween.HasTweenSelected;
        public Type TweenType => tween.TweenType;
        public Type TargetType => tween.TargetType;
        public bool IsTargetSet => tween.IsTargetSet;
        
        public float StartDelay => tween.StartDelay;
        public float EndDelay => tween.EndDelay;
        public bool IsZeroDuration => tween.Duration <= 0;
        
        public void SetGameObject(GameObject gameObject) => tween?.SetGameObject(gameObject);
        
        public Tween CreateTween(bool forceFiniteCycle)
        {
            return tween.CreateTween(forceFiniteCycle);
        }
        
        public override void SaveOriginalValue() => tween?.SaveOriginalValue();
        public override void RestoreOriginalValue() => tween?.RestoreOriginalValue();
    }
    
    [Serializable]
    public class SequenceStep : StepBase
    {
        [SerializeField] private SequenceCreator sequence = new();
        
        public Sequence CreateSequence(bool avoidInfiniteCycle)
        {
            return sequence.CreateSequence(avoidInfiniteCycle);
        }
        
        public override void SaveOriginalValue() => sequence?.SaveOriginalValue();
        public override void RestoreOriginalValue() => sequence?.RestoreOriginalValue();
        
        public void ValidatePlayerReference(TweenSequencer root)
        {
            if (!Enabled) return;
            sequence.ValidatePlayerReference(root);
        }
        public void ValidatePlayerReference(HashSet<TweenSequencer> visited)
        {
            if (!Enabled) return;
            sequence.ValidatePlayerReference(visited);
        }
    }
    
    [Serializable]
    public class PlaySequencerStep : StepBase
    {
        [SerializeField] private TweenSequencer tweenSequencer;
        
        public TweenSequencer Sequencer => tweenSequencer;
        
        private const string InvalidReferenceMessage = "Circular or self-reference detected! Player reference has been cleared before creating the sequence.";
        
        public void ValidatePlayerReference(TweenSequencer root)
        {
            if (!Enabled) return;
            
            var visited = new HashSet<TweenSequencer>() { root };
            ValidatePlayerReference(visited);
        }
        
        public void ValidatePlayerReference(HashSet<TweenSequencer> visited)
        {
            if (!Enabled || tweenSequencer == null) return;
            
            if (visited.Contains(tweenSequencer))
            {
                Debug.LogWarning(InvalidReferenceMessage);
                tweenSequencer = null;
                return;
            }
            
            visited.Add(tweenSequencer);
            tweenSequencer.ValidatePlayerReference(visited);
            visited.Remove(tweenSequencer);
        }
        
        
        public override void SaveOriginalValue()
        {
            if (tweenSequencer) tweenSequencer.SaveState();
        }
        public override void RestoreOriginalValue()
        {
            if (tweenSequencer) tweenSequencer.RestoreState();
        }
    }
    
    [Serializable]
    public class DelayStep : StepBase
    {
        [SerializeField] private float delay;
        [SerializeField] private UnityEvent callback;
        
        public float Delay => delay;
        
        private static readonly Action<DelayStep> OnInvoke = delayStep => delayStep.callback.Invoke();
        
        public Tween CreateDelay()
        {
            return Tween.Delay(this, delay, OnInvoke);
        }
        
        public override void SaveOriginalValue() { }
        public override void RestoreOriginalValue() { }
    }
    
    [Serializable]
    public class CallbackStep : StepBase
    {
        [SerializeField] private UnityEvent callback;
        
        public static readonly Action<CallbackStep> OnInvoke = callbackStep => callbackStep.callback.Invoke();
        
        public override void SaveOriginalValue() { }
        public override void RestoreOriginalValue() { }
    }
    
    
}
