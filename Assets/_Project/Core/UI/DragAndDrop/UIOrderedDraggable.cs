using UnityEngine;
using UnityEngine.EventSystems;
using KBCore.Refs;
using UnityEngine.UI;

namespace ProjectATLAS.UI.DragAndDrop
{
    public class UIOrderedDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private bool snapToPointer;
        [SerializeField, Parent] private UIOrderedDropContainer parentContainer;
        
        private RectTransform _rectTransform;
        private Vector2 _dragOffset;
        
        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
        }
        
        
        public void SetParentContainer(UIOrderedDropContainer parentContainer)
        {
            this.parentContainer = parentContainer;
        }
        
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            Vector2 transformPos = transform.position;
            Vector2 clickPos = eventData.position;
            
            _dragOffset = transformPos - clickPos;
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            Vector2 dragPosition = eventData.position;
            if (!snapToPointer) dragPosition += _dragOffset;
            
            parentContainer.UpdateCurrentlyDraggingItem(_rectTransform, eventData);
            transform.position = dragPosition;
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            LayoutRebuilder.MarkLayoutForRebuild(parentContainer.transform as RectTransform);
        }
    }
}
