using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using KBCore.Refs;

namespace ProjectATLAS.Dialogue
{
    public class DialogueAnimator : MonoBehaviour
    {
        [SerializeField, Self] private PlayableDirector playableDirector;
        
        public bool IsAnimating { get; private set; }
        
        private bool isActive;
        private Coroutine playCoroutine;
        private float targetEndTime;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            playableDirector.stopped += PlayableDirector_stopped;
        }
        
        private void OnValidate()
        {
            this.ValidateRefs();
        }
        
        private void PlayableDirector_stopped(PlayableDirector playableDirector)
        {
            isActive = false;
        }
        
        
        // PUBLIC METHODS
        public void PlaySection(float startTime, float endTime)
        {
            if (playCoroutine != null) StopCoroutine(playCoroutine);
            
            playCoroutine = StartCoroutine(PlayInRange(startTime, endTime));
        }
        
        public void SkipToTime(float time)
        {
            SetTime(time);
        }
        
        public void SkipAnimation()
        {
            if (IsAnimating)
            {
                IsAnimating = false;
                playableDirector.time = targetEndTime;
                playableDirector.Evaluate();
                playableDirector.Pause();
            }
        }
        
        public void ResetToStart()
        {
            playableDirector.time = 0;
            playableDirector.Evaluate();
            
            // playableDirector.Stop();
            isActive = false;
            IsAnimating = false;
        }
        
        // PRIVATE METHODS
        private IEnumerator PlayInRange(float startTime, float endTime)
        {
            if (!isActive)
            {
                playableDirector.Play();
                isActive = true;
            }
            
            targetEndTime = endTime;
            
            
            playableDirector.time = startTime;
            playableDirector.Resume();
            IsAnimating = true;
            
            float duration = Mathf.Max(endTime - startTime, 0);
            yield return new WaitForSeconds(duration);
            
            IsAnimating = false;
            playableDirector.Pause();
            playableDirector.time = endTime;
            playableDirector.Evaluate();
            
            
            playCoroutine = null;
        }
        
        private void SetTime(float time)
        {
            if (!isActive)
            {
                // Ensure the director is at least playing once so tracks are evaluated
                playableDirector.Play();
                isActive = true;
            }
            
            IsAnimating = false;
            if (playCoroutine != null)
            {
                StopCoroutine(playCoroutine);
                playCoroutine = null;
            }
            
            // Set timeline time directly and evaluate
            playableDirector.time = time;
            playableDirector.Evaluate();  // forces timeline to update scene objects immediately
            playableDirector.Pause();     // pause so it doesn’t continue playing
        }
        
    }
}
