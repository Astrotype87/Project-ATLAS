using UnityEngine;

namespace ProjectATLAS
{
    public class GizmoHologram : MonoBehaviour
    {
        [SerializeField] private ParticleSystem hologramParticle;
        [SerializeField, Range(0, 1)] private float amount;
        [SerializeField, Range(0, 1)] private float maxAlpha;
        
        private void OnValidate()
        {
            var main = hologramParticle.main;
            Color color = main.startColor.color;
            
            color.a = maxAlpha * amount;
            main.startColor = color;
        }
        
        private void Update()
        {
            var main = hologramParticle.main;
            Color color = main.startColor.color;
            
            color.a = maxAlpha * amount;
            main.startColor = color;
        }
    }
}
