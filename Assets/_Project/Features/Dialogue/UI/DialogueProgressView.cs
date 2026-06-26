using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectATLAS.Dialogue
{
    public class DialogueProgressView : MonoBehaviour
    {
        [Header("Data")]
        public string title;
        public int current;
        public int total;
        
        [Header("Components")]
        [SerializeField] private RectTransform progressFill;
        [SerializeField] private TMP_Text progressText;
        
        
        private void OnValidate()
        {
            SetTitle(title);
            SetProgress(current, total);
        }
        
        
        public void SetTitle(string title)
        {
            this.title = title;
            if (progressText) progressText.text = $"{title} ({current}/{total})";
        }
        
        public void SetProgress(int current, int total)
        {
            this.current = Mathf.Clamp(current, 0, total);
            this.total = Mathf.Max(total, 1);
            
            float progress = Mathf.Clamp01((float)this.current / this.total);
            
            if (progressText) progressText.text = $"{this.title} ({this.current}/{this.total})";
            if (progressFill) progressFill.anchorMax = new(progress, progressFill.anchorMax.y);
        }
        
        
    }
}
