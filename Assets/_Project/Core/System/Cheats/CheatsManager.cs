using UnityEngine;

using ProjectATLAS.Architecture;

namespace ProjectATLAS.Cheats
{
    public class CheatsManager : PersistentSingletonMonoBehaviour<CheatsManager> 
    {
        public static bool EnableCheats => Instance != null && Instance.enableCheats;
        public static bool UnlockAllLevels => Instance != null && EnableCheats && Instance.unlockAllLevels;
        public static bool UnlockAllGuidebooks => Instance != null && EnableCheats && Instance.unlockAllGuidebooks;
        public static bool UnlockAllGlossary => Instance != null && EnableCheats && Instance.unlockAllGlossary;
        
        public bool enableCheats = false;
        public bool unlockAllLevels = false;
        public bool unlockAllGuidebooks = false;
        public bool unlockAllGlossary = false;
        
        
    }
}
