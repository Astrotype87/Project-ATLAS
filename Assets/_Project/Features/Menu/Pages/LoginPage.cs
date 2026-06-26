using System.Collections;
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
    public class LoginPage : UIPage
    {
        [Header("Components")]
        [SerializeField] private TMP_InputField usernameInput;
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private UIToggleButton showPasswordToggle;
        [SerializeField] private TMP_Text usernameInfoText;
        [SerializeField] private TMP_Text passwordInfoText;
        [SerializeField] private UIToggleButton loadGameDataToggle;
        [SerializeField] private Button loginButton;
        
        [Header("References")]
        [SerializeField, Scene] private UIOverlay overlay;
        [SerializeField, Scene] private LoginDialog loginDialog;
        [SerializeField, Scene] private ProfilePage profilePage;
        [SerializeField, Scene] private AccountPage accountPage;
        
        
        // MONOBEHAVIOUR METHODS
        protected override void Start()
        {
            base.Start();
            
            usernameInput.onValueChanged.AddListener(OnUsernameInput);
            passwordInput.onValueChanged.AddListener(OnPasswordInput);
            showPasswordToggle.OnValueChanged += OnToggleShowPassword;
            loginButton.onClick.AddListener(() => StartCoroutine(OnLogin()));
        }
        
        
        // PUBLIC METHODS
        public override void ResetPage()
        {
            usernameInput.text = string.Empty;
            usernameInput.ForceLabelUpdate();
            
            passwordInput.text = string.Empty;
            passwordInput.contentType = TMP_InputField.ContentType.Password;
            passwordInput.ForceLabelUpdate();
            
            showPasswordToggle.SetValue(false);
            
            usernameInfoText.text = string.Empty;
            passwordInfoText.text = string.Empty;
            
            loadGameDataToggle.SetValue(true);
        }
        
        
        // EVENT LISTENER METHODS
        private void OnToggleShowPassword(UIToggleButton toggleButton, bool isOn)
        {
            passwordInput.contentType = isOn
                ? TMP_InputField.ContentType.Standard
                : TMP_InputField.ContentType.Password;
            passwordInput.ForceLabelUpdate();
        }
        
        private void OnUsernameInput(string text)
        {
            bool isInputTextEmpty = string.IsNullOrEmpty(text.Trim());
            bool isErrorTextEmpty = string.IsNullOrEmpty(usernameInfoText.text.Trim());
            
            if (!isInputTextEmpty && !isErrorTextEmpty)
            {
                usernameInfoText.text = string.Empty;
            }
        }
        
        private void OnPasswordInput(string text)
        {
            bool isInputTextEmpty = string.IsNullOrEmpty(text.Trim());
            bool isErrorTextEmpty = string.IsNullOrEmpty(passwordInfoText.text.Trim());
            
            if (!isInputTextEmpty && !isErrorTextEmpty)
            {
                passwordInfoText.text = string.Empty;
            }
        }
        
        private IEnumerator OnLogin()
        {
            yield return LoginAsync();
        }
        
        
        // PRIVATE METHODS
        private async Task LoginAsync()
        {
            // Get username and password
            string username = usernameInput.text.Trim();
            string password = passwordInput.text.Trim();
            
            // Check for empty username and password input
            bool isUsernameEmpty = string.IsNullOrEmpty(username);
            bool isPasswordEmpty = string.IsNullOrEmpty(password);
            if (isUsernameEmpty || isPasswordEmpty)
            {
                string message =
                    isUsernameEmpty && isUsernameEmpty ? "Username and password is empty."
                    : isUsernameEmpty ? "Username is empty."
                    : isPasswordEmpty ? "Password is empty."
                    : "";
                if (isUsernameEmpty) usernameInfoText.text = "Username is empty.";
                if (isPasswordEmpty) passwordInfoText.text = "Password is empty.";
                
                Debug.Log(message);
                return;
            }
            
            // Show login confirmation dialog
            loginDialog.OpenDialog();
            bool isConfirmed = await loginDialog.WaitForConfirmation();
            if (!isConfirmed) return;
            
            // Display overlay message: logging in...
            overlay.DisplayLoading("Logging in...", "Logging in to the account...", UIOverlay.Icon.Loading, "");
            
            // Call login backend
            AuthResult authResult = await AuthenticationManager.Instance.LoginAsync(username, password);
            if (authResult.Status == LoginResult.Success)
            {
                if (loadGameDataToggle.IsOn)
                {
                    await LoadCloudData();
                }
                else
                {
                    // Display overlay message: login success
                    overlay.DisplayMessage("Login success", "You have logged in successfully!", UIOverlay.Icon.Success, "Return to profile screen.");
                    overlay.DoAfterClosing(() => {
                        profilePage.PageGroup.BackPage();
                    });
                }
            }
            else
            {
                overlay.DisplayMessage("Login failed", authResult.Message, UIOverlay.Icon.Failed, "Press anywhere to close.");
                Debug.Log(authResult.Message);
            }
        }
        
        private async Task LoadCloudData()
        {
            // Display overlay message
            overlay.DisplayLoading("Logging in...", "Loading cloud data...", UIOverlay.Icon.Loading, "");
            
            // Get CloudSaveManager instance
            CloudSaveManager cloudSaveManager = CloudSaveManager.Instance;
            if (cloudSaveManager == null)
            {
                overlay.DisplayMessage("Cloud load failed", "(Developer) CloudSaveManager instance is null.", UIOverlay.Icon.Failed, "Press anywhere to close.");
                Debug.LogError("CloudSaveManager instance is null.");
                return;
            }
            
            // Load data from cloud
            var loadResult = await cloudSaveManager.LoadCloudDataAsync();
            if (loadResult.Status == LoadResult.Success)
            {
                overlay.DisplayMessage("Login success", "You have logged in and loaded cloud data successfully.", UIOverlay.Icon.Success, "Press anywhere to close.");
                overlay.DoAfterClosing(() => {
                    profilePage.PageGroup.BackPage();
                });
            }
            else
            {
                overlay.DisplayMessage("Login success, cloud failed", "You have logged in but failed to load cloud data. Try again in account page.", UIOverlay.Icon.Failed, "Proceed to account page.");
                overlay.DoAfterClosing(() => {
                    accountPage.OpenPageInGroup();
                });
                Debug.Log(loadResult.Message);
            }
        }
    }
}
