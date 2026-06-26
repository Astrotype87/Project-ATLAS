using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace ProjectATLAS.Input
{
    public class InputButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Optional Keyboard Input (New Input System)")]
        public Key key;
        
        public float Value { get; private set; }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            Value = 1;
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            Value = 0;
        }
        
        private void Update()
        {
            if (Keyboard.current == null || key == Key.None)
                return;
            
            // Key pressed
            if (Keyboard.current[key].wasPressedThisFrame)
            {
                Value = 1;
            }
            
            // Key released
            if (Keyboard.current[key].wasReleasedThisFrame)
            {
                Value = 0;
            }
        }
    }
}
