using UnityEngine;
using PrimeTween;

namespace AstrotypeTools.PrimeTweenSequencer
{
    public class CameraOrthographicSize : StandardTween<Camera, float>
    {
        protected override float CurrentValue
        {
            get => target.orthographicSize;
            set => target.orthographicSize = value;
        }
        protected override Tween CreateTween(TweenSettings<float> tweenSettings)
            => Tween.CameraOrthographicSize(target, tweenSettings);
        
        protected override float GetFollowTargetOffset(Camera followTarget, float offset)
            => followTarget.orthographicSize + offset;
        
        protected override float GetSumOfValues(float a, float b) => a + b;
    }
}
