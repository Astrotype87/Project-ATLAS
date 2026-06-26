using TMPro;
using UnityEngine;
using KBCore.Refs;

namespace ProjectATLAS.UI
{
    public class UIButton : MonoBehaviour
    {
        [SerializeField, Child] private TMP_Text textComponent;
        [SerializeField] private string buttonText;
        
        private void OnValidate()
        {
            this.ValidateRefs();
            
            gameObject.name = string.IsNullOrWhiteSpace(buttonText) ? "Button" : $"{buttonText} Button";
            
            if (textComponent) textComponent.text = buttonText;
        }
    }
}
