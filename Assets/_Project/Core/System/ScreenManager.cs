using UnityEngine;
using UnityEngine.SceneManagement;
using AstrotypeTools.InspectorAttributes;

using ProjectATLAS.UI;

namespace ProjectATLAS.System
{
    public class ScreenManager : MonoBehaviour
    {
        [SerializeField] private bool autoDetect;
        [SerializeField, DisableIf(nameof(autoDetect))] private Padding padding;
        
        [Header("References")]
        [SerializeField] private UIScreenSafe[] screenSaves;
        
        
        // MONOBEHAVIOUR METHODS
        private void OnEnable()
        {
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }
        
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }
        
        
        // EVENT LISTENER METHODS
        private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            screenSaves = FindObjectsByType<UIScreenSafe>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            
            foreach (UIScreenSafe screenSafe in screenSaves)
            {
                screenSafe.AutoDetect = autoDetect;
                screenSafe.SetPadding(padding);
                screenSafe.UpdateSafeArea();
            }
        }
    }
}
