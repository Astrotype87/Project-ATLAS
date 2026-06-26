using UnityEngine;

namespace ProjectATLAS
{
    public class Bootstrapper
    {
        public const string GameManagerPrefabPath = "Game Manager";
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeGameManager()
        {
            GameObject prefab = Resources.Load<GameObject>(GameManagerPrefabPath);
            
            if (prefab != null)
            {
                GameObject gameObject = Object.Instantiate(prefab);
                gameObject.name = "Game Manager";
            }
            else
            {
                Debug.LogError("GameManager prefab not found in Resources!");
            }
        }
        
    }
}
