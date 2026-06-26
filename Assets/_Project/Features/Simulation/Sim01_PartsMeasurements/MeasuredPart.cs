using ProjectATLAS.Types;
using UnityEngine;

namespace ProjectATLAS.Simulation.Sim01_PartsMeasurements
{
    public class MeasuredPart : MonoBehaviour
    {
        [SerializeField] private new string name;
        [SerializeField] private float length;
        [SerializeField] private Range randomRange;
        [SerializeField, Range(0, 4)] private int rounding;
        [SerializeField] private float originalLength;
        
        // PROPERTIES
        public string Name => name;
        public float Length => length;
        public Range RandomRange => randomRange;
        
        
        // MONOBEHAVIOUR METHODS
        private void OnValidate()
        {
            SetLength(length);
        }
        
        
        // PUBLIC METHODS
        public void SetLength(float length)
        {
            this.length = length;
            UpdateScale();
        }
        
        private void UpdateScale()
        {
            if (originalLength <= 0f) return;
            
            float scaleFactor = length / originalLength;
            transform.localScale = Vector3.one * scaleFactor;
        }
    }
}
