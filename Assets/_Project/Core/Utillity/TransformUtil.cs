using UnityEngine;

namespace ProjectATLAS.Utility
{
    public static class TransformUtil
    {
        public static void SetLocalPositionX(this Transform transform, float x)
            => transform.localPosition = new(x, transform.localPosition.y, transform.localPosition.z);
        
        public static void SetLocalPositionY(this Transform transform, float y)
            => transform.localPosition = new(transform.localPosition.x, y, transform.localPosition.z);
        
        public static void SetLocalPositionZ(this Transform transform, float z)
            => transform.localPosition = new(transform.localPosition.x, transform.localPosition.y, z);
    }
}
