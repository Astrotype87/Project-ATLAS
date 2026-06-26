using UnityEngine;

namespace AstrotypeTools.PrimeTweenSequencer
{
    public enum StartMode
    {
        Current,
        Absolute,
        Relative,
        // Additive
    }
    public enum EndMode
    {
        Absolute,
        Relative,
        // Additive,
        Follow
    }
    public enum CustomEase
    {
        Curve, Overshoot, Bounce, Elastic, BounceExact
    }
    public enum Direction
    {
        Forward, Reverse
    }
}
