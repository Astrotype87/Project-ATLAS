using UnityEngine;

using ProjectATLAS.Gameplay.UI;
using TMPro;

namespace ProjectATLAS.Minigame.Mini03_HoverRacing
{
    public class HovercraftForceDisplay : MonoBehaviour
    {
        [Header("Hovercraft")]
        [SerializeField] private HovercraftPhysics hovercraftPhysics;
        
        [Header("Arrow")]
        [SerializeField] private ArrowView thrustView;
        [SerializeField] private ArrowView dragView;
        [SerializeField] private ArrowView hoverView;
        [SerializeField] private ArrowView gravityView;
        
        [Header("Text")]
        [SerializeField] private TMP_Text thrustText;
        [SerializeField] private TMP_Text dragText;
        [SerializeField] private TMP_Text hoverText;
        [SerializeField] private TMP_Text gravityText;
        
        private void LateUpdate()
        {
            if (!hovercraftPhysics) return;
            
            float localForwardThrust = Vector2.Dot(hovercraftPhysics.AppliedThrust, hovercraftPhysics.RightDirection);
            float localForwardDrag = Vector2.Dot(hovercraftPhysics.AppliedDrag, hovercraftPhysics.RightDirection);
            float hoverForce = (hovercraftPhysics.AppliedHover + hovercraftPhysics.AppliedPull).y;
            float weightForce = hovercraftPhysics.AppliedWeight.y;
            
            if (thrustView) thrustView.UpdateArrow(localForwardThrust);
            if (dragView) dragView.UpdateArrow(localForwardDrag);
            if (hoverView) hoverView.UpdateArrow(hoverForce);
            if (gravityView) gravityView.UpdateArrow(weightForce);
            
            if (thrustText) thrustText.text = $"Thrust Force\n{localForwardThrust:N0} N";
            if (dragText) dragText.text = $"Drag Force\n{localForwardDrag:N0} N";
            if (hoverText) hoverText.text = $"Hover Force\n{hoverForce:N0} N";
            if (gravityText) gravityText.text = $"Weight Force\n{weightForce:N0} N";
        }
    }
}
