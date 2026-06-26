using UnityEngine;

using ProjectATLAS.Architecture;
using ProjectATLAS.Dialogue;
using ProjectATLAS.GameData;

namespace ProjectATLAS.Avatar
{
    public class AvatarManager : PersistentSingletonMonoBehaviour<AvatarManager>
    {
        [SerializeField] private AvatarProfile[] avatarProfiles;
        
        /// <summary>
        /// Returns the AvatarProfile matching the given ID, or null if not found.
        /// </summary>
        public AvatarProfile GetAvatarProfileByID(int avatarID)
        {
            if (avatarProfiles == null || avatarProfiles.Length == 0)
                return null;
            
            foreach (var profile in avatarProfiles)
            {
                if (profile != null && profile.characterIndex == avatarID)
                    return profile;
            }
            
            return null;
        }
        
        public AvatarProfile GetCurrentAvatarProfile()
        {
            // Avatar profile list null checks
            if (avatarProfiles == null || avatarProfiles.Length == 0)
                return null;
            
            // Get reference to game data
            GameDataManager gameDataService = GameDataManager.Instance;
            if (gameDataService == null)
                return null;
            
            // Get avatar index
            int avatarIndex = gameDataService.AvatarData.avatarIndex;
            
            // Return reference to avatar profiles
            foreach (var profile in avatarProfiles)
            {
                if (profile != null && profile.characterIndex == avatarIndex)
                    return profile;
            }
            
            return null;
        }
    }
}
