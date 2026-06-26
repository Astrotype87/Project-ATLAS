using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectATLAS.UI
{
    /// <summary>
    /// Class for displaying overlay status for operations that take time
    /// such as logging in or saving to cloud.
    /// </summary>
    public class UIOverlay : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text detailsText;
        [SerializeField] private TMP_Text bottomText;
        [SerializeField] private Button screenButton;
        
        [Header("Icons")]
        [SerializeField] private Sprite loadingIcon;
        [SerializeField] private Sprite successIcon;
        [SerializeField] private Sprite failedIcon;
        
        [Header("Animation")]
        [SerializeField] private UnityEngine.Animation rotateAnimation;
        
        [Header("Logging")]
        [SerializeField] private bool activateDebugLog;
        
        [Header("Test")]
        [SerializeField] private Message loading;
        [SerializeField] private Message message;
        
        private Action closingAction;
        
        public Message LoadingInfo => loading;
        public Message MessageInfo => message;
        
        
        // MONOBEHAVIOUR METHODS
        private void Start()
        {
            screenButton.onClick.AddListener(CloseOverlay);
        }
        
        
        // PUBLIC METHODS
        public void OpenOverlay()
        {
            gameObject.SetActive(true);
        }
        
        public void CloseOverlay()
        {
            closingAction?.Invoke();
            closingAction = null;
            gameObject.SetActive(false);
        }
        
        public void DisplayLoading(Message message)
        {
            DisplayLoading(message.title, message.details, message.icon, message.bottom);
        }
        
        /// <summary> Display a loading message. Clicking anywhere to close is disabled. </summary>
        public void DisplayLoading(string title, string details, Icon icon, string bottom)
        {
            OpenOverlay();
            
            if (rotateAnimation) rotateAnimation.Play();
            
            if (titleText) titleText.text = title;
            if (detailsText) detailsText.text = details;
            if (iconImage) iconImage.sprite = GetIcon(icon);
            if (bottomText) bottomText.text = bottom;
            if (screenButton) screenButton.enabled = false;
            
            if (activateDebugLog) Debug.Log($"{title} : {details}");
        }
        
        public void DisplayMessage(Message message)
        {
            DisplayMessage(message.title, message.details, message.icon, message.bottom);
        }
        
        /// <summary> Display an overlay message. Clicking anywhere to close is enabled. </summary>
        public void DisplayMessage(string title, string details, Icon icon, string bottom)
        {
            OpenOverlay();
            
            if (rotateAnimation)
            {
                rotateAnimation.Rewind();
                rotateAnimation.Stop();
                
            }
            
            if (titleText) titleText.text = title;
            if (detailsText) detailsText.text = details;
            if (iconImage)
            {
                iconImage.sprite = GetIcon(icon);
                
                // Reset icon rotation after animation
                Vector3 eulerAngles = iconImage.transform.eulerAngles;
                eulerAngles.z = 0f;
                iconImage.transform.eulerAngles = eulerAngles;
            }
            if (bottomText) bottomText.text = bottom;
            if (screenButton) screenButton.enabled = true;
            
            if (activateDebugLog) Debug.Log($"{title} : {details}");
        }
        
        public void DoAfterClosing(Action action)
        {
            closingAction = action;
        }
        
        // PRIVATE METHODS
        private Sprite GetIcon(Icon icon)
        {
            return icon switch
            {
                Icon.Loading => loadingIcon,
                Icon.Success => successIcon,
                Icon.Failed => failedIcon,
                _ => loadingIcon
            };
        }
        
        
        
        // ENUMS & STRUCTS
        public enum Icon
        {
            Loading, Success, Failed
        }
        
        [Serializable]
        public struct Message
        {
            public string title;
            [TextArea] public string details;
            public Icon icon;
            public string bottom;
            
            public Message(string title, string details, Icon icon, string bottom)
            {
                this.title = title;
                this.details = details;
                this.icon = icon;
                this.bottom = bottom;
            }
        }
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(UIOverlay))]
    public class UIOverlayEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var script = target as UIOverlay;
            if (GUILayout.Button("Open"))
            {
                script.OpenOverlay();
            }
            if (GUILayout.Button("Show Loading"))
            {
                script.DisplayLoading(script.LoadingInfo);
            }
            if (GUILayout.Button("Show Message"))
            {
                script.DisplayMessage(script.MessageInfo);
            }
            if (GUILayout.Button("Close"))
            {
                script.CloseOverlay();
            }
        }
    }
    #endif
}
