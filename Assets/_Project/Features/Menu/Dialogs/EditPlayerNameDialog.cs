using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KBCore.Refs;
using ProjectATLAS.Authentication;
using ProjectATLAS.UI;

namespace ProjectATLAS.Menu
{
    public class EditPlayerNameDialog : UIDialog
    {
        [Header("Components")]
        [SerializeField] private TMP_InputField playerNameInput;
        [SerializeField] private TMP_Text playerNameInfoText;
        [SerializeField] private Button applyButton;
        
        [Header("References")]
        [SerializeField, Scene] private UIOverlay overlay;
        [SerializeField, Scene] private AccountPage accountPage;
        
        
        
        // MONOBEHAVIOUR METHODS
        protected override void Start()
        {
            base.Start();
            
            applyButton.onClick.AddListener(OnApply);
        }
        
        
        // PUBLIC METHODS
        public override void ResetDialog()
        {
            playerNameInput.text = AuthenticationManager.Instance.PlayerName;
            playerNameInput.onValueChanged.AddListener(text => ValidatePlayerName(text));
            
            TMP_Text placeholder = playerNameInput.placeholder as TMP_Text;
            if (placeholder) placeholder.text = AuthenticationManager.Instance.PlayerName;
            
            playerNameInfoText.text = string.Empty;
        }
        
        
        // EVENT LISTENER METHODS
        private async void OnApply()
        {
            await UpdatePlayerNameAsync();
        }
        
        
        // PRIVATE METHODS
        private async Task UpdatePlayerNameAsync()
        {
            string playerName = playerNameInput.text;
            
            if (!ValidatePlayerName(playerName))
            {
                return;
            }
            
            overlay.DisplayLoading("Updating...",
                "Updating your player name.",
                UIOverlay.Icon.Loading, "");
            
            AuthResult authResult = await AuthenticationManager.Instance.UpdatePlayerNameAsync(playerName);
            if (authResult.Status == UpdatePlayerNameResult.Success)
            {
                overlay.DisplayMessage("Player name updated",
                    "Your player name has been updated online. The changes will be visible in leaderboards.",
                    UIOverlay.Icon.Success, "Return to profile screen.");
                
                overlay.DoAfterClosing(() => {
                    base.CloseDialog();
                    accountPage.ResetPage();
                });
            }
            else
            {
                overlay.DisplayMessage("Logout failed", authResult.Message, UIOverlay.Icon.Failed, "Press anywhere to close");
            }
        }
        
        private bool ValidatePlayerName(string playerName)
        {
            bool isNullOrEmpty = string.IsNullOrEmpty(playerName);
            bool isValidLength = playerName.Length <= 50;
            bool hasSpace = playerName.Contains(" ");
            
            if (isNullOrEmpty)
            {
                playerNameInfoText.text = "Player name is empty.";
                return false;
            }
            else if (!isValidLength)
            {
                playerNameInfoText.text = "Maximum of 50 characters only.";
                return false;
            }
            else if (hasSpace)
            {
                playerNameInfoText.text = "Spaces are not allowed.";
                return false;
            }
            else
            {
                playerNameInfoText.text = string.Empty;
                return true;
            }
        }
        
        
    }
}
