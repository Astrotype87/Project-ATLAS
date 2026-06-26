using System;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;

namespace AstrotypeTools.PrimeTweenSequencer
{
    [Serializable]
    public class SequenceCreator
    {
        [SerializeReference] private List<StepBase> steps = new();
        [SerializeField] private int cycles = 1;
        [SerializeField] private CycleMode cycleMode = CycleMode.Restart;
        [SerializeField] private Ease sequenceEase = Ease.Default;
        [SerializeField] private bool useUnscaledTime = false;
        [SerializeField] private UpdateType updateType = UpdateType.Default;
        
        private readonly Queue<StepBase> _chainGroupQueue = new();
        private readonly Queue<StepBase> _insertQueue = new();
        private readonly Queue<TweenStep> _startDelayQueue = new();
        private readonly Queue<TweenStep> _endDelayQueue = new();
        
        #if UNITY_EDITOR
        public bool e_expanded = false;
        public bool e_cyclesExpanded = false;
        #endif
        
        public float TotalDuration => CalculateDuration() * cycles;
        public float Duration => CalculateDuration();
        public bool IsInfiniteCycle => cycles == -1;
        public CycleMode CycleMode => cycleMode;
        // TODO: return true if a step is added, and if the step is TweenStep, check if it TweenStep.IsTweenBaseSet
        public bool HaveStepsPlayable => steps.Count < 0;
        
        
        public void AddStep(StepBase step)
        {
            if (steps == null) steps = new();
            steps.Add(step);
        }
        
        public Sequence CreateSequence(bool avoidInfiniteCycle)
        {
            int cycles = avoidInfiniteCycle && this.cycles == -1 ? 1 : this.cycles;
            
            // Create new sequence
            Sequence sequence = Sequence.Create(cycles, cycleMode, sequenceEase,
                useUnscaledTime, updateType);
            
            // Add each steps to sequence
            for (int i = 0; i < steps.Count; i++)
            {
                StepBase step = steps[i];
                if (step == null || !step.Enabled) continue;
                
                if (step is TweenStep tweenStep) AddTweenStep(tweenStep, sequence);
                else if (step is SequenceStep sequenceStep) AddSequenceStep(sequenceStep, sequence);
                else if (step is PlaySequencerStep playSequencerStep) AddPlaySequencerStep(playSequencerStep, sequence);
                else if (step is DelayStep delayStep) AddDelayStep(delayStep, sequence);
                else if (step is CallbackStep callbackStep) AddCallbackStep(callbackStep, sequence);
            }
            DequeueChainGroupSteps(sequence);
            DequeueInsertSteps(sequence);
            
            return sequence;
        }
        
        
        private void DequeueChainGroupSteps(Sequence sequence)
        {
            while (_chainGroupQueue.Count > 0)
            {
                StepBase step = _chainGroupQueue.Dequeue();
                if (step.StepMode == StepMode.Insert) continue;
                
                StepMode mode = step.StepMode;
                if (step is TweenStep tweenStep)
                {
                    Tween tween = tweenStep.CreateTween(true);
                    if (mode == StepMode.Chain) sequence.Chain(tween);
                    else if (mode == StepMode.Group) sequence.Group(tween);
                }
                else if (step is SequenceStep sequenceStep)
                {
                    Sequence newSequence = sequenceStep.CreateSequence(true);
                    if (mode == StepMode.Chain) sequence.Chain(newSequence);
                    else if (mode == StepMode.Group) sequence.Group(newSequence);
                }
                else if (step is PlaySequencerStep playSequencerStep)
                {
                    Sequence newSequence = playSequencerStep.Sequencer.PlayAwaitable(true);
                    if (mode == StepMode.Chain) sequence.Chain(newSequence);
                    else if (mode == StepMode.Group) sequence.Group(newSequence);
                }
            }
            
            while (_startDelayQueue.Count > 0)
            {
                TweenStep tweenStep = _startDelayQueue.Dequeue();
                float startDelay = tweenStep.StartDelay;
                Tween delayTween = Tween.Delay(tweenStep, startDelay, TweenStep.OnTweenStart);
                
                sequence.Group(delayTween);
            }
            
            while (_endDelayQueue.Count > 0)
            {
                TweenStep tweenStep = _endDelayQueue.Dequeue();
                float totalDelay = tweenStep.StartDelay + tweenStep.EndDelay;
                Tween delayTween = Tween.Delay(totalDelay);
                
                sequence.Group(delayTween);
            }
        }
        
