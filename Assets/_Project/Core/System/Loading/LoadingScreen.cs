using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PrimeTween;

using ProjectATLAS.Architecture;

namespace ProjectATLAS.System
{
    public class LoadingScreen : PersistentSingletonGameObject<LoadingScreen>
    {
        [Header("Preview")]
        [SerializeField] private string text;
        [SerializeField, Range(0, 1)] private float progress;
        [SerializeField] private string title;
        [SerializeField, TextArea] private string subtitle;
        
        [Header("Loading")]
        [SerializeField] private CanvasGroup loadingGroup;
        [SerializeField] private CanvasGroup infoGroup;
        [SerializeField] private Image background;
        [SerializeField] private Button button;
        
        [Header("Progress")]
        [SerializeField] private TMP_Text loadingText;
        [SerializeField] private TMP_Text progressText;
        [SerializeField] private Image progressFill;
        
        [Header("Info")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text subtitleText;
        
        private bool _isClickPending;
        private bool _isOpen;
        
        // MONOBEHAVIOUR METHODS
        protected override void Awake()
        {
            base.Awake();
            button.onClick.AddListener(Button_onClick);
            
            loadingGroup.alpha = 0;
            infoGroup.alpha = 0;
            gameObject.SetActive(false);
        }
        
        private void OnValidate()
        {
            progressText.text = $"{progress * 100:00.00}%";
            progressFill.fillAmount = progress;
            
            titleText.text = title;
            subtitleText.text = subtitle;
        }
        
        
        // PUBLIC METHODS
        public void OpenUnanimated()
        {
            if (_isOpen) return;
            _isOpen = true;
            
            SetLoadingText("");
            SetProgress(0.0f);
            gameObject.SetActive(true);
            
            loadingGroup.alpha = 1;
        }
        
        public IEnumerator Open()
        {
            if (_isOpen) yield break;
            _isOpen = true;
            
            SetLoadingText("");
            SetProgress(0.0f);
            gameObject.SetActive(true);
            
            yield return Tween.Alpha(loadingGroup, 1, 0.2f).ToYieldInstruction();
        }
        
        public IEnumerator Close(bool isAnimated = true)
        {
            if (!_isOpen) yield break;
            _isOpen = false;
            
            if (isAnimated)
            {
                yield return Tween.Alpha(loadingGroup, 0, 0.2f).ToYieldInstruction();
            }
            else
            {
                loadingGroup.alpha = 0;
            }
            
            infoGroup.alpha = 0.0f;
            gameObject.SetActive(false);
        }
        
        public IEnumerator WaitForConfirmation(string message = "Press anywhere to continue.")
        {
            loadingText.text = message;
            
            yield return new WaitUntil(() => _isClickPending);
            _isClickPending = false;
        }
        
        
        public void SetLoadingText(string text)
        {
            loadingText.text = text;
        }
        
        public void SetProgress(float progress)
        {
            progress = Mathf.Clamp01(progress);
            
            progressText.text = $"{progress * 100:00.00}%";
            progressFill.fillAmount = progress;
        }
        
        public void ShowInfo(string title, string subtitle)
        {
            infoGroup.alpha = 1.0f;
            
            titleText.text = title;
            subtitleText.text = subtitle;
        }
        
        
        // EVENT LISTENER METHODS
        public void Button_onClick()
        {
            _isClickPending = true;
        }
        
        
    }
}
