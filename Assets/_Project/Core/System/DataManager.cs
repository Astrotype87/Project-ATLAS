using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectATLAS.System
{
    using ProjectATLAS.Architecture;
    using ProjectATLAS.GameData;
    
    public class DataManager : MonoBehaviour
    {
        [SerializeField] private GameDataManager gameDataService;
        
        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }
        
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }
        
        // PRIVATE METHODS
        private void HandleSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            InjectToReceivers();
            Debug.Log($"New scene loaded! --- Scene: {scene.name}");
        }
        
        private void InjectToReceivers()
        {
            foreach (var script in FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (script is IInject<IGameDataService> receiver)
                {
                    // receiver.Inject(gameDataService);
                }
            }
        }
    }
}