        private void DequeueInsertSteps(Sequence sequence)
        {
            while (_insertQueue.Count > 0)
            {
                StepBase step = _insertQueue.Dequeue();
                if (step.StepMode != StepMode.Insert) continue;
                
                if (step is TweenStep tweenStep)
                {
                    Tween tween = tweenStep.CreateTween(true);
                    sequence.Insert(tweenStep.InsertTime, tween);
                }
                else if (step is SequenceStep sequenceStep)
                {
                    Sequence newSequence = sequenceStep.CreateSequence(true);
                    sequence.Insert(sequenceStep.InsertTime, newSequence);
                }
                else if (step is PlaySequencerStep playSequencerStep)
                {
                    Sequence newSequence = playSequencerStep.Sequencer.PlayAwaitable(true);
                    sequence.Insert(playSequencerStep.InsertTime, newSequence);
                }
            }
        }
        
        
        private void AddTweenStep(TweenStep tweenStep, Sequence sequence)
        {
            if (!tweenStep.HasTweenSelected || !tweenStep.IsTargetSet) return;
            
            StepMode mode = tweenStep.StepMode;
            float startDelay = tweenStep.StartDelay;
            float endDelay = tweenStep.EndDelay;
            bool hasDuration = !tweenStep.IsZeroDuration;
            
            if (mode == StepMode.Chain)
                DequeueChainGroupSteps(sequence);
            
            if (mode is StepMode.Chain or StepMode.Group)
            {
                if (startDelay > 0)
                    _startDelayQueue.Enqueue(tweenStep);
                else
                    sequence.ChainCallback(tweenStep, TweenStep.OnTweenStart);
                
                if (hasDuration)
                    _chainGroupQueue.Enqueue(tweenStep);
                else if (endDelay > 0)
                    _endDelayQueue.Enqueue(tweenStep);
            }
            else if (mode == StepMode.Insert)
            {
                sequence.InsertCallback(tweenStep.InsertTime + startDelay, tweenStep, TweenStep.OnTweenStart);
                
                if (hasDuration)
                    _insertQueue.Enqueue(tweenStep);
                else if (endDelay > 0)
                    _endDelayQueue.Enqueue(tweenStep);
            }
        }
        
        private void AddSequenceStep(SequenceStep sequenceStep, Sequence sequence)
        {
            StepMode mode = sequenceStep.StepMode;
            
            if (mode == StepMode.Chain)
                DequeueChainGroupSteps(sequence);
            
            if (mode is StepMode.Chain or StepMode.Group)
                _chainGroupQueue.Enqueue(sequenceStep);
            else if (mode == StepMode.Insert)
                _insertQueue.Enqueue(sequenceStep);
        }
        
        private void AddPlaySequencerStep(PlaySequencerStep playSequencerStep, Sequence sequence)
        {
            StepMode mode = playSequencerStep.StepMode;
            
            if (mode == StepMode.Chain)
                DequeueChainGroupSteps(sequence);
            
            if (mode is StepMode.Chain or StepMode.Group)
                _chainGroupQueue.Enqueue(playSequencerStep);
            else if (mode == StepMode.Insert)
                _insertQueue.Enqueue(playSequencerStep);
        }
        
        private void AddDelayStep(DelayStep delayStep, Sequence sequence)
        {
            StepMode mode = delayStep.StepMode;
            Tween delay = delayStep.CreateDelay();
            
            if (mode == StepMode.Chain)
            {
                DequeueChainGroupSteps(sequence);
                sequence.Chain(delay);
            }
            else if (mode == StepMode.Group)
            {
                sequence.Group(delay);
            }
            else if (mode == StepMode.Insert)
            {
                sequence.Insert(delayStep.InsertTime, delay);
            }
        }
        
        private void AddCallbackStep(CallbackStep callbackStep, Sequence sequence)
        {
            StepMode mode = callbackStep.StepMode;
            if (mode == StepMode.Chain)
            {
                DequeueChainGroupSteps(sequence);
                sequence.ChainCallback(callbackStep, CallbackStep.OnInvoke);
            }
            else if (mode == StepMode.Group)
            {
                sequence.ChainCallback(callbackStep, CallbackStep.OnInvoke);
            }
            else if (mode == StepMode.Insert)
            {
                sequence.InsertCallback(callbackStep.InsertTime, callbackStep, CallbackStep.OnInvoke);
            }
        }
        
        
        public void SaveOriginalValue()
        {
            for (int i = 0; i < steps.Count; i++)
            {
                steps[i]?.SaveOriginalValue();
            }
        }
        
        public void RestoreOriginalValue()
        {
            for (int i = steps.Count - 1; i >= 0; i--)
            {
                steps[i]?.RestoreOriginalValue();
            }
        }
        
        public void ValidateSettings()
        {
            cycles = Math.Max(-1, cycles);
            if (cycles == 0) cycles = 1;
        }
        
        public void ValidatePlayerReference(TweenSequencer root)
        {
            foreach (var step in steps)
            {
                if (step is SequenceStep sequenceStep)
                    sequenceStep.ValidatePlayerReference(root);
                if (step is PlaySequencerStep playSequencerStep)
                    playSequencerStep.ValidatePlayerReference(root);
            }
        }
        
        public void ValidatePlayerReference(HashSet<TweenSequencer> visited)
        {
            foreach (var step in steps)
            {
                if (step is SequenceStep sequenceStep)
                    sequenceStep.ValidatePlayerReference(visited);
                if (step is PlaySequencerStep playSequencerStep)
                    playSequencerStep.ValidatePlayerReference(visited);
            }
        }
        
        
        private float CalculateDuration()
        {
            // TODO: Calculate sequence total duration by iterating to each loop
            // TODO: Warning for PlayerStep: self-reference will cause StackOverflowException upon calling PredictDuration()
            
            return 0f;
        }
        
        
    }
}
