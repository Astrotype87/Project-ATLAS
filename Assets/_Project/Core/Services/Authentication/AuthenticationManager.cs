using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Unity.Services.Core;
using Unity.Services.Authentication;

namespace ProjectATLAS.Authentication
{
    using ProjectATLAS.Architecture;
    using ProjectATLAS.CloudSave;
    using ProjectATLAS.GameData;
    
    /// <summary> Facade singleton class for accessing Unity Authentication features. </summary>
    public class AuthenticationManager : PersistentSingletonMonoBehaviour<AuthenticationManager>
    {
        [SerializeField] private string websiteToPing = "https://google.com";
        [SerializeField] private int requestTimeout = 5;
        [SerializeField] private GameDataManager gameDataService;
        
        public string RawPlayerName { get {
            if (string.IsNullOrEmpty(AuthenticationService.Instance.PlayerName)) return string.Empty;
            try { return AuthenticationService.Instance.PlayerName; }
            catch (Exception) { return string.Empty; }
        }}
        public string PlayerName { get {
            int lastIndex = RawPlayerName.LastIndexOf('#');
            return lastIndex >= 0 ? RawPlayerName.Substring(0, lastIndex) : RawPlayerName;
        }}
        public string PlayerTag { get {
            int lastIndex = RawPlayerName.LastIndexOf('#');
            return lastIndex >= 0 ? RawPlayerName.Substring(lastIndex) : RawPlayerName;
        }}
        public string CachedUsername
        {
            get => PlayerPrefs.GetString("CachedUsername", string.Empty);
            set => PlayerPrefs.SetString("CachedUsername", value);
        }
        public string CachedPassword
        {
            get => PlayerPrefs.GetString("CachedPassword", string.Empty);
            set => PlayerPrefs.SetString("CachedPassword", value);
        }
        public bool IsLoginCached
        {
            get
            {
                return PlayerPrefs.GetInt("IsLoginCached", 0) == 1 && IsPreviouslyLoggedIn;
            }
            set => PlayerPrefs.SetInt("IsLoginCached", value ? 1 : 0);
        }
        public bool IsLoggedIn => AuthenticationService.Instance.IsSignedIn;
        public bool IsPreviouslyLoggedIn => AuthenticationService.Instance.SessionTokenExists;
        
        public bool IsInitialized { get; private set; }
        
        public event Action OnInitialized;
        public event Action<string, string> OnRegisterSuccess;
        public event Action<string, string> OnChangePasswordSuccess;
        public event Action<string> OnDeleteAccountSuccess;
        
        
        // PUBLIC METHODS
        /// <summary> Initializes the AuthenticationManager and signs in anonymously if no valid session exists. </summary>
        public async Task InitializeAsync()
        {
            // Initialize Unity Services if not already initialized
            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                await UnityServices.InitializeAsync();
                Debug.Log("Unity Services initialized.");
            }
            
            // Login anonymously if no valid session exists
            if (IsLoginCached)
            {
                await LoginAnonymouslyAsync();
            }
            
            // Mark as initialized
            IsInitialized = true;
            OnInitialized?.Invoke();
        }
        
        
        /// <summary> Checks the network status whether internet connection is available, failed to connect, or device is offline. </summary>
        public async Task<AuthResult> CheckNetworkStatusAsync()
        {
            if (IsDeviceOffline())
            {
                return new AuthResult(NetworkResult.Offline, "The device is offline.");
            }
            else if (!await CheckInternetAccessAsync())
            {
                return new AuthResult(NetworkResult.NoInternetAccess, "Cannot connect to the internet.");
            }
            else
            {
                return new AuthResult(NetworkResult.Online, "Internet connection is available.");
            }
        }
        
        /// <summary> Registers a new user with the given username and password. </summary>
        public async Task<AuthResult> RegisterAsync(string username, string password)
        {
            // Check network status
            AuthResult result = await CheckNetworkStatusAsync();
            if (result.Status != NetworkResult.Online) return result;
            
            try
            {
                // Proceed with registration
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
                
                // Trigger registration event for saving registration records to scriptable object
                OnRegisterSuccess?.Invoke(username, password);
                
                try
                {
                    // Set player name to username
                    await AuthenticationService.Instance.UpdatePlayerNameAsync(username);
                }
                catch (Exception) { }
                
                // Update cached username and password
                CachedUsername = username;
                CachedPassword = password;
                IsLoginCached = true;
                GameDataManager.Instance.AccountCacheData.username = CachedUsername;
                
                return new AuthResult(RegistrationResult.Success, "Registration successful!");
            }
            catch (Exception ex)
            {
                return new AuthResult(LoginResult.RequestFailed, ex.Message);
            }
        }
        
        /// <summary> Logs in a user with the given username and password. </summary>
        public async Task<AuthResult> LoginAsync(string username, string password)
        {
            // Check network status
            AuthResult result = await CheckNetworkStatusAsync();
            if (result.Status != NetworkResult.Online) return result;
            
            try
            {
                // Proceed with login
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
                
                // Update cached username and password
                CachedUsername = username;
                CachedPassword = password;
                IsLoginCached = true;
                GameDataManager.Instance.AccountCacheData.username = CachedUsername;
                
                return new AuthResult(LoginResult.Success, "Login successful.");
            }
            catch (Exception ex)
            {
                return new AuthResult(LoginResult.RequestFailed, ex.Message);
            }
        }
        
