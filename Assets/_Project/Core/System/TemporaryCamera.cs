using UnityEngine;
using UnityEngine.SceneManagement;

using ProjectATLAS.Architecture;

namespace ProjectATLAS.System
{
    /// <summary> This camera is automatically destroyed if there are already existing main camera. </summary>
    public class TemporaryCamera : MonoBehaviour
    {
        private void OnEnable()
        {
            Camera[] loadedCameras = FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (loadedCameras.Length > 1) Destroy(gameObject);
        }
        
        private void Start()
        {
            Camera[] loadedCameras = FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (loadedCameras.Length > 1) Destroy(gameObject);
        }
    }
}
