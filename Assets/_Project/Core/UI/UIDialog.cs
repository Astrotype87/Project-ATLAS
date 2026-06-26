using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using KBCore.Refs;

namespace ProjectATLAS.UI
{
    public class UIDialog : MonoBehaviour
    {
        [Header("UI Dialog")]
        [SerializeField, Child] private TMP_Text titleComponent;
        [SerializeField] private string titleText;
        [SerializeField] private Button closeButton;
        
        public bool IsOpen { get; private set; }
        public event Action<UIDialog, bool> OnOpenStateChanged;
        
        
        // MONOBEHAVIOUR METHODS
        protected virtual void Start()
        {
            ResetDialog();
            
            closeButton.onClick.AddListener(CloseDialog);
        }
        
        protected virtual void OnValidate()
        {
            gameObject.name = string.IsNullOrEmpty(titleText)
                ? "UI Dialog"
                : titleText + " Dialog";
            
            if (titleComponent)
            {
                titleComponent.text = string.IsNullOrEmpty(titleText)
                ? "UI Dialog"
                : titleText;
            }
        }
        
        
        // PUBLIC METHODS
        /// <summary> You must call <c>base.OpenDialog()</c> before adding extra functionality. </summary>
        public virtual void OpenDialog()
        {
            ResetDialog();
            gameObject.SetActive(true);
            IsOpen = true;
            OnOpenStateChanged?.Invoke(this, true);
        }
        
        /// <summary> You must call <c>base.OpenDialog()</c> before adding extra functionality. </summary>
        public virtual void CloseDialog()
        {
            gameObject.SetActive(false);
            IsOpen = false;
            OnOpenStateChanged?.Invoke(this, false);
        }
        
        /// <summary>
        /// Reset state of UI elements in initial state.<br/>
        /// You don't need to call base.ResetDialog() has no code.
        /// </summary>
        public virtual void ResetDialog()
        {
            
        }
    }
}
