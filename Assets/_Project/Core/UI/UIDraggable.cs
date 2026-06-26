using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ProjectATLAS.UI
{
    public class UIDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private bool snapToPointer;
        [SerializeField] private bool resetIfNotAccepted;
        [SerializeField] private Transform dragArea;
        
        private Vector2 _lastPosition;
        private Transform _lastParent;
        private Vector2 _dragOffset;
        
        private Graphic[] _graphics;
        private bool[] _raycastTargets;
        
        private UIDropArea _currentDropArea;
        
        
        
        public void SetDropArea(UIDropArea dropArea)
        {
            if (_currentDropArea)
            {
                _currentDropArea.StoredGameObjects.Remove(this.gameObject);
            }
            _currentDropArea = dropArea;
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            Vector2 transformPos = transform.position;
            Vector2 clickPos = eventData.position;
            
            _dragOffset = transformPos - clickPos;
            _lastPosition = transformPos;
            _lastParent = transform.parent;
            
            if (transform.parent.TryGetComponent(out UIDropArea dropArea))
            {
                transform.SetParent(dropArea.transform.parent);
            }
            else
            {
                transform.SetSiblingIndex(transform.childCount - 1);
            }
            
            
            _graphics = GetComponentsInChildren<Graphic>(true);
            _raycastTargets = _graphics.Select(g => g.raycastTarget).ToArray();
            foreach (Graphic graphic in _graphics)
            {
                graphic.raycastTarget = false;
            }
            
            if (_currentDropArea) _currentDropArea.StoredGameObjects.Remove(this.gameObject);
            _currentDropArea = null;
            
            if (dragArea)
            {
                transform.SetParent(dragArea);
            }
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            Vector2 dragPosition = eventData.position;
            if (!snapToPointer) dragPosition += _dragOffset;
            
            transform.position = dragPosition;
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            if (resetIfNotAccepted && !_currentDropArea)
            {
                transform.SetParent(_lastParent);
                transform.position = _lastPosition;
                
                if (_lastParent.gameObject.TryGetComponent(out UIDropArea dropArea))
                {
                    dropArea.StoredGameObjects.Add(this.gameObject);
                    _currentDropArea = dropArea;
                }
            }
            
            _dragOffset = Vector2.zero;
            
            for (int i = 0; i < _graphics.Length; i++)
            {
                _graphics[i].raycastTarget = _raycastTargets[i];
            }
        }
    }
}
