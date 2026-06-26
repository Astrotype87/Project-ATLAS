using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace ProjectATLAS.Input
{
    [RequireComponent(typeof(RectTransform))]
    public class InputDirection : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        [SerializeField] private RectTransform angleDisplay;
        [SerializeField] private TMP_Text angleText;
        [SerializeField, Range(-180, 180)] private float angle;
        [SerializeField] private bool snapToInt;
        
        public float Angle => angle;
        
        
        // MONOBEHAVIOUR METHODS
        private void OnValidate()
        {
            // Normalize to -180 -> 180
            angle = Mathf.Repeat(angle + 180f, 360f) - 180f;
            
            // Snap to integer
            if (snapToInt) angle = Mathf.Round(angle);
            
            UpdateUI();
        }
        
        // EVENT LISTENER METHODS
        public void OnPointerDown(PointerEventData eventData)
        {
            UpdateAngle(eventData);
            UpdateUI();
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            UpdateAngle(eventData);
            UpdateUI();
        }
        
        // PUBLIC METHODS
        public void SetAngle(float angle)
        {
            this.angle = angle;
        }
        
        // PRIVATE METHODS
        private void UpdateAngle(PointerEventData eventData)
        {
            RectTransform rect = transform as RectTransform;
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rect, eventData.position, eventData.pressEventCamera, out Vector2 localPoint
            );
            
            // Calculate the local center (accounts for pivot offset)
            Vector2 rectCenter = new(
                (0.5f - rect.pivot.x) * rect.rect.width,
                (0.5f - rect.pivot.y) * rect.rect.height
            );
            
            // Direction vector from center -> touch point
            Vector2 direction = localPoint - rectCenter;
            
            // Compute angle from center to touch
            angle = Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg;
            
            // Normalize to -180 -> 180
            angle = Mathf.Repeat(angle + 180f, 360f) - 180f;
            
            // Snap to integer
            if (snapToInt) angle = Mathf.Round(angle);
        }
        
        private void UpdateUI()
        {
            if (angleDisplay)
            {
                Vector3 eulerAngles = angleDisplay.eulerAngles;
                eulerAngles.z = angle;
                angleDisplay.eulerAngles = eulerAngles;
            }
            if (angleText)
            {
                angleText.text = $"{Angle:0}°";
            }
        }
    }
}
