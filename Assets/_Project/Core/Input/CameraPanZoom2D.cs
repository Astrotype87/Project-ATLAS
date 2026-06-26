using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace ProjectATLAS.Input
{
    public class CameraPanZoom2D : MonoBehaviour
    {
        [Header("Camera")]
        [SerializeField] private new Camera camera;
        
        [Header("Pan")]
        [SerializeField] private float panSensitivity = 1f;
        [SerializeField] private Transform panArea;
        
        [Header("Zoom")]
        [SerializeField] private float scrollSensitivity = 1f;
        [SerializeField] private float pinchSensitivity = 1f;
        [SerializeField] private float minZoom = 2f;
        [SerializeField] private float maxZoom = 10f;
        
        [SerializeField] private bool _isPressedOverUI;
        
        private bool _isPanning;
        private Vector2 _lastPanPosition;
        
        private bool _isPinching;
        private float _lastPinchDistance;
        
        
        private void Update()
        {
            bool isPressed = Pointer.current.press.isPressed;
            bool wasPressedThisFrame = Pointer.current.press.wasPressedThisFrame;
            bool IsPointerOverGameObject = EventSystem.current.IsPointerOverGameObject();
            
            if (wasPressedThisFrame && IsPointerOverGameObject)
                _isPressedOverUI = true;
            else if (!isPressed)
                _isPressedOverUI = false;
            
            if (InputInteractionState.IsDragging) return;
            
            
            if (!IsPointerOverGameObject)
            {
                HandleScrollToZoom();
            }
            if (!_isPressedOverUI)
            {
                HandlePinchToZoom();
                HandlePanning();
            }
            ClampToPanArea();
        }
        
        
        private void HandleScrollToZoom()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null) return;
            
            Vector3 worldPointBeforeZoom = camera.ScreenToWorldPoint(mouse.position.value);
            
            float scrollDelta = mouse.scroll.y.value;
            float targetZoom = camera.orthographicSize + scrollSensitivity * -scrollDelta;
                camera.orthographicSize = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            
            Vector3 worldPointAfterZoom = camera.ScreenToWorldPoint(mouse.position.value);
            
            Vector3 worldPointDelta = worldPointBeforeZoom - worldPointAfterZoom;
            camera.transform.position += worldPointDelta;
        }
        
        private void HandlePinchToZoom()
        {
            Touchscreen touchscreen = Touchscreen.current;
            if (touchscreen == null || touchscreen.touches.Count < 2)
            {
                _isPinching = false;
                return;
            }
            
            TouchControl touch0 = touchscreen.touches[0];
            TouchControl touch1 = touchscreen.touches[1];
            if (touch0 == null || touch1 == null)
            {
                _isPinching = false;
                return;
            }
            
            if (!touch0.press.isPressed || !touch1.press.isPressed)
            {
                Debug.Log("Pinch released!");
                _isPinching = false;
                return;
            }
            
            Debug.Log($"touch0.position.value: {touch0.position.value}, touch1.position.value; {touch1.position.value}");
            
            
            Vector2 pinchCenter = (touch0.position.value + touch1.position.value) * 0.5f;
            Vector3 pinchPointBeforeZoom = camera.ScreenToWorldPoint(pinchCenter);
            
            Vector2 worldPoint0 = camera.ScreenToWorldPoint(touch0.position.value);
            Vector2 worldPoint1 = camera.ScreenToWorldPoint(touch1.position.value);
            float pinchDistance = Vector2.Distance(worldPoint0, worldPoint1);
            
            if (!_isPinching)
            {
                _lastPinchDistance = pinchDistance;
                _isPinching = true;
            }
            
            float pinchDelta = (_lastPinchDistance - pinchDistance) * 0.5f;
            float targetZoom = camera.orthographicSize + pinchDelta * pinchSensitivity;
            camera.orthographicSize = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            
            worldPoint0 = camera.ScreenToWorldPoint(touch0.position.value);
            worldPoint1 = camera.ScreenToWorldPoint(touch1.position.value);
            _lastPinchDistance = Vector2.Distance(worldPoint0, worldPoint1);
            
            Vector3 pinchPointAfterZoom = camera.ScreenToWorldPoint(pinchCenter);
            Vector3 pinchPointDelta = pinchPointBeforeZoom - pinchPointAfterZoom;
            camera.transform.position += pinchPointDelta;
        }
        
        private void HandlePanning()
        {
            if (_isPinching)
            {
                _isPanning = false;
                return;
            }
            
            Pointer pointer = Pointer.current;
            if (!pointer.press.isPressed)
            {
                _isPanning = false;
                return;
            }
            
            Vector2 pointerWorldPos = camera.ScreenToWorldPoint(pointer.position.value);
            if (!_isPanning)
            {
                _lastPanPosition = pointerWorldPos;
                _isPanning = true;
            }
            
            Vector2 panDelta = _lastPanPosition - pointerWorldPos;
            camera.transform.Translate(panDelta * panSensitivity);
            
            _lastPanPosition = camera.ScreenToWorldPoint(pointer.position.value);
        }
        
        private void ClampToPanArea()
        {
            if (!panArea) return;
            
            float camHeight = camera.orthographicSize * 2f;
            float camWidth = camHeight * camera.aspect;
            
            Vector2 panAreaCenter = panArea.position;
            Vector2 panAreaSize = panArea.localScale;
            
            Vector3 clampedPos = camera.transform.position;
            
            if (camWidth >= panAreaSize.x)
            {
                clampedPos.x = panAreaCenter.x;
            }
            else
            {
                float left = panAreaCenter.x - (panAreaSize.x - camWidth) / 2f;
                float right = panAreaCenter.x + (panAreaSize.x - camWidth) / 2f;
                clampedPos.x = Mathf.Clamp(clampedPos.x, left, right);
            }
            
            if (camHeight >= panAreaSize.y)
            {
                clampedPos.y = panAreaCenter.y;
            }
            else
            {
                float bottom = panAreaCenter.y - (panAreaSize.y - camHeight) / 2f;
                float top = panAreaCenter.y + (panAreaSize.y - camHeight) / 2f;
                clampedPos.y = Mathf.Clamp(clampedPos.y, bottom, top);
            }
            
            camera.transform.position = clampedPos;
        }
    }
}
