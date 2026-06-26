using UnityEngine;

namespace ProjectATLAS.System
{
    public class FPSManager : MonoBehaviour
    {
        [SerializeField] private bool useDefaultFPS = true;
        [SerializeField] private int targetFPS = 60;
        [SerializeField] private int defaultMobileFPS = 60;
        
        private void Start()
        {
            UpdateFPS();
        }
        
        private void OnValidate()
        {
            if (Application.isPlaying) UpdateFPS();
        }
        
        private void UpdateFPS()
        {
            Application.targetFrameRate = useDefaultFPS
                ? Application.isMobilePlatform
                    ? defaultMobileFPS
                    : -1
                : targetFPS;
        }
    }
}
