using ProjectATLAS.Lesson;
using UnityEngine;

namespace ProjectATLAS.Simulation
{
    public class SineWave : MonoBehaviour
    {
        [Header("Component")]
        [SerializeField] private LineRenderer lineRenderer;
        
        [Header("Sliders")]
        [SerializeField] private VariableSlider frequencySlider;
        [SerializeField] private VariableSlider amplitudeSlider;
        [SerializeField] private VariableSlider phaseSlider;
        
        [Header("Settings")]
        [SerializeField] private int points;
        [SerializeField] private float frequency = 1;
        [SerializeField] private float amplitude = 1;
        [SerializeField] [Range(-179, 180)] private float phase = 0;
        [SerializeField] private Vector2 xLimits = new(0, 1);
        [SerializeField] private float movementSpeed = 1;
        [Range(0, Tau)]
        [SerializeField] private float radians;
        
        [Header("Scale")]
        [SerializeField] private Vector2 scaleFactor = Vector2.one;
        
        private const float Tau = 2 * Mathf.PI;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            frequencySlider.OnSliderUpdated += FrequencySlider_OnSliderUpdated;
            amplitudeSlider.OnSliderUpdated += AmplitudeSlider_OnSliderUpdated;
            phaseSlider.OnSliderUpdated += PhaseSlider_OnSliderUpdated;
        }
        
        private void Update()
        {
            DrawSineWave();
        }
        
        
        // EVENT LISTENER METHODS
        private void FrequencySlider_OnSliderUpdated(string name, double value)
        {
            frequency = (float)value;
        }
        
        private void AmplitudeSlider_OnSliderUpdated(string name, double value)
        {
            amplitude = (float)value;
        }
        
        private void PhaseSlider_OnSliderUpdated(string name, double value)
        {
            phase = (float)value;
        }
        
        
        
        
        // PRIVATE METHODS
        private void DrawSineWave()
        {
            float xStart = xLimits.x;
            float xFinish = xLimits.y;
            
            lineRenderer.positionCount = points;
            
            for (int i = 0; i < points; i++)
            {
                float progress = (float) i / (points - 1);
                
                float x = Mathf.Lerp(xStart, xFinish, progress);
                float phaseRad = phase * Mathf.Deg2Rad;
                float y = amplitude * Mathf.Sin((Tau * frequency * x) + (Time.timeSinceLevelLoad * movementSpeed) + phaseRad);
                
                lineRenderer.SetPosition(i, new Vector3(x, y, 0) * scaleFactor);
            }
        }
    }
}
