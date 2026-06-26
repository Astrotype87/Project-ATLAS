using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KBCore.Refs;
using ProjectATLAS.Authentication;
using System.Threading.Tasks;
using ProjectATLAS.UI;

namespace ProjectATLAS.Menu
{
    public class DeleteAccountDialog : UIDialog
    {
        [Header("Components")]
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private UIToggleButton showPasswordToggle;
        [SerializeField] private TMP_Text passwordInfoText;
        [SerializeField] private Button deleteButton;
        // [SerializeField] private CountdownButton deleteButton;
        
        [Header("References")]
        [SerializeField, Scene] private UIOverlay overlay;
        [SerializeField, Scene] private AccountPage accountPage;
        [SerializeField, Scene] private WelcomePage welcomePage;
        
        
        // MONOBEHAVIOUR METHODS
        protected override void Start()
        {
            base.Start();
            
            passwordInput.onValueChanged.AddListener(text => {
                if (!string.IsNullOrEmpty(passwordInfoText.text)) passwordInfoText.text = "";
            });
            showPasswordToggle.OnValueChanged += OnToggleShowPassword;
            deleteButton.onClick.AddListener(OnDelete);
        }
        
        
        // PUBLIC METHODS
        public override void ResetDialog()
        {
            passwordInput.text = string.Empty;
            passwordInput.contentType = TMP_InputField.ContentType.Password;
            passwordInput.ForceLabelUpdate();
            
            showPasswordToggle.SetValue(false);
            
            passwordInfoText.text = string.Empty;
        }
        
        
        // EVENT LISTENER METHODS
        private void OnToggleShowPassword(UIToggleButton toggleButton, bool isOn)
        {
            passwordInput.contentType = isOn
                ? TMP_InputField.ContentType.Standard
                : TMP_InputField.ContentType.Password;
            passwordInput.ForceLabelUpdate();
        }
        
        private async void OnDelete()
        {
            await DeleteAccountAsync();
        }
        
        
        // PRIVATE METHODS
        private async Task DeleteAccountAsync()
        {
            // Get username and password
            string username = AuthenticationManager.Instance.CachedUsername;
            string password = passwordInput.text;
            
            bool isPasswordValid = ValidatePassword(password);
            if (!isPasswordValid)
            {
                return;
            }
            
            // Close dialog first to avoid overlay being hidden behind
            base.CloseDialog();
            // Display deleting account overlay message
            overlay.DisplayLoading("Deleting account...", "Please wait a moment.", UIOverlay.Icon.Loading, "");
            
            AuthResult authResult = await AuthenticationManager.Instance.DeleteAccountAsync();
            if (authResult.Status == DeleteAccountResult.Success)
            {
                overlay.DisplayMessage("Account Deleted",
                    "Your account has been successfully deleted, including your local and cloud game data.",
                    UIOverlay.Icon.Success, "Return to profile screen");
                
                overlay.DoAfterClosing(() =>
                {
                    welcomePage.PageGroup.ClearPageStack();
                    welcomePage.OpenPageInGroup();
                });
            }
            else
            {
                overlay.DisplayMessage("Delete account failed",
                    authResult.Message,
                    UIOverlay.Icon.Failed, "Press anywhere to close");
            }
        }
        
        private bool ValidatePassword(string password)
        {
            bool isNullOrEmpty = string.IsNullOrEmpty(password);
            bool isPasswordMatch = password.Equals(AuthenticationManager.Instance.CachedPassword);
            
            if (isNullOrEmpty)
            {
                passwordInfoText.text = "Password is empty.";
                return false;
            }
            else if (!isPasswordMatch)
            {
                passwordInfoText.text = "Incorrect password.";
                return false;
            }
            else
            {
                passwordInfoText.text = string.Empty;
                return true;
            }
        }
        
        
        
    }
}
