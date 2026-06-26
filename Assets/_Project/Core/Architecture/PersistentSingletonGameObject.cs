using UnityEngine;

namespace ProjectATLAS.Architecture
{
    /// <summary>
    /// The game object this component is attached to will persist across scenes.<br/>
    /// Destroys game object if an instance of this component exists.
    /// </summary>
    public class PersistentSingletonGameObject<T> : MonoBehaviour where T : PersistentSingletonGameObject<T>
    {
        public static T Instance { get; private set; }
        
        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                if (gameObject.transform.parent == null)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
