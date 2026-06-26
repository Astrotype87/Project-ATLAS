using UnityEngine;

namespace ProjectATLAS.Animation
{
    using Animation = UnityEngine.Animation;
    
    /// <summary> Adjusts the legacy animation weight. </summary>
    public class AnimationWeight : MonoBehaviour
    {
        [SerializeField] private Animation animationComponent;
        [SerializeField] private float weight;
        
        private void Start()
        {
            string clipName = animationComponent.clip.name;
            
            animationComponent[clipName].enabled = true;
            animationComponent[clipName].weight = weight;
        }
        
        private void Update()
        {
            string clipName = animationComponent.clip.name;
            
            animationComponent[clipName].weight = weight;
        }
    }
}
