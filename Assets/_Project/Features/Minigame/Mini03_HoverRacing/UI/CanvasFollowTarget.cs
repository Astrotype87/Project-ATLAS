using UnityEngine;

namespace ProjectATLAS.Gameplay
{
    public class CanvasFollowTarget : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Transform followTarget;
        
        private void LateUpdate()
        {
            transform.SetPositionAndRotation(followTarget.position, followTarget.rotation);
        }
    }
}
