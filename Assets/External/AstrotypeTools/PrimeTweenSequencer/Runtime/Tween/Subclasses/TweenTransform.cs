using UnityEngine;
using PrimeTween;

namespace AstrotypeTools.PrimeTweenSequencer
{
    public class Position : StandardTween<Transform, Vector3>
    {
        protected override Vector3 CurrentValue
        {
            get => target.position;
            set => target.position = value;
        }
        protected override Tween CreateTween(TweenSettings<Vector3> tweenSettings)
            => Tween.Position(target, tweenSettings);
        
        protected override Vector3 GetFollowTargetOffset(Transform followTarget, Vector3 offset)
            => followTarget.position + offset;
        
        protected override Vector3 GetSumOfValues(Vector3 a, Vector3 b) => a + b;
    }
    
    public class Rotation : StandardTween<Transform, Quaternion>
    {
        protected override Quaternion CurrentValue
        {
            get => target.rotation;
            set => target.rotation = value;
        }
        protected override Tween CreateTween(TweenSettings<Quaternion> tweenSettings)
            => Tween.Rotation(target, tweenSettings);
        
        protected override Quaternion GetFollowTargetOffset(Transform followTarget, Quaternion offset)
            => followTarget.rotation * offset;
        
        protected override Quaternion GetSumOfValues(Quaternion a, Quaternion b) => a * b;
    }
}
