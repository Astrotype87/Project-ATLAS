using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KBCore.Refs;

using ProjectATLAS.Authentication;
using ProjectATLAS.UI;
using ProjectATLAS.GameData;
using ProjectATLAS.CloudSave;

namespace ProjectATLAS.Menu
{
    public class RegistrationPage : UIPage
    {
        [Header("Components")]
        // [SerializeField] private TMP_InputField playerNameInput;
        [SerializeField] private TMP_InputField usernameInput;
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private TMP_InputField confirmPasswordInput;
        
        [SerializeField] private UIToggleButton showPasswordToggle;
        [SerializeField] private UIToggleButton showConfirmPasswordToggle;
        
        // [SerializeField] private TMP_Text playerNameInfoText;
        [SerializeField] private TMP_Text usernameInfoText;
        [SerializeField] private TMP_Text passwordInfoText;
        [SerializeField] private TMP_Text confirmPasswordInfoText;
        
        [SerializeField] private Button registerButton;
        
        [Header("References")]
        [SerializeField, Scene] private UIOverlay overlay;
        [SerializeField, Scene] private RegistrationDialog registrationDialog;
        [SerializeField, Scene] private ProfilePage profilePage;
        
        
        // MONOBEHAVIOUR METHODS
        protected override void Start()
        {
            base.Start();
            
            // playerNameInput.onValueChanged.AddListener(text => ValidatePlayerName(text));
            usernameInput.onValueChanged.AddListener(text => ValidateUsername(text));
            passwordInput.onValueChanged.AddListener(text => ValidatePassword(text));
            confirmPasswordInput.onValueChanged.AddListener(text => ValidateConfirmPassword(text));
            
            showPasswordToggle.OnValueChanged += OnToggleShowPassword;
            showConfirmPasswordToggle.OnValueChanged += OnToggleShowConfirmPassword;
            
            registerButton.onClick.AddListener(() => StartCoroutine(OnRegister()));
        }
        
        
        // PUBLIC METHODS
        public override void ResetPage()
        {
            // playerNameInput.text = string.Empty;
            // playerNameInput.ForceLabelUpdate();
            
            usernameInput.text = string.Empty;
            usernameInput.ForceLabelUpdate();
            
            passwordInput.text = string.Empty;
            passwordInput.contentType = TMP_InputField.ContentType.Password;
            passwordInput.ForceLabelUpdate();
            
            confirmPasswordInput.text = string.Empty;
            confirmPasswordInput.contentType = TMP_InputField.ContentType.Password;
            confirmPasswordInput.ForceLabelUpdate();
            
            showPasswordToggle.SetValue(false);
            showConfirmPasswordToggle.SetValue(false);
            
            // playerNameInfoText.text = "You will get a random player name.";
            usernameInfoText.text = string.Empty;
            passwordInfoText.text = string.Empty;
            confirmPasswordInfoText.text = string.Empty;
        }
        
        
        // EVENT LISTENER METHODS
        private void OnToggleShowPassword(UIToggleButton toggleButton, bool isOn)
        {
            passwordInput.contentType = isOn
                ? TMP_InputField.ContentType.Standard
                : TMP_InputField.ContentType.Password;
            passwordInput.ForceLabelUpdate();
        }
        
        private void OnToggleShowConfirmPassword(UIToggleButton toggleButton, bool isOn)
        {
            confirmPasswordInput.contentType = isOn
                ? TMP_InputField.ContentType.Standard
                : TMP_InputField.ContentType.Password;
            confirmPasswordInput.ForceLabelUpdate();
        }
        
