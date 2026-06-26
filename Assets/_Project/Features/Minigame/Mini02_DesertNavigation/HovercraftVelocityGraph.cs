using ProjectATLAS.Gameplay.UI;
using UnityEngine;

namespace ProjectATLAS.Minigame.Mini02_DesertNavigation
{
    public class HovercraftVelocityGraph : MonoBehaviour
    {
        [Header("Hovercraft Reference")]
        [SerializeField] private new Rigidbody2D rigidbody2D;
        
        [Header("UI References")]
        [SerializeField] private ArrowView xArrowView;
        [SerializeField] private ArrowView yArrowView;
        [SerializeField] private ArrowView xyArrowView;
        
        private void LateUpdate()
        {
            Vector2 linearVelocity = rigidbody2D.linearVelocity;
            
            xArrowView.UpdateArrow(linearVelocity.x);
            yArrowView.UpdateArrow(linearVelocity.y);
            xyArrowView.UpdateArrow(linearVelocity.magnitude, -AngleFromVector(linearVelocity));
        }
        
        float AngleFromVector(Vector2 dir)
        {
            // Normalize to avoid magnitude issues
            dir.Normalize();
            
            // Swap X/Y and invert X to make 0° = up, 90° = left
            float angle = Mathf.Atan2(-dir.x, dir.y) * Mathf.Rad2Deg;
            
            // Normalize to 0–360 range (optional)
            if (angle < 0) angle += 360f;
            
            return angle;
        }
    }
}
