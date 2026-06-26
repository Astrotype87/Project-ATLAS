using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ProjectATLAS.Simulation.Sim01
{
    using ProjectATLAS.Input;
    
    public class RotateHandle : MonoBehaviour,
        IBeginDragHandler, IDragHandler,
        IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform targetToRotate;
        [SerializeField] private float snapInterval = 5f;
        
        private Camera _camera;
        
        private void UpdateRulerAngle(float angle)
        {
            angle = SnapValue(angle, snapInterval);
            
            Vector3 euler = targetToRotate.eulerAngles;
            euler.z = angle;
            targetToRotate.eulerAngles = euler;
        }
        
        private float SnapValue(float value, float interval)
        {
            return Mathf.Round(value / interval) * interval;
        }
        
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            _camera = Camera.main;
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            Vector2 clickPos = eventData.position;
            Vector2 worldPoint = _camera.ScreenToWorldPoint(clickPos);
            
            Vector2 baseVector = Vector2.right * transform.localPosition.magnitude;
            Vector2 draggedVector = worldPoint - (Vector2)transform.parent.position;
            float angle = Vector2.SignedAngle(baseVector, draggedVector);
            
            UpdateRulerAngle(angle);
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
