using UnityEngine;
using UnityEngine.EventSystems;

using CustomInspector;

namespace ProjectATLAS.Input
{
    public class Draggable2D : MonoBehaviour,
        IBeginDragHandler, IDragHandler, IEndDragHandler,
        IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private bool snapToPointer;
        [SerializeField] private bool enableGridSnap;
        [SerializeField, ShowIf(nameof(enableGridSnap))] private Vector2 gridSnap = new(0.1f, 0.1f);
        
        private Vector2 _dragOffset;
        private Camera _camera;
        private new Rigidbody2D rigidbody2D;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
        }
        
        
        // PUBLIC METHODS
        public void SetEnableGridSnap(bool enableGridSnap) => this.enableGridSnap = enableGridSnap;
        public void SetGridSnap(Vector2 gridSnap) => this.gridSnap = gridSnap;
        
        
        // HANDLER METHODS
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (rigidbody2D != null) rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            
            Vector2 transformPos = transform.position;
            Vector2 clickPos = eventData.position;
            
            _camera = eventData.pressEventCamera;
            if (!_camera) _camera = Camera.main;
            Vector2 worldPointerPos = _camera.ScreenToWorldPoint(clickPos);
            
            _dragOffset = transformPos - worldPointerPos;
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            Vector2 clickPos = eventData.position;
            Vector2 worldPoint = _camera.ScreenToWorldPoint(clickPos);
            if (!snapToPointer) worldPoint += _dragOffset;
            
            transform.position = enableGridSnap ? Snapping.Snap(worldPoint, gridSnap) : worldPoint;
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            _dragOffset = Vector2.zero;
            if (rigidbody2D != null) rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
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
