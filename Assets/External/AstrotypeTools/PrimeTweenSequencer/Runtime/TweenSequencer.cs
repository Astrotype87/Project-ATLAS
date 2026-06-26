using System;
using UnityEngine;
using PrimeTween;
using System.Collections.Generic;
using UnityEngine.Profiling;

namespace AstrotypeTools.PrimeTweenSequencer
{
    /// <summary>
    /// Create sequence of tweens in the inspector powered by PrimeTween.
    /// Preview animation using play controls.
    /// </summary>
    public class TweenSequencer : MonoBehaviour
    {
        public enum AutoPlayMode
        {
            None, Awake, Start
        }
        
        [SerializeField] private AutoPlayMode autoPlayMode;
        [SerializeField] private SequenceCreator sequence;
        
        private Sequence _currentSequence;
        private PlayerState _playerState;
        
        
        #if UNITY_EDITOR
        public bool e_expandPlayer;
        public float e_progress;
        public bool e_smoothRefresh;
        public bool e_expandSequence;
        #endif
        
        public SequenceCreator Sequence => sequence;
        public PlayerState PlayerState => _playerState;
        public bool IsInfiniteCycle => sequence.IsInfiniteCycle;
        
        /// <summary> Callback when the sequence has started. </summary>
        public event Action OnStarted;
        /// <summary> Callback when the sequence has either stopped, completed or reset. </summary>
        public event Action OnEnded;
        
        
        private static readonly Action<TweenSequencer> OnSequenceCompleteCallback
            = player => player.Sequence_OnComplete();
        
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            if (autoPlayMode == AutoPlayMode.Awake)
            {
                Play();
            }
        }
        
        private void Start()
        {
            if (autoPlayMode == AutoPlayMode.Start)
            {
                Play();
            }
        }
        
        private void OnDestroy()
        {
            Stop();
        }
        
        
        // PUBLIC METHODS: PLAYER
        public void SetSequence(SequenceCreator sequence)
        {
            this.sequence = sequence;
        }
        
        public void Restart()
        {
            RestoreOriginalValues();
            SaveOriginalValues();
            StopSequence();
            StartSequence();
            PauseSequence();
            SetSequenceProgress(0f);
            
            _playerState = PlayerState.Ready;
        }
        
        public void Play(bool avoidInfiniteCycle = false)
        {
            Profiler.BeginSample("Play Sequence");
            
            PlayAwaitable(avoidInfiniteCycle);
            
            Profiler.EndSample();
        }
        
        public Sequence PlayAwaitable(bool avoidInfiniteCycle = false)
        {
            ValidatePlayerReference();
            
            StopSequence();
            RestoreOriginalValues();
            SaveOriginalValues();
            StartSequence(avoidInfiniteCycle);
            
            return _currentSequence;
        }
        
        public void Pause()
        {
            PauseSequence();
            _playerState = PlayerState.Paused;
        }
        
        public void Resume()
        {
            ResumeSequence();
            _playerState = PlayerState.Playing;
        }
        
        public void Seek(float progress)
        {
            if (!_currentSequence.isAlive)
            {
                RestoreOriginalValues();
                SaveOriginalValues();
                StartSequence();
                PauseSequence();
            }
            SetSequenceProgress(progress);
            
            if (progress <= 0)
                _playerState = PlayerState.Ready;
            else if (progress < 1)
                _playerState = PlayerState.Paused;
            else
                CompleteSequence();
        }
        
        public void Complete()
        {
            if (!_currentSequence.isAlive)
            {
                RestoreOriginalValues();
                SaveOriginalValues();
                StartSequence();
            }
            CompleteSequence();
            
            _playerState = PlayerState.Completed;
        }
        
        public void Stop()
        {
            StopSequence();
            
            _playerState = PlayerState.Inactive;
        }
        
        public void SetTimeScale(float timeScale)
        {
            if (_currentSequence.isAlive)
            {
                _currentSequence.timeScale = timeScale;
            }
        }
        
        public void SaveState()
        {
            SaveOriginalValues();
        }
        
        public void RestoreState()
        {
            RestoreOriginalValues();
        }
        
        
        // PUBLIC METHODS: GETTERS
        public float GetElapsedTime()
        {
            return _currentSequence.isAlive ? _currentSequence.elapsedTimeTotal : 0f;
        }
        
        public float GetDuration()
        {
            return _currentSequence.isAlive
                ? _currentSequence.cyclesTotal == -1 ? _currentSequence.duration : _currentSequence.durationTotal
                : sequence != null ? sequence.TotalDuration : 0f;
        }
        
        public float GetProgress()
        {
            return _currentSequence.isAlive
                ? _currentSequence.cyclesTotal == -1 ? _currentSequence.progress : _currentSequence.progressTotal
                : _playerState == PlayerState.Completed ? 1f : 0f;
        }
        
        
        // PUBLIC METHODS: VALIDATION
        public void ValidatePlayerReference()
        {
            sequence.ValidatePlayerReference(this);
        }
        
        public void ValidatePlayerReference(HashSet<TweenSequencer> visited)
        {
            sequence.ValidatePlayerReference(visited);
        }
        
        
        
        // PRIVATE METHODS: PLAYER INTERNAL
        private void StartSequence(bool avoidInfiniteCycle = false)
        {
            if (sequence == null) return;
            
            _currentSequence = sequence.CreateSequence(avoidInfiniteCycle);
            _currentSequence.OnComplete(this, OnSequenceCompleteCallback);
            
            _playerState = PlayerState.Playing;
            OnStarted?.Invoke();
        }
        
        private void PauseSequence()
        {
            if (_currentSequence.isAlive)
                _currentSequence.isPaused = true;
        }
        
        private void ResumeSequence()
        {
            if (_currentSequence.isAlive)
                _currentSequence.isPaused = false;
        }
        
        private void SetSequenceProgress(float progress)
        {
            if (_currentSequence.isAlive && _currentSequence.cyclesTotal != -1)
                _currentSequence.progressTotal = progress;
        }
        
        private void CompleteSequence()
        {
            if (!_currentSequence.isAlive) return;
            
            if (_currentSequence.cyclesTotal == -1) {
                if (sequence.CycleMode is CycleMode.Yoyo or CycleMode.Rewind)
                    _currentSequence.SetRemainingCycles(true);
                else
                    _currentSequence.SetRemainingCycles(0);
            }
            
            _currentSequence.Complete();
        }
        
        private void StopSequence()
        {
            if (!_currentSequence.isAlive) return;
            
            if (_currentSequence.cyclesTotal == -1) {
                if (sequence.CycleMode is CycleMode.Yoyo or CycleMode.Rewind)
                    _currentSequence.SetRemainingCycles(true);
                else
                    _currentSequence.SetRemainingCycles(0);
            }
            
            _currentSequence.Stop();
            OnEnded?.Invoke();
        }
        
        private void SaveOriginalValues()
        {
            if (_playerState == PlayerState.Inactive && sequence != null)
                sequence.SaveOriginalValue();
        }
        
        private void RestoreOriginalValues()
        {
            if (_playerState != PlayerState.Inactive && sequence != null)
                sequence.RestoreOriginalValue();
        }
        
        
        // CALLBACKS
        private void Sequence_OnComplete()
        {
            _playerState = PlayerState.Completed;
            OnEnded?.Invoke();
        }
        
    }
}
