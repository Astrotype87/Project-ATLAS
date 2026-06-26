using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

namespace ProjectATLAS.UI.Animation
{
    public class UITextAnimation : MonoBehaviour
    {
        [SerializeField] private TMP_Text textComponent;
        [SerializeField] private float tokensPerSecond = 50;
        
        private Coroutine _typingAnimationCoroutine;
        private WaitForSeconds _waitForSeconds;
        private StringBuilder _stringBuilder;
        private string _targetText;
        private bool _isAnimating;
        
        public bool IsAnimating => _isAnimating;
        
        
        /// <summary> Play typing animation. </summary>
        public void Play(string text)
        {
            if (_isAnimating) Complete();
            _typingAnimationCoroutine = StartCoroutine(AnimateTypingText(text));
        }
        
        /// <summary> Stop typing animation midway. </summary>
        public void Stop()
        {
            if (_isAnimating)
            {
                StopCoroutine(_typingAnimationCoroutine);
                _isAnimating = false;
            }
        }
        
        /// <summary> Force complete typing animation. </summary>
        public void Complete()
        {
            if (_isAnimating)
            {
                StopCoroutine(_typingAnimationCoroutine);
                textComponent.text = _targetText;
                _isAnimating = false;
            }
        }
        
        
        private IEnumerator AnimateTypingText(string text)
        {
            _isAnimating = true;
            _targetText = text;
            
            _waitForSeconds = new WaitForSeconds(1.0f / tokensPerSecond);
            if (_stringBuilder == null) _stringBuilder = new();
            _stringBuilder.Clear();
            
            for (int i = 0; i < _targetText.Length; i++)
            {
                yield return _waitForSeconds;
                _stringBuilder.Append(_targetText[i]);
                textComponent.text = _stringBuilder.ToString();
            }
            
            _targetText = textComponent.text;
            _isAnimating = false;
        }
        
        // public enum AnimationType
        // {
        //     PerCharacter,
        //     PerWord
        // }
    }
}
