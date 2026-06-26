using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KBCore.Refs;
using ProjectATLAS.Authentication;
using ProjectATLAS.UI;

namespace ProjectATLAS.Menu
{
    public class AccountPage : UIPage
    {
        [Header("Components")]
        [SerializeField] private TMP_Text playerNameText;
        [SerializeField] private TMP_Text usernameText;
        
        
        // MONOBEHAVIOUR METHODS
        protected override void Start()
        {
            base.Start();
        }
        
        
        // PUBLIC METHODS
        public override void ResetPage()
        {
            string playerName = AuthenticationManager.Instance.PlayerName;
            string playerTag = AuthenticationManager.Instance.PlayerTag;
            
            playerNameText.text = $"{playerName}<color=#32323280>{playerTag}</color>";
            usernameText.text = AuthenticationManager.Instance.CachedUsername;
        }
    }
}
