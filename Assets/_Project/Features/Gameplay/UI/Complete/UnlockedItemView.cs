using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectATLAS
{
    public class UnlockedItemView : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private Sprite icon;
        [SerializeField] private new string name;
        [SerializeField] private string description;
        
        [Header("Components")]
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private CanvasGroup canvasGroup;
        
        // MONOBEHAVIOUR METHODS
        private void OnValidate()
        {
            if (iconImage) iconImage.sprite = icon;
            if (nameText) nameText.text = name;
            if (descriptionText) descriptionText.text = description;
        }
        
        // PUBLIC METHODS
        public void DisplayUnlockedItem(Sprite icon, string name, string description)
        {
            this.icon = icon;
            this.name = name;
            this.description = description;
            
            if (iconImage) iconImage.sprite = icon;
            if (nameText) nameText.text = name;
            if (descriptionText) descriptionText.text = description;
        }
        
        public void SetVisible(bool isVisible) => canvasGroup.alpha = isVisible ? 1f : 0f;
    }
}
