using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using KBCore.Refs;
using UnityEngine.UI;
using System;
using PrimeTween;

namespace ProjectATLAS.UI.DragAndDrop
{
    public class UIOrderedDropContainer : MonoBehaviour
    {
        [SerializeField, Self] private LayoutGroup layoutGroup;
        [SerializeField, Child] private List<UIOrderedDraggable> items;
        
        private RectTransform rectTransform;
        
        // PROPERTIES
        public event Action OnIndexUpdated;
        
        
        
        // MONOBEHAVIOUR METHODS
        private void Start()
        {
            rectTransform = transform as RectTransform;
        }
        
        private void OnValidate()
        {
            this.ValidateRefs();
        }
        
        
        // PUBLIC METHODS
        public void SetItemsList(List<GameObject> gameObjects)
        {
            items.Clear();
            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (gameObjects[i].TryGetComponent(out UIOrderedDraggable orderedDraggable))
                {
                    items.Add(orderedDraggable);
                    orderedDraggable.SetParentContainer(this);
                }
            }
        }
        // TWEEN
        public void UpdateCurrentlyDraggingItem(RectTransform currentDraggingItem, PointerEventData eventData)
        {
            // Sequence sequence = Sequence.Create();
            // sequence
            //     .Chain(Tween.Position(this.transform, new Vector3(1, 2, 3), 12))
            //     .Chain(Tween.Rotation(this.transform, new Vector3(1, 2, 3), 12))
            //     .Chain(Tween.Position(this.transform, new Vector3(1, 2, 3), 12))
            //     .Chain(Tween.Position(this.transform, new Vector3(1, 2, 3), 12));
            
            
            
            foreach (var item in items)
            {
                if (item == null )
                {
                    items.Remove(item);
                    continue;
                }
                if (item.transform == null) continue;
                if (item == currentDraggingItem) continue;
                
                RectTransform targetRectTransform = item.transform as RectTransform;
                Vector2 pointerLocalPos = targetRectTransform.InverseTransformPoint(eventData.position);
                
                int currentIndex = currentDraggingItem.GetSiblingIndex();
                int targetIndex = item.transform.GetSiblingIndex();
                
                if (targetRectTransform.rect.Contains(pointerLocalPos) && currentIndex != targetIndex)
                {
                    currentDraggingItem.transform.SetSiblingIndex(targetIndex);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
                    OnIndexUpdated?.Invoke();
                    break;
                }
            }
        }
        
        // public void SwapCurrentlyDraggingItem(RectTransform currentDraggingItem, PointerEventData eventData)
        // {
        //     foreach (var item in items)
        //     {
        //         if (item == currentDraggingItem) continue;
                
        //         RectTransform itemToSwap = item.transform as RectTransform;
        //         Vector2 mousePos = eventData.position;
        //         Camera camera = Camera.main;
                
        //         RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //             itemToSwap, mousePos, camera, out Vector2 localPoint);
                
        //         if (currentDraggingItem.rect.Contains(localPoint) && itemToSwap)
        //         {
        //             currentDraggingItem.transform.SetSiblingIndex(item.transform.GetSiblingIndex());
        //             break;
        //         }
        //     }
        // }
    }
}