        /// <summary> Logs out the current user. </summary>
        public async Task<AuthResult> LogOut()
        {
            // Check network status
            AuthResult result = await CheckNetworkStatusAsync();
            if (result.Status != NetworkResult.Online) return result;
            
            // Proceed with logout
            AuthenticationService.Instance.SignOut();
            
            // Clear cached username and password
            CachedUsername = string.Empty;
            CachedPassword = string.Empty;
            IsLoginCached = false;
            
            // Clear local game data
            GameDataManager.Instance.ResetDataWithPlaceholder();
            
            return new AuthResult(LogoutResult.Success, "Logout successful.");
        }
        
        /// <summary> Logs in anonymously using existing session token if available. </summary>
        public async Task<AuthResult> LoginAnonymouslyAsync()
        {
            // Check network status
            AuthResult result = await CheckNetworkStatusAsync();
            if (result.Status != NetworkResult.Online)
                return result;
            
            // Check if session token exists
            if (!AuthenticationService.Instance.SessionTokenExists)
            {
                return new AuthResult(LoginResult.NoSessionToken, "No valid session found for anonymous login.");
            }
            
            try
            {
                // Proceed with anonymous login
                Debug.Log("Session token found, signing in anonymously...");
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                
                // Update login cache status
                IsLoginCached = true;
                GameDataManager.Instance.AccountCacheData.username = CachedUsername;
                
                Debug.Log("Anonymous login successful.");
                return new AuthResult(LoginResult.Success, "Anonymous login successful.");
            }
            catch (Exception ex)
            {
                return new AuthResult(LoginResult.RequestFailed, ex.Message);
            }
        }
        
        
        /// <summary> Updates the current user's password. </summary>
        public async Task<AuthResult> UpdatePasswordAsync(string currentPassword, string newPassword)
        {
            // Check network status
            AuthResult result = await CheckNetworkStatusAsync();
            if (result.Status != NetworkResult.Online) return result;
            
            try
            {
                // Proceed with password update
                await AuthenticationService.Instance.UpdatePasswordAsync(currentPassword, newPassword);
                OnChangePasswordSuccess?.Invoke(CachedUsername, newPassword);
                
                // Update cached password
                CachedPassword = newPassword;
                
                // Check if the new password is the same as the current password
                bool isSamePassword = currentPassword == newPassword;
                if (!isSamePassword)
                {
                    return new AuthResult(UpdatePasswordResult.Success, "Update password successful.");
                }
                else
                {
                    return new AuthResult(UpdatePasswordResult.SamePassword, "The current and new password is the same.");
                }
            }
            catch (Exception ex)
            {
                return new AuthResult(UpdatePasswordResult.Unknown, ex.Message);
            }
        }
        
        /// <summary>
        /// Updates the current user's player name. <br/>
        /// The use of player name is outdated. It is now replaced with the actual username.
        /// </summary>
        [Obsolete("The use of player name is outdated. It is now replaced with the actual username.")]
        public async Task<AuthResult> UpdatePlayerNameAsync(string name)
        {
            AuthResult result = await CheckNetworkStatusAsync();
            if (result.Status != NetworkResult.Online) return result;
            
            try
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync(name);
                return new AuthResult(UpdatePlayerNameResult.Success, "Update player name successful.");
            }
            catch (Exception ex)
            {
                return new AuthResult(LoginResult.RequestFailed, ex.Message);
            }
        }
        
        /// <summary> Deletes the current user's account. </summary>
        public async Task<AuthResult> DeleteAccountAsync()
        {
            // Check network status
            AuthResult result = await CheckNetworkStatusAsync();
            if (result.Status != NetworkResult.Online) return result;
            
            try
            {
                // Delete cloud save
                await CloudSaveManager.Instance.DeleteCloudDataAsync();
                
                // Proceed with account deletion
                await AuthenticationService.Instance.DeleteAccountAsync();
                OnDeleteAccountSuccess?.Invoke(CachedUsername);
                
                // Clear cached username and password
                CachedUsername = string.Empty;
                CachedPassword = string.Empty;
                IsLoginCached = false;
                
                return new AuthResult(DeleteAccountResult.Success, "Account deleted.");
            }
            catch (Exception ex)
            {
                return new AuthResult(LoginResult.RequestFailed, ex.Message);
            }
        }
        
        
        
        // PRIVATE METHODS
        private bool IsDeviceOffline()
        {
            return Application.internetReachability == NetworkReachability.NotReachable;
        }
        
        private async Task<bool> CheckInternetAccessAsync()
        {
            using UnityWebRequest request = UnityWebRequest.Head(websiteToPing);
            request.timeout = requestTimeout;
            await request.SendWebRequest();
            
            bool isConnectionError = request.result == UnityWebRequest.Result.ConnectionError;
            bool isProtocolError = request.result == UnityWebRequest.Result.ProtocolError;
            
            return !isConnectionError && !isProtocolError && request.responseCode == 200;
        }
        
        
        
    }
    
}
