using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectATLAS.Authentication.Debugging
{
    [CreateAssetMenu(fileName = "RegistrationData", menuName = "Scriptable Objects/RegistrationData")]
    public class RegistrationData : ScriptableObject
    {
        public List<AccountData> accountData;
        
        public void Register(string username, string password)
        {
            accountData.Add(new AccountData(username, password));
        }
        
        public void UpdatePassword(string username, string newPassword)
        {
            AccountData accountToUpdate = accountData.FirstOrDefault(acc => acc.username.ToLower() == username.ToLower());
            accountToUpdate.password = newPassword;
        }
        
        public void DeleteAccount(string username)
        {
            AccountData accountToDelete = accountData.FirstOrDefault(acc => acc.username.ToLower() == username.ToLower());
            accountData.Remove(accountToDelete);
        }
    }
    
    [Serializable]
    public class AccountData
    {
        public string username;
        public string password;

        public AccountData(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }
    
    
}
