using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using KBCore.Refs;

using ProjectATLAS.UI;
using ProjectATLAS.Utility;

namespace ProjectATLAS.Menu
{
    public class LoginDialog : UIDialog
    {
        [Header("Components")]
        [SerializeField] private Button loginButton;
        
        [Header("References")]
        [SerializeField, Scene] private UIOverlay overlay;
        
        
        // PROPERTIES
        public bool IsConfirmed { get; private set; }
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            loginButton.onClick.AddListener(OnLogin);
        }
        
        // PUBLIC METHODS
        public async Task<bool> WaitForConfirmation()
        {
            IsConfirmed = false;
            
            await TaskUtil.WaitUntilAsync(() => !IsOpen, TimeSpan.FromMilliseconds(100));
            return IsConfirmed;
        }
        
        // EVENT LISTENER METHODS
        private void OnLogin()
        {
            IsConfirmed = true;
            base.CloseDialog();
        }
    }
}
