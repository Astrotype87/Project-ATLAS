using UnityEngine;
using KBCore.Refs;

using ProjectATLAS.Avatar;

namespace ProjectATLAS
{
    public class PlayerAvatar : MonoBehaviour
    {
        [SerializeField, Child] private SpriteRenderer spriteRenderer;
        [SerializeField] private AvatarProfile avatarProfile;
        
        // PROPERTY
        public AvatarProfile AvatarProfile => avatarProfile;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            AvatarManager avatarManager = AvatarManager.Instance;
            avatarProfile = avatarManager.GetCurrentAvatarProfile();
            
            SetAvatarProfile(avatarProfile);
        }
        
        private void OnValidate()
        {
            this.ValidateRefs();
            
            LoadAvatarSprite();
        }
        
        
        // PUBLIC METHODS
        /// <summary> Sets a new avatar profile and immediately updates the sprite. </summary>
        /// <param name="profile">The new avatar profile to apply.</param>
        public void SetAvatarProfile(AvatarProfile profile)
        {
            if (profile == null)
            {
                Debug.LogWarning($"{nameof(PlayerAvatar)}: Tried to set a null AvatarProfile.");
                return;
            }
            
            avatarProfile = profile;
            LoadAvatarSprite();
        }
        
        // PRIVATE METHODS
        private void LoadAvatarSprite()
        {
            if (spriteRenderer == null)
            {
                Debug.LogWarning($"{nameof(PlayerAvatar)}: Missing SpriteRenderer reference.");
                return;
            }
            
            if (avatarProfile == null)
            {
                spriteRenderer.sprite = null;
                return;
            }
            
            spriteRenderer.sprite = avatarProfile.characterSprite;
        }
    }
}
