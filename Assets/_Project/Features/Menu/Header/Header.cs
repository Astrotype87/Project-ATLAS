using TMPro;
using UnityEngine;

namespace ProjectATLAS.Menu
{
    public class Header : MonoBehaviour
    {
        public enum BarMode { Full, Short }
        
        [Header("Settings")]
        [SerializeField] private bool visible = true;
        [SerializeField] private string headerText = "Title";
        [SerializeField] private bool allCaps;
        [SerializeField] private BarMode barMode;
        
        [Header("Components")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TMP_Text textComponent;
        [SerializeField] private RectTransform fullBar;
        [SerializeField] private RectTransform shortBar;
        
        // MONOBEHAVIOUR METHODS
        private void OnValidate()
        {
            SetVisible(visible);
            SetText(headerText, allCaps);
            SetBarMode(barMode);
        }
        
        // PUBLIC METHODS
        public void SetVisible(bool visible)
        {
            this.visible = visible;
            if (canvasGroup)
            {
                canvasGroup.alpha = visible ? 1f : 0f;
                canvasGroup.interactable = visible;
            }
        }
        
        public void SetText(string headerText, bool allCaps = false)
        {
            this.headerText = headerText;
            
            if (textComponent)
            {
                textComponent.text = headerText;
                
                if (allCaps)
                    textComponent.fontStyle |= FontStyles.UpperCase;
                else
                    textComponent.fontStyle &= ~FontStyles.UpperCase;
            }
        }
        
        public void SetBarMode(BarMode barMode)
        {
            this.barMode = barMode;
            
            if (barMode == BarMode.Full)
            {
                if (fullBar) fullBar.gameObject.SetActive(true);
                if (shortBar) shortBar.gameObject.SetActive(false);
            }
            else if (barMode == BarMode.Short)
            {
                if (fullBar) fullBar.gameObject.SetActive(false);
                if (shortBar) shortBar.gameObject.SetActive(true);
            }
        }
    }
}
