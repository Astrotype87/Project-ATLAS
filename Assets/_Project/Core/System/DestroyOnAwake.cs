using UnityEngine;

namespace ProjectATLAS.System
{
    public class DestroyOnAwake : MonoBehaviour
    {
        private void Awake()
        {
            Destroy(this.gameObject);
        }
    }
}
