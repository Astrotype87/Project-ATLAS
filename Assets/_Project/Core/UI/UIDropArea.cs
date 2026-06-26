using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ProjectATLAS.UI
{
    public class UIDropArea : MonoBehaviour, IDropHandler
    {
        [SerializeField] private bool snapPosition = true;
        [SerializeField] private bool storeAsChild = true;
        [SerializeField] private int fillLimit = 1;
        
        public List<GameObject> StoredGameObjects { get; set; } = new();
        public event Action<GameObject> OnDropped;
        
        private void OnValidate()
        {
            fillLimit = Mathf.Max(fillLimit, -1);
        }
        
        public void OnDrop(PointerEventData eventData)
        {
            GameObject droppedObject = eventData.pointerDrag;
            DropObject(droppedObject);
        }
        
        /// <summary> Drops a game object to this drop area via code. </summary>
        public void DropObject(GameObject droppedObject)
        {
            bool isFull = StoredGameObjects.Count >= fillLimit && fillLimit >= 0;
            if (!isFull && droppedObject.TryGetComponent(out UIDraggable draggable))
            {
                draggable.SetDropArea(this);
                StoredGameObjects.Add(draggable.gameObject);
                
                if (storeAsChild)
                {
                    droppedObject.transform.SetParent(this.transform);
                    
                    if (snapPosition)
                    {
                        RectTransform rectTransform = droppedObject.transform as RectTransform;
                        
                        rectTransform.anchorMin = new(0.5f, 0.5f);
                        rectTransform.anchorMax = new(0.5f, 0.5f);
                        rectTransform.pivot = new(0.5f, 0.5f);
                        rectTransform.anchoredPosition = Vector2.zero;
                    }
                    else
                    {
                        droppedObject.transform.position = this.transform.position;
                    }
                }
                else
                {
                    droppedObject.transform.position = this.transform.position;
                }
                
                OnDropped?.Invoke(draggable.gameObject);
            }
        }
        
        public GameObject GetFirstStored()
        {
            return StoredGameObjects.Count == 0 ? null : StoredGameObjects[0];
        }
    }
}
