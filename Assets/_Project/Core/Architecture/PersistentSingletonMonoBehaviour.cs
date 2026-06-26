using UnityEngine;

namespace ProjectATLAS.Architecture
{
    /// <summary>
    /// The game object this component is attached to will persist across scenes.<br/>
    /// Destroys component if an instance of this component exists.<br/>
    /// The game object will remain until manually destroyed.
    /// </summary>
    public class PersistentSingletonMonoBehaviour<T> : MonoBehaviour where T : PersistentSingletonMonoBehaviour<T>
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
                Destroy(this);
            }
        }
    }
}
