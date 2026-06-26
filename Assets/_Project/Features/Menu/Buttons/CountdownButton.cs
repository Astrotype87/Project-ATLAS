using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using KBCore.Refs;
using System;

namespace ProjectATLAS.UI
{
    public class CountdownButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField, Child] private TMP_Text textComponent;
        [SerializeField] private string text;
        [SerializeField] private int countdownFrom;
        
        private Coroutine countdownCoroutine;
        private int time;
        
        public event Action OnButtonClick;
        
        
        // MONOBEHAVIOUR METHODS
        private void OnValidate()
        {
            this.ValidateRefs();
            
            if (textComponent) textComponent.text = $"{text} ({countdownFrom})";
        }
        
        
        // POINTER EVENT METHODS
        public void OnPointerDown(PointerEventData eventData)
        {
            if (countdownCoroutine == null)
            {
                countdownCoroutine = StartCoroutine(CountdownCoroutine());
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (countdownCoroutine != null)
            {
                StopCoroutine(countdownCoroutine);
                countdownCoroutine = null;
            }
            
            if (time <= 0)
            {
                Debug.Log($"OnPointerClick: {time}");
                OnButtonClick?.Invoke();
            }
            
            ResetCountdown();
        }
        
        
        // PRIVATE METHODS
        public IEnumerator CountdownCoroutine()
        {
            for (time = countdownFrom; time >= 0; time--)
            {
                if (textComponent) textComponent.text = $"{text} ({time})";
                yield return new WaitForSeconds(1f);
            }
            countdownCoroutine = null;
        }
        
        public void ResetCountdown()
        {
            time = countdownFrom;
            if (textComponent) textComponent.text = $"{text} ({countdownFrom})";
        }
        
        
    }
}
