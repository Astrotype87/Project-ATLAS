using UnityEngine;

namespace ProjectATLAS.Architecture
{
    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
    {
        public static T Instance { get; private set; }
        
        protected virtual void Start()
        {
            Debug.Log($"SingletonScriptableObject.Awake()");
            
            if (Instance == null)
            {
                Instance = this as T;
            }
            else
            {
                Debug.LogWarning($"An instance of {typeof(T)} already exists. Destroying duplicate instance.");
                Destroy(this);
            }
        }
    }
}
