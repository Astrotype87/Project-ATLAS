using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectATLAS
{
    public class UIAutoMinWidth : MonoBehaviour
    {
        [SerializeField, Self] private LayoutElement layoutElement;
        
        private void Start()
        {
            RectTransform parentRectTransform = transform.parent as RectTransform;
            layoutElement.minWidth = parentRectTransform.rect.width;
        }
        
        private void OnValidate()
        {
            this.ValidateRefs();
        }
    }
}
