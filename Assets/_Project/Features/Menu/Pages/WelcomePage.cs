using UnityEngine;
using TMPro;

using ProjectATLAS.UI;
using ProjectATLAS.GameData;

namespace ProjectATLAS.Menu
{
    public class WelcomePage : UIPage
    {
        [Header("Components")]
        [SerializeField] private TMP_InputField displayNameInput;
        [SerializeField] private TMP_InputField ageInput;
        [SerializeField] private UIToggleButton maleToggle;
        [SerializeField] private UIToggleButton femaleToggle;
        [SerializeField] private UIToggleButton otherToggle;
        
        private GameDataManager gameDataManager;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            displayNameInput.onEndEdit.AddListener(DisplayNameInput_onEndEdit);
            ageInput.onEndEdit.AddListener(AgeInput_onEndEdit);
            maleToggle.OnValueChanged += MaleToggle_OnClicked;
            femaleToggle.OnValueChanged += FemaleToggle_OnClicked;
            otherToggle.OnValueChanged += OtherToggle_OnClicked;
            
            gameDataManager = GameDataManager.Instance;
            
            gameDataManager.DetailsData.displayName = displayNameInput.text;
            gameDataManager.DetailsData.age = !int.TryParse(ageInput.text, out int age) ? age : 0;
            gameDataManager.DetailsData.gender = maleToggle.IsOn ? "Male" : femaleToggle.IsOn ? "Female" : "Other";
            gameDataManager.DetailsData.completedFirstTime = true;
        }
        
        
        // EVENT LISTENER METHODS
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
        
    }
}
