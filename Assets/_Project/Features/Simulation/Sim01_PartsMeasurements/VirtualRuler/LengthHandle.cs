using UnityEngine;
using UnityEngine.EventSystems;

namespace ProjectATLAS.Simulation.Sim01
{
    using AstrotypeTools.InspectorAttributes;
    using ProjectATLAS.Input;

    public class LengthHandle : MonoBehaviour,
        IBeginDragHandler, IDragHandler, IEndDragHandler,
        IPointerDownHandler, IPointerUpHandler
    {
        [Header("References")]
        [SerializeField] private RectTransform targetToExpand;
        [SerializeField] private NumberSpawner numberSpawner;
        [SerializeField] private LengthHandle lengthHandle;
        
        [Header("Settings")]
        [SerializeField] private float minLength = 2f;
        [SerializeField] private float maxLength = 10f;
        [SerializeField] private float snapInterval = 0.1f;
        [SerializeField] private bool snapToPointer;
        
        private Vector2 _dragOffset;
        private Camera _camera;
        
        
        private void UpdateRulerLength(float handleDistance)
        {
            RectTransform rectTransform = transform as RectTransform;
            
            float length = handleDistance - rectTransform.anchoredPosition.x;
            length = SnapValue(length, snapInterval);
            length = Mathf.Clamp(length, minLength, maxLength);
            
            Vector2 sizeDelta = targetToExpand.sizeDelta;
            sizeDelta.x = length;
            targetToExpand.sizeDelta = sizeDelta;
            
            numberSpawner.UpdateNumber(length);
        }
        
        private float SnapValue(float value, float interval)
        {
            return Mathf.Round(value / interval) * interval;
        }
        
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            _camera = Camera.main;
            
            Vector2 clickPos = eventData.position;
            Vector2 worldPoint = _camera.ScreenToWorldPoint(clickPos);
            
            Vector2 transformPos = transform.position;
            _dragOffset = transformPos - worldPoint;
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            Vector2 clickPos = eventData.position;
            Vector2 worldPoint = _camera.ScreenToWorldPoint(clickPos);
            
            if (!snapToPointer) worldPoint += _dragOffset;
            
            float localPosX = transform.parent.InverseTransformPoint(worldPoint).x;
            
            UpdateRulerLength(localPosX);
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            _dragOffset = Vector2.zero;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            InputInteractionState.IsDragging = true;
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            InputInteractionState.IsDragging = false;
        }
    }
}
