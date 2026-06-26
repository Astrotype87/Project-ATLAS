using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KBCore.Refs;
using ProjectATLAS.Authentication;
using ProjectATLAS.UI;

namespace ProjectATLAS.Menu
{
    public class ChangePasswordDialog : UIDialog
    {
        [Header("Components")]
        [SerializeField] private TMP_InputField currentPasswordInput;
        [SerializeField] private TMP_InputField newPasswordInput;
        [SerializeField] private TMP_InputField confirmPasswordInput;
        
        [SerializeField] private UIToggleButton showCurrentPasswordToggle;
        [SerializeField] private UIToggleButton showNewPasswordToggle;
        [SerializeField] private UIToggleButton showConfirmPasswordToggle;
        
        [SerializeField] private TMP_Text currentPasswordInfoText;
        [SerializeField] private TMP_Text newPasswordInfoText;
        [SerializeField] private TMP_Text confirmPasswordInfoText;
        
        [SerializeField] private Button changeButton;
        
        [Header("References")]
        [SerializeField, Scene] private UIOverlay overlay;
        
        
        
        // MONOBEHAVIOUR METHODS
        protected override void Start()
        {
            base.Start();
            
            currentPasswordInput.onValueChanged.AddListener(text => ValidateCurrentPassword(text));
            newPasswordInput.onValueChanged.AddListener(text => ValidateNewPassword(text));
            confirmPasswordInput.onValueChanged.AddListener(text => ValidateConfirmPassword(text));
            
            showCurrentPasswordToggle.OnValueChanged += OnToggleShowCurrentPassword;
            showNewPasswordToggle.OnValueChanged += OnToggleShowNewPassword;
            showConfirmPasswordToggle.OnValueChanged += OnToggleShowConfirmPassword;
            
            changeButton.onClick.AddListener(OnChange);
        }
        
        
        // PUBLIC METHODS
        public override void ResetDialog()
        {
            currentPasswordInput.text = string.Empty;
            currentPasswordInput.contentType = TMP_InputField.ContentType.Password;
            currentPasswordInput.ForceLabelUpdate();
            
            newPasswordInput.text = string.Empty;
            newPasswordInput.contentType = TMP_InputField.ContentType.Password;
            newPasswordInput.ForceLabelUpdate();
            
            confirmPasswordInput.text = string.Empty;
            confirmPasswordInput.contentType = TMP_InputField.ContentType.Password;
            confirmPasswordInput.ForceLabelUpdate();
            
            showCurrentPasswordToggle.SetValue(false);
            showNewPasswordToggle.SetValue(false);
            showConfirmPasswordToggle.SetValue(false);
            
            currentPasswordInfoText.text = string.Empty;
            newPasswordInfoText.text = string.Empty;
            confirmPasswordInfoText.text = string.Empty;
        }
        
        
        // EVENT LISTENER METHODS
        private void OnToggleShowCurrentPassword(UIToggleButton toggleButton, bool isOn)
        {
            currentPasswordInput.contentType = isOn
                ? TMP_InputField.ContentType.Standard
                : TMP_InputField.ContentType.Password;
            currentPasswordInput.ForceLabelUpdate();
        }
        
        private void OnToggleShowNewPassword(UIToggleButton toggleButton, bool isOn)
        {
            newPasswordInput.contentType = isOn
                ? TMP_InputField.ContentType.Standard
                : TMP_InputField.ContentType.Password;
            newPasswordInput.ForceLabelUpdate();
        }
        
        private void OnToggleShowConfirmPassword(UIToggleButton toggleButton, bool isOn)
        {
            confirmPasswordInput.contentType = isOn
                ? TMP_InputField.ContentType.Standard
                : TMP_InputField.ContentType.Password;
            confirmPasswordInput.ForceLabelUpdate();
        }
        
