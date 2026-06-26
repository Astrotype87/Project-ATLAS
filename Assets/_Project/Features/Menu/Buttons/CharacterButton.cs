using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KBCore.Refs;

using ProjectATLAS.UI;
using ProjectATLAS.Avatar;

namespace ProjectATLAS.Menu
{
    public class AvatarButton : MonoBehaviour
    {
        [Header("Details")]
        [SerializeField] private AvatarProfile avatarProfile;
        [SerializeField] private int characterIndex;
        [SerializeField] private string characterName;
        [SerializeField] private Sprite characterSprite;
        [SerializeField] private bool isLocked;
        [SerializeField, TextArea] private string lockedMessage;
        
        [Header("Components")]
        [SerializeField, Self] private Button button;
        [SerializeField, Self(Flag.Editable | Flag.Optional)] private UIToggleButton uIToggleButton;
        [SerializeField, Child(Flag.Editable | Flag.Optional)] private Image characterImage;
        [SerializeField] private GameObject lockedObject;
        [SerializeField] private TMP_Text lockedText;
        
        // PROPERTIES
        public string CharacterName => characterName;
        public Sprite CharacterSprite => characterSprite;
        public int CharacterIndex => characterIndex;
        
        public event Action<AvatarProfile> OnClicked;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            button.onClick.AddListener(Button_onClick);
        }
        
        private void OnValidate()
        {
            this.ValidateRefs();
            
            LoadAvatarProfile();
            UpdateButton();
        }
        
        private void Start()
        {
            // Optional safety: ensure data is synced at runtime
            LoadAvatarProfile();
            UpdateButton();
        }
        
        private void Button_onClick()
        {
            if (isLocked)
            {
                Debug.Log($"Character '{characterName}' is locked. {lockedMessage}");
                return;
            }
            
            OnClicked?.Invoke(avatarProfile);
        }
        
        
        // PUBLIC METHODS
        public void UpdateButton()
        {
            this.name = $"Char: {characterName}";
            
            if (uIToggleButton)
            {
                uIToggleButton.offStyle.text = isLocked ? "Locked" : characterName;
                uIToggleButton.onStyle.text = isLocked ? "Locked" : characterName;
                uIToggleButton.RefreshStyle();
            }
            
            if (characterImage)
            {
                characterImage.sprite = characterSprite;
                characterImage.color = isLocked ? Color.black : Color.white;
            }
            
            if (lockedObject)
                lockedObject.SetActive(isLocked);
            
            if (lockedText)
                lockedText.text = lockedMessage;
            
            if (button)
                button.interactable = !isLocked;
        }
        
        /// <summary>
        /// Updates the locked state and message, then refreshes the button visuals.
        /// </summary>
        /// <param name="locked">Whether this character is locked.</param>
        /// <param name="message">Optional locked message to display.</param>
        public void SetLockedState(bool locked, string message = "")
        {
            isLocked = locked;
            
            if (!string.IsNullOrEmpty(message))
                lockedMessage = message;
            
            UpdateButton();
        }
        
        public void SetAsSelected(bool isSelected)
        {
            uIToggleButton.SetValue(isSelected);
        }
        
        
        // PRIVATE METHODS
        private void LoadAvatarProfile()
        {
            if (avatarProfile == null)
                return;
            
            characterIndex = avatarProfile.characterIndex;
            characterName = avatarProfile.characterName;
            characterSprite = avatarProfile.characterSprite;
        }
    }
}
