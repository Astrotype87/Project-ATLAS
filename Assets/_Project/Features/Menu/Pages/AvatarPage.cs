using UnityEngine;
using TMPro;
using KBCore.Refs;

using ProjectATLAS.UI;
using ProjectATLAS.GameData;
using ProjectATLAS.Avatar;
using ProjectATLAS.Gameplay;
using UnityEngine.UI;
using ProjectATLAS.CloudSave;

namespace ProjectATLAS.Menu
{
    public class AvatarPage : UIPage
    {
        [Header("Components")]
        [SerializeField, Child(Flag.Editable)] private AvatarButton[] avatarButtons;
        [SerializeField] private Image avatarHead;
        [SerializeField] private TMP_InputField displayNameInput;
        [SerializeField] private TMP_InputField ageInput;
        [SerializeField] private UIToggleButton maleToggle;
        [SerializeField] private UIToggleButton femaleToggle;
        [SerializeField] private UIToggleButton otherToggle;
        
        private GameDataManager gameDataManager;
        private LevelManager levelManager;
        private AvatarManager avatarManager;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            foreach (var avatarButton in avatarButtons)
            {
                avatarButton.OnClicked += AvatarButton_OnClicked;
            }
            
            displayNameInput.onEndEdit.AddListener(DisplayNameInput_onEndEdit);
            ageInput.onEndEdit.AddListener(AgeInput_onEndEdit);
            maleToggle.OnValueChanged += MaleToggle_OnClicked;
            femaleToggle.OnValueChanged += FemaleToggle_OnClicked;
            otherToggle.OnValueChanged += OtherToggle_OnClicked;
            
            TryInitializeServices();
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();
        }
        
        
        // UIPage METHODS
        public override void OpenPage()
        {
            base.OpenPage();
            TryInitializeServices(); // Ensure services are ready
            
            if (levelManager != null)
            {
                int lastCompletedChapter = levelManager.LastCompletedPostTest();
                
                foreach (var avatarButton in avatarButtons)
                {
                    // Example logic:
                    // If avatar index > last completed + 1 → locked
                    bool isLocked = avatarButton.CharacterIndex > lastCompletedChapter;
                    avatarButton.SetLockedState(isLocked);
                }
            }
            else
            {
                Debug.LogWarning($"{nameof(AvatarPage)}: LevelManager is still null, cannot update unlock states.");
            }
            
            // Load configuration from game data
            // ✅ Load saved configuration from GameData
            if (gameDataManager != null)
            {
                var details = gameDataManager.DetailsData;
                var avatar = gameDataManager.AvatarData;
                
                // Load avatar head sprite
                if (!avatarManager) avatarManager = AvatarManager.Instance;
                if (avatarManager != null)
                {
                    var avatarData = avatarManager.GetAvatarProfileByID(avatar.avatarIndex);
                    avatarHead.sprite = avatarData.characterHead;
                }
                
                // Load text fields
                displayNameInput.text = details.displayName;
                ageInput.text = details.age > 0 ? details.age.ToString() : "0";
                
                // Load gender toggles
                maleToggle.SetValue(details.gender == "Male");
                femaleToggle.SetValue(details.gender == "Female");
                otherToggle.SetValue(details.gender == "Other");
                
                // Load selected avatar button (if applicable)
                foreach (var avatarButton in avatarButtons)
                {
                    avatarButton.SetAsSelected(avatarButton.CharacterIndex == avatar.avatarIndex);
                }
            }
            else
            {
                Debug.LogWarning($"{nameof(AvatarPage)}: GameDataService is null — cannot load saved configuration.");
            }
        }

        public override void ClosePage()
        {
            base.ClosePage();
            
            if (gameDataManager)
            {
                gameDataManager.SaveData();
            }
            
            if (CloudSaveManager.Instance != null)
            {
                _ = CloudSaveManager.Instance.SaveCloudDataUnderAutoSave();
            }
        }
        
        
        // EVENT LISTENER METHODS
        private void AvatarButton_OnClicked(AvatarProfile avatarProfile)
        {
            TryInitializeServices();
            
            if (gameDataManager != null)
            {
                int avatarIndex = avatarProfile.characterIndex;
                gameDataManager.AvatarData.avatarIndex = avatarIndex;
                
                // Set head icon
                if (!avatarManager) avatarManager = AvatarManager.Instance;
                if (avatarManager != null)
                {
                    var avatarData = avatarManager.GetAvatarProfileByID(avatarIndex);
                    avatarHead.sprite = avatarData.characterHead;
                }
            }
            else
            {
                Debug.LogWarning($"{nameof(AvatarPage)}: GameDataService not found — cannot save avatar selection.");
            }
        }
        
        private void DisplayNameInput_onEndEdit(string text)
        {
            if (gameDataManager) gameDataManager.DetailsData.displayName = text;
        }
        
        private void AgeInput_onEndEdit(string text)
        {
            if (gameDataManager)
            {
                if (!int.TryParse(text, out int age))
                    ageInput.text = "0";
                
                gameDataManager.DetailsData.age = age;
            }
        }
        
        private void MaleToggle_OnClicked(UIToggleButton toggleButton, bool active)
        {
            if (gameDataManager && active && active) gameDataManager.DetailsData.gender = "Male";
        }
        
        private void FemaleToggle_OnClicked(UIToggleButton toggleButton, bool active)
        {
            if (gameDataManager && active) gameDataManager.DetailsData.gender = "Female";
        }
        
        private void OtherToggle_OnClicked(UIToggleButton toggleButton, bool active)
        {
            if (gameDataManager && active) gameDataManager.DetailsData.gender = "Other";
        }
        
        
        
        // PRIVATE METHODS
        private void TryInitializeServices()
        {
            // Safely retrieve singleton services if missing
            if (gameDataManager == null)
            {
                gameDataManager = GameDataManager.Instance;
                if (gameDataManager == null)
                    Debug.LogWarning($"{nameof(AvatarPage)}: Failed to find GameDataService instance.");
            }
            
            if (levelManager == null)
            {
                levelManager = LevelManager.Instance;
                if (levelManager == null)
                    Debug.LogWarning($"{nameof(AvatarPage)}: Failed to find LevelManager instance.");
            }
        }
    }
}