        private async void OnChange()
        {
            await ChangePassword();
        }
        
        
        // PRIVATE METHODS
        private async Task ChangePassword()
        {
            // Get password input values
            string currentPassword = currentPasswordInput.text;
            string newPassword = newPasswordInput.text;
            string confirmPassword = confirmPasswordInput.text;
            
            bool isCurrentPasswordValid = ValidateCurrentPassword(currentPassword);
            bool isNewPasswordValid = ValidateNewPassword(newPassword);
            bool isConfirmPasswordValid = ValidateConfirmPassword(confirmPassword);
            if (!isCurrentPasswordValid || !isNewPasswordValid || !isConfirmPasswordValid)
            {
                return;
            }
            
            overlay.DisplayLoading("Updating password...", "Updating your password. Please wait a moment.", UIOverlay.Icon.Loading, "");
            
            AuthResult authResult = await AuthenticationManager.Instance.UpdatePasswordAsync(currentPassword, newPassword);
            if (authResult.Status == UpdatePasswordResult.SamePassword)
            {
                overlay.DisplayMessage("Change password failed", authResult.Message, UIOverlay.Icon.Failed, "Press anywhere to close");
            }
            else if (authResult.Status == UpdatePasswordResult.Success)
            {
                overlay.DisplayMessage("Change password success",
                    "You have successfully updated your password.",
                    UIOverlay.Icon.Success, "Press anywhere to close");
                overlay.DoAfterClosing(() => {
                    base.CloseDialog();
                });
            }
            else
            {
                overlay.DisplayMessage("Change password failed", authResult.Message, UIOverlay.Icon.Failed, "");
            }
        }
        
        private bool ValidateCurrentPassword(string password)
        {
            bool isNullOrEmpty = string.IsNullOrEmpty(password);
            
            if (isNullOrEmpty)
            {
                currentPasswordInfoText.text = "Password is empty.";
                return false;
            }
            else
            {
                currentPasswordInfoText.text = string.Empty;
                return true;
            }
        }
        
        private bool ValidateNewPassword(string password)
        {
            bool isNullOrEmpty = string.IsNullOrEmpty(password);
            bool isValidLength = password.Length >= 8 && password.Length <= 30;
            bool hasLowercase = Regex.IsMatch(password, @"[a-z]");
            bool hasUppercase = Regex.IsMatch(password, @"[A-Z]");
            bool hasDigit = Regex.IsMatch(password, @"[0-9]");
            bool hasSymbol = Regex.IsMatch(password, @"[^a-zA-Z0-9]");
            
            if (isNullOrEmpty)
            {
                newPasswordInfoText.text = "Password is empty.";
                return false;
            }
            else if (!isValidLength)
            {
                newPasswordInfoText.text = "Must be 8-30 characters.";
                return false;
            }
            else if (!hasLowercase || !hasUppercase || !hasDigit || !hasSymbol)
            {
                List<string> strings = new();
                if (!hasLowercase) strings.Add("lower");
                if (!hasUppercase) strings.Add("upper");
                if (!hasDigit) strings.Add("digit");
                if (!hasSymbol) strings.Add("symbol");
                
                newPasswordInfoText.text = $"At least one {string.Join(", ", strings)}.";
                return false;
            }
            else
            {
                newPasswordInfoText.text = string.Empty;
                return true;
            }
        }
        
        private bool ValidateConfirmPassword(string confirmPassword)
        {
            bool isNullOrEmpty = string.IsNullOrEmpty(confirmPassword);
            bool isPasswordMatch = confirmPassword.Equals(newPasswordInput.text);
            
            if (isNullOrEmpty)
            {
                confirmPasswordInfoText.text = "Confirm password is empty.";
                return false;
            }
            else if (!isPasswordMatch)
            {
                confirmPasswordInfoText.text = "Password does not match.";
                return false;
            }
            else
            {
                confirmPasswordInfoText.text = string.Empty;
                return true;
            }
        }
        
        
        
        
        
    }
}
