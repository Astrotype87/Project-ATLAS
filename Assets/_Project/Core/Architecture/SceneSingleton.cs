using UnityEngine;

namespace ProjectATLAS.Architecture
{
    /// <summary>
    /// A singleton that doesn't persist across scenes.
    /// Destroys game object if an instance of this component exists.
    /// </summary>
    public class SceneSingleton<T> : MonoBehaviour where T : SceneSingleton<T>
    {
        public static T Instance { get; private set; }
        
        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
