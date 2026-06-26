using System.Collections.Generic;
using UnityEngine;
using KBCore.Refs;

namespace ProjectATLAS.Graphics
{
    public class SpriteAnimation : MonoBehaviour
    {
        [SerializeField, Self] private SpriteRenderer spriteRenderer;
        [SerializeField] private List<Sprite> animationFrames;
        [SerializeField] private float frameRate = 0.1f;
        [SerializeField] private PlaybackMode playbackMode = PlaybackMode.Forward;
        
        private int currentFrame = 0;
        private float timer = 0f;
        private int direction = 1; // Used for PingPong
        
        
        // MONOBEHAVIOUR METHODS
        private void OnValidate()
        {
            spriteRenderer.sprite = animationFrames[1];
        }
        
        private void Update()
        {
            if (animationFrames.Count == 0 || spriteRenderer == null) return;
            
            timer += Time.deltaTime;
            
            if (timer >= frameRate)
            {
                timer -= frameRate;
                
                if (playbackMode == PlaybackMode.Forward)
                {
                    currentFrame = (currentFrame + 1) % animationFrames.Count;
                }
                else if (playbackMode == PlaybackMode.Backward)
                {
                    currentFrame = (currentFrame - 1 + animationFrames.Count) % animationFrames.Count;
                }
                else if (playbackMode == PlaybackMode.PingPong)
                {
                    currentFrame += direction;
                    
                    if (currentFrame >= animationFrames.Count - 1 || currentFrame <= 0)
                    {
                        direction *= -1; // reverse direction
                    }
                }
                
                spriteRenderer.sprite = animationFrames[currentFrame];
            }
        }
        
        
        
        
        // ENUMS
        public enum PlaybackMode
        {
            Forward, Backward, PingPong
        }
    }
}
