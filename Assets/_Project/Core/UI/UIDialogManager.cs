using System.Collections.Generic;
using UnityEngine;
using KBCore.Refs;

namespace ProjectATLAS.UI
{
    public class UIDialogManager : MonoBehaviour
    {
        [SerializeField] private GameObject overlay;
        [SerializeField, Child(Flag.Optional)] private UIDialog[] dialogs;
        
        private List<UIDialog> dialogStack = new();
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            foreach (var dialog in dialogs)
            {
                dialog.OnOpenStateChanged += OnOpenStateChanged;
            }
        }
        
        private void OnValidate()
        {
            this.ValidateRefs();
        }
        
        // PRIVATE METHODS
        private void OnOpenStateChanged(UIDialog dialog, bool isOpen)
        {
            if (isOpen)
                dialogStack.Add(dialog);
            else
                dialogStack.Remove(dialog);
            
            if (dialogStack.Count > 0)
                overlay.SetActive(true);
            else
                overlay.SetActive(false);
        }
    }
}
