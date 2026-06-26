using ProjectATLAS.Architecture;
using UnityEngine;

namespace ProjectATLAS.System
{
    public class CameraManager : PersistentSingletonGameObject<CameraManager>
    {
        [SerializeField] private Camera[] cameras;
        
        protected override void Awake()
        {
            RefreshCameras();
        }
        
        /// <summary> Finds all cameras currently in the scene. </summary>
        public void RefreshCameras()
        {
            cameras = FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        }
        
        /// <summary> Enables only the specified camera and disables all others. </summary>
        public void EnableOnly(Camera target)
        {
            if (cameras == null || cameras.Length == 0)
                RefreshCameras();

            foreach (var cam in cameras)
            {
                if (cam != null)
                    cam.enabled = (cam == target);
            }
        }
        
        /// <summary> Enables a camera by index (based on current list). </summary>
        public void EnableOnly(int index)
        {
            if (cameras == null || cameras.Length == 0)
                RefreshCameras();
            
            if (index >= 0 && index < cameras.Length)
                EnableOnly(cameras[index]);
            else
                Debug.LogWarning("CameraManager: Invalid camera index!");
        }
        
        /// <summary>
        /// Returns all cached cameras.
        /// </summary>
        public Camera[] GetCameras()
        {
            if (cameras == null || cameras.Length == 0)
                RefreshCameras();
            
            return cameras;
        }
    }
}
