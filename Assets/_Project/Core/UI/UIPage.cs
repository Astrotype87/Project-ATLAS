using System;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectATLAS.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIPage : MonoBehaviour
    {
        [Header("UI Page")]
        [SerializeField] protected string pageName;
        
        private UIPageGroup pageGroup;
        
        // PROPERTIES
        public string PageName => pageName;
        public UIPageGroup PageGroup => pageGroup; 
        public bool IsVisible { get; private set; }
        public event Action OnOpened;
        public event Action OnClosed;
        
        
        // MONOBEHAVIOUR METHODS
        protected virtual void Start()
        {
            ResetPage();
        }
        
        protected virtual void OnValidate()
        {
            gameObject.name = string.IsNullOrEmpty(pageName)
                ? "UI Page"
                : pageName + " Page";
        }
        
        // PUBLIC METHODS
        public virtual void OpenPage()
        {
            ResetPage();
            gameObject.SetActive(true);
            IsVisible = true;
            OnOpened?.Invoke();
        }
        
        public virtual void ClosePage()
        {
            IsVisible = false;
            gameObject.SetActive(false);
            OnClosed?.Invoke();
        }
        
        public virtual void ResetPage()
        {
            
        }
        
        public void SetPageGroup(UIPageGroup pageGroup)
        {
            this.pageGroup = pageGroup;
        }
        
        public void OpenPageInGroup()
        {
            if (pageGroup)
                pageGroup.OpenPage(this);
            else
                Debug.LogWarning($"This UIPage \"{pageName}\" is not referenced by ay UIPageGroup.");
        }
    }
}
