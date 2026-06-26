using UnityEngine;
using UnityEngine.UI;
using KBCore.Refs;
using TMPro;

using ProjectATLAS.Menu;
using ProjectATLAS.UI;

namespace ProjectATLAS.Library.Guidebooks
{
    public class GuidebookButton : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private UIPage guidebookContainerPage;
        [SerializeField] private GuidebookDataPage guidebookPage;
        [SerializeField] private bool isLocked;
        
        [Header("Components")]
        [SerializeField, Self] private Button button;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private GameObject lockObject;
        
        public int Level => guidebookPage.Level;
        
        private void Awake()
        {
            button.onClick.AddListener(Button_onClick);
        }
        
        private void OnValidate()
        {
            UpdateUI();
        }
        
        public void SetLocked(bool isLocked)
        {
            this.isLocked = isLocked;
            UpdateUI();
        }
        
        private void UpdateUI()
        {
            if (guidebookPage)
            {
                this.name = $"Guide Button {guidebookPage.GuidebookID}";
                
                if (titleText) titleText.text = isLocked
                    ? "LOCKED"
                    : $"GUIDE {guidebookPage.GuidebookID}";
                
                if (nameText) nameText.text = isLocked
                    ? $"Complete level {guidebookPage.Level} to unlock."
                    : guidebookPage.GuidebookName;
                
                if (lockObject) lockObject.SetActive(isLocked);
                
                if (button) button.interactable = !isLocked;
            }
        }
        
        private void Button_onClick()
        {
            if (guidebookContainerPage) guidebookContainerPage.OpenPageInGroup();
            if (guidebookPage) guidebookPage.OpenPageInGroup();
        }
    }
}
