using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

namespace AstrotypeTools.PrimeTweenSequencer
{
    public class UIColor : StandardTween<Graphic, Color>
    {
        protected override Color CurrentValue
        {
            get => target.color;
            set => target.color = value;
        }
        protected override Tween CreateTween(TweenSettings<Color> tweenSettings)
            => Tween.Color(target, tweenSettings);
        
        protected override Color GetFollowTargetOffset(Graphic followTarget, Color offset)
            => followTarget.color + offset;
        
        protected override Color GetSumOfValues(Color a, Color b) => a + b;
    }
    
    public class ShadowColor : StandardTween<Shadow, Color>
    {
        protected override Color CurrentValue
        {
            get => target.effectColor;
            set => target.effectColor = value;
        }
        protected override Tween CreateTween(TweenSettings<Color> tweenSettings)
            => Tween.Color(target, tweenSettings);
        
        protected override Color GetFollowTargetOffset(Shadow followTarget, Color offset)
            => followTarget.effectColor + offset;
        
        protected override Color GetSumOfValues(Color a, Color b) => a + b;
    }
    
    public class SpriteColor : StandardTween<SpriteRenderer, Color>
    {
        protected override Color CurrentValue
        {
            get => target.color;
            set => target.color = value;
        }
        protected override Tween CreateTween(TweenSettings<Color> tweenSettings)
            => Tween.Color(target, tweenSettings);
        
        protected override Color GetFollowTargetOffset(SpriteRenderer followTarget, Color offset)
            => followTarget.color + offset;
        
        protected override Color GetSumOfValues(Color a, Color b) => a + b;
    }
}
