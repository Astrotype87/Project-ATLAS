using UnityEngine;
using KBCore.Refs;
using ProjectATLAS.UI;
using ProjectATLAS.Authentication;

namespace ProjectATLAS.Authentication.Debugging
{
    public class RegistrationRecorder : MonoBehaviour
    {
        [Header("Asset")]
        [SerializeField] private RegistrationData registrationData;
        
        [Header("References")]
        [SerializeField, Scene(Flag.Editable | Flag.Optional)] private AuthenticationManager authenticationManager;
        
        
        // MONOBEHAVIOUR METHODS
        private void Start()
        {
            authenticationManager.OnRegisterSuccess += OnRegisterSuccess;
            authenticationManager.OnChangePasswordSuccess += OnChangePasswordSuccess;
            authenticationManager.OnDeleteAccountSuccess += OnDeleteAccountSuccess;
        }
        
        private void OnValidate()
        {
            this.ValidateRefs();
        }
        
        // EVENT LISTENER METHODS
        private void OnRegisterSuccess(string username, string password)
        {
            registrationData.Register(username, password);
        }
        
        private void OnChangePasswordSuccess(string username, string newPassword)
        {
            registrationData.UpdatePassword(username, newPassword);
        }
        
        private void OnDeleteAccountSuccess(string username)
        {
            registrationData.DeleteAccount(username);
        }
        
        
        
        
    }
}
