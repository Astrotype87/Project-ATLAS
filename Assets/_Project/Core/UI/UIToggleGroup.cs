using System;
using KBCore.Refs;
using UnityEngine;

namespace ProjectATLAS.UI
{
    public class UIToggleGroup : MonoBehaviour
    {
        [SerializeField, Child(Flag.Editable)] private UIToggleButton[] toggleButtons;
        
        // PROPERTIES
        public event Action<UIToggleButton> OnToggleChanged;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            for (int i = 0; i < toggleButtons.Length; i++)
            {
                toggleButtons[i].OnValueChanged += UpdateToggleButtons;
            }
        }
        
        private void OnValidate()
        {
            this.ValidateRefs();
        }
        
        
        // PRIVATE METHODS
        private void UpdateToggleButtons(UIToggleButton enabledButton, bool isOn)
        {
            for (int i = 0; i < toggleButtons.Length; i++)
            {
                toggleButtons[i].SetValueIsolated(toggleButtons[i] == enabledButton);
            }
            
        }
    }
}
