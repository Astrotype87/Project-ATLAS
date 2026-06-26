using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectATLAS.Dialogue
{
    using ProjectATLAS.UI.Animation;
    
    public class DialoguePanelView : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private Sprite icon;
        [SerializeField] private new string name;
        [SerializeField] [TextArea(1, 4)] private string message;
        
        [Header("Animation")]
        [SerializeField] private UITextAnimation textAnimation;
        
        [Header("Components")]
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private Button playButton;
        [SerializeField] private Button audioButton;
        [SerializeField] private Image playImage;
        [SerializeField] private Image audioImage;
        
        [Header("Icons")]
        [SerializeField] private Sprite playIcon;
        [SerializeField] private Sprite stopIcon;
        [SerializeField] private Sprite unmutedIcon;
        [SerializeField] private Sprite mutedIcon;
        
        public bool IsAnimating => textAnimation.IsAnimating;
        
        public event Action OnPlayClicked;
        public event Action OnAudioClicked;
        
        
        private void OnEnable()
        {
            if (playButton) playButton.onClick.AddListener(PlayButton_onClick);
            if (audioButton) audioButton.onClick.AddListener(AudioButton_onClick);
        }
        
        private void OnDisable()
        {
            if (playButton) playButton.onClick.RemoveListener(PlayButton_onClick);
            if (audioButton) audioButton.onClick.RemoveListener(AudioButton_onClick);
        }
        
        private void OnValidate()
        {
            SetIcon(icon);
            SetName(name);
            SetMessage(message);
            SetPlayIcon(false);
            SetAudioIcon(false);
        }
        
        
        public void SetIcon(Sprite sprite)
        {
            if (iconImage) iconImage.sprite = this.icon = sprite;
        }
        
        public void SetName(string text)
        {
            if (nameText) nameText.text = this.name = text;
        }
        
        public void SetMessage(string text)
        {
            if (messageText) messageText.text = this.message = text;
        }
        
        public void SetMessageAnimated(string text)
        {
            if (textAnimation)
                textAnimation.Play(text);
        }
        
        
        
        public void SetPlayIcon(bool isPlaying)
        {
            if (playImage) playImage.sprite = isPlaying ? stopIcon : playIcon;
        }
        
        public void SetAudioIcon(bool isMuted)
        {
            if (audioImage) audioImage.sprite = isMuted ? mutedIcon : unmutedIcon;
        }
        
        public void CompleteAnimation()
        {
            if (textAnimation)
            {
                textAnimation.Complete();
            }
        }
        
        
        // CALLBACK
        private void PlayButton_onClick() => OnPlayClicked?.Invoke();
        private void AudioButton_onClick() => OnAudioClicked?.Invoke();
        
        
    }
}
