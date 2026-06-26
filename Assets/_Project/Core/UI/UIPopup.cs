using UnityEngine;
using UnityEngine.UI;

namespace ProjectATLAS.UI
{
    public class UIPopup : MonoBehaviour
    {
        [Header("UI Popup")]
        [SerializeField] private GameObject overlay;
        [SerializeField] private Button closeButton;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            closeButton.onClick.AddListener(ClosePopup);
        }
        
        
        // PUBLIC METHODS
        public void OpenPopup()
        {
            overlay.SetActive(true);
            gameObject.SetActive(true);
        }
        
        public void ClosePopup()
        {
            overlay.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