        private IEnumerator OnRegister()
        {
            yield return RegisterAsync();
        }
        
        
        // PRIVATE METHODS
        private async Task RegisterAsync()
        {
            // Get username and password
            // string playerName = playerNameInput.text;
            string username = usernameInput.text;
            string password = passwordInput.text;
            string confirmPassword = confirmPasswordInput.text;
            
            // Validate player name, username, password, and confirm password
            // bool isPlayerNameValid = ValidatePlayerName(playerName);
            bool isUsernameValid = ValidateUsername(username);
            bool isPasswordValid = ValidatePassword(password);
            bool isConfirmPasswordValid = ValidateConfirmPassword(confirmPassword);
            if (!isUsernameValid || !isPasswordValid || !isConfirmPasswordValid)
            {
                List<string> strings = new();
                // if (!isPlayerNameValid) strings.Add("player name");
                if (!isUsernameValid) strings.Add("username");
                if (!isPasswordValid) strings.Add("password");
                if (!isConfirmPasswordValid) strings.Add("confirm password");
                
                string message = $"Invalid {string.Join(", ", strings)}.";
                Debug.Log(message);
                return;
            }
            
            // Show registration confirmation dialog
            registrationDialog.OpenDialog();
            bool isConfirmed = await registrationDialog.WaitForConfirmation();
            if (!isConfirmed) return;
            
            // Display overlay message: registering...
            overlay.DisplayLoading("Registering...", "Creating new account...", UIOverlay.Icon.Loading, "");
            
            // Call registration backend
            AuthResult authResult = await AuthenticationManager.Instance.RegisterAsync(username, password);
            if (authResult.Status == RegistrationResult.Success)
            {
                // Save cloud data
                overlay.DisplayLoading("Registering...", "Saving progress to cloud...", UIOverlay.Icon.Loading, "");
                await CloudSaveManager.Instance.SaveCloudDataAsync();
                
                // Display overlay message: registration success
                overlay.DisplayMessage("Registration success",
                    "You have successfully created a new account!",
                    UIOverlay.Icon.Success, "Return to profile screen.");
                
                overlay.DoAfterClosing(() => {
                    profilePage.PageGroup.BackPage(); // close reg page
                    GameDataManager.Instance.SaveData();
                });
            }
            else
            {
                // Display overlay message: registration failed
                overlay.DisplayMessage("Registration failed", authResult.Message, UIOverlay.Icon.Failed, "Press anywhere to close");
            }
        }
        
        // private bool ValidatePlayerName(string playerName)
        // {
        //     bool isNullOrEmpty = string.IsNullOrEmpty(playerName);
        //     bool isValidLength = playerName.Length <= 50;
        //     bool hasSpace = playerName.Contains(" ");
            
        //     if (isNullOrEmpty)
        //     {
        //         playerNameInfoText.text = "You will get a random player name.";
        //         return true;
        //     }
        //     else if (!isValidLength)
        //     {
        //         playerNameInfoText.text = "Maximum of 50 characters only.";
        //         return false;
        //     }
        //     else if (hasSpace)
        //     {
        //         playerNameInfoText.text = "Spaces are not allowed.";
        //         return false;
        //     }
        //     else
        //     {
        //         playerNameInfoText.text = string.Empty;
        //         return true;
        //     }
        // }
        
        private bool ValidateUsername(string username)
        {
            bool isNullOrEmpty = string.IsNullOrEmpty(username);
            bool isValidCharacters = Regex.IsMatch(username, @"^[A-Za-z0-9.\-@_]+$");
            bool isValidLength = username.Length >= 3 && username.Length <= 20;
            
            if (isNullOrEmpty)
            {
                usernameInfoText.text = "Username is empty.";
                return false;
            }
            else if (!isValidCharacters)
            {
                usernameInfoText.text = "Supported symbols are ., -, @, and _.";
                return false;
            }
            else if (!isValidLength)
            {
                usernameInfoText.text = "Must be 3-20 characters only.";
                return false;
            }
            else
            {
                usernameInfoText.text = string.Empty;
                return true;
            }
        }
        
        private bool ValidatePassword(string password)
        {
            bool isNullOrEmpty = string.IsNullOrEmpty(password);
            bool isValidLength = password.Length >= 8 && password.Length <= 30;
            bool hasLowercase = Regex.IsMatch(password, @"[a-z]");
            bool hasUppercase = Regex.IsMatch(password, @"[A-Z]");
            bool hasDigit = Regex.IsMatch(password, @"[0-9]");
            bool hasSymbol = Regex.IsMatch(password, @"[^a-zA-Z0-9]");
            
            if (isNullOrEmpty)
            {
                passwordInfoText.text = "Password is empty.";
                return false;
            }
            else if (!isValidLength)
            {
                passwordInfoText.text = "Must be 8-30 characters.";
                return false;
            }
            else if (!hasLowercase || !hasUppercase || !hasDigit || !hasSymbol)
            {
                List<string> strings = new();
                if (!hasLowercase) strings.Add("lower");
                if (!hasUppercase) strings.Add("upper");
                if (!hasDigit) strings.Add("digit");
                if (!hasSymbol) strings.Add("symbol");
                
                passwordInfoText.text = $"At least one {string.Join(", ", strings)}.";
                return false;
            }
            else
            {
                passwordInfoText.text = string.Empty;
                return true;
            }
        }
        
        private bool ValidateConfirmPassword(string confirmPassword)
        {
            bool isNullOrEmpty = string.IsNullOrEmpty(confirmPassword);
            bool isPasswordMatch = confirmPassword.Equals(passwordInput.text);
            
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
